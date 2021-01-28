using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.DomainServices.Queries.MovingHouse.Validation;
using EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators;
using EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators.OutputResolvers;
using EI.RP.DomainServices.UnitTests.Infrastructure.RepositoryMocks;
using EI.RP.TestServices;
using Moq.AutoMock;
using NUnit.Framework;

namespace EI.RP.DomainServices.UnitTests.Queries.MovingHouse.Validation.RuleValidators
{
	[TestFixture]
	internal class IsNotLossInProgressValidatorUnitTests : UnitTestFixture<
		IsNotLossInProgressValidatorUnitTests.TestContext,
		IsNotLossInProgressValidator>
	{
		public static IEnumerable<TestCaseData> CanResolveCasesDuelAccounts()
		{
			var isLossInProgressElecGasExpectedIsValid = new List<Tuple<bool, bool, OutputType>>()
			{
				new Tuple<bool, bool, OutputType>(false, false, OutputType.Passed),
				new Tuple<bool, bool, OutputType>(false , true, OutputType.Failed),
				new Tuple<bool, bool, OutputType>(true , false, OutputType.Failed),
				new Tuple<bool, bool, OutputType>(true , true, OutputType.Failed),
			};

			foreach (var test in isLossInProgressElecGasExpectedIsValid)
			{
				var movingHouseRulesValidationResult = new MovingHouseRulesValidationResult
				{
					Output = test.Item3,
					MovingHouseValidationType = MovingHouseValidationType.IsNotLossInProgress
				};

				yield return new TestCaseData(test.Item1, test.Item2).Returns(movingHouseRulesValidationResult);
			}
		}

		public static IEnumerable<TestCaseData> CanResolveCasesSingleAccountOnly()
		{
			var isLossInProgressExpectedIsValid = new List<Tuple<bool, OutputType>>()
			{
				new Tuple<bool, OutputType>(false, OutputType.Passed),
				new Tuple<bool, OutputType>(true , OutputType.Failed),
			};

			foreach (var test in isLossInProgressExpectedIsValid)
			{
				var movingHouseRulesValidationResult = new MovingHouseRulesValidationResult
				{
					Output = test.Item2,
					MovingHouseValidationType = MovingHouseValidationType.IsNotLossInProgress
				};

				yield return new TestCaseData(test.Item1).Returns(movingHouseRulesValidationResult);
			}
		}

		[TestCaseSource(nameof(CanResolveCasesSingleAccountOnly))]
		public async Task<MovingHouseRulesValidationResult> WhenElectricityOnly_CanResolve(bool isLossInProgress)
		{
			return await Context
				.WithElectricityIsLossInProgress(isLossInProgress)
				.Sut
				.Resolve(Context.Query);
		}

		[TestCaseSource(nameof(CanResolveCasesSingleAccountOnly))]
		public async Task<MovingHouseRulesValidationResult> WhenGasOnly_CanResolve(bool isLossInProgress)
		{
			return await Context
				.WithGasIsLossInProgress(isLossInProgress)
				.Sut
				.Resolve(Context.Query);
		}

		[TestCaseSource(nameof(CanResolveCasesDuelAccounts))]
		public async Task<MovingHouseRulesValidationResult> WhenDuelFuel_CanResolve(bool isElectricityLossInProgress, bool isGasLossInProgress)
		{
			return await Context
				.WithElectricityIsLossInProgress(isElectricityLossInProgress)
				.WithGasIsLossInProgress(isGasLossInProgress)
				.Sut
				.Resolve(Context.Query);
		}

		public class TestContext : UnitTestContext<IsNotLossInProgressValidator>
		{
			private bool? _isElectricityLossInProgress;
			private bool? _isGasLossInProgress;

			public MovingHouseValidationQuery Query { get; private set; }

			public TestContext WithElectricityIsLossInProgress(bool isLossInProgress)
			{
				_isElectricityLossInProgress = isLossInProgress;
				return this;
			}

			public TestContext WithGasIsLossInProgress(bool isLossInProgress)
			{
				_isGasLossInProgress = isLossInProgress;
				return this;
			}

			protected override IsNotLossInProgressValidator BuildSut(AutoMocker autoMocker)
			{
				var domainFacade = new DomainFacade();
				autoMocker.Use<ICompositeRuleValidatorOutputTypeResolver>(new CompositeRuleValidatorOutputResolver ());
				domainFacade.SetUpMocker(autoMocker);
				var cfg = new AppUserConfigurator(domainFacade);
				if (_isElectricityLossInProgress.HasValue)
					cfg.AddElectricityAccount()
						.WithIsLossInProgress(_isElectricityLossInProgress.Value);

				if (_isGasLossInProgress.HasValue)
					cfg.AddGasAccount()
						.WithIsLossInProgress(_isGasLossInProgress.Value);

				cfg.Execute();

				var electricityAccount = cfg.ElectricityAccount()?.Model;
				var gasAccount = cfg.GasAccount()?.Model;
				Query = Fixture.Build<MovingHouseValidationQuery>()
					.With(x => x.ElectricityAccountNumber, electricityAccount?.AccountNumber)
					.With(x => x.GasAccountNumber, gasAccount?.AccountNumber)
					.Without(x => x.MovingHouseType)
					.Create();
				

				return base.BuildSut(autoMocker);
			}
		}
	}
}