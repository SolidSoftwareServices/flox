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
	internal class IsNotPAYGCustomerValidatorUnitTests : UnitTestFixture<
		IsNotPAYGCustomerValidatorUnitTests.TestContext,
		IsNotPAYGCustomerValidator>
	{
		public static IEnumerable<TestCaseData> CanResolveDuelAccountCases()
		{
			var isElecGasPAYGValid = new List<Tuple<bool, bool, OutputType>>()
			{
				new Tuple<bool, bool, OutputType>(false, false, OutputType.Passed),
				new Tuple<bool, bool, OutputType>(false, true, OutputType.Failed),
				new Tuple<bool, bool, OutputType>(true, false, OutputType.Failed),
				new Tuple<bool, bool, OutputType>(true, true, OutputType.Failed),
			};
			
			foreach (var testCase in isElecGasPAYGValid)
			{
				var movingHouseRulesValidationResult = new MovingHouseRulesValidationResult
				{
					Output = testCase.Item3,
					MovingHouseValidationType = MovingHouseValidationType.IsNotPAYGCustomer
				};
				yield return new TestCaseData(testCase.Item1, testCase.Item2).Returns(movingHouseRulesValidationResult);
			}
		}
		public static IEnumerable<TestCaseData> CanResolveSingle()
		{
			var isPAYGValid = new List<Tuple<bool, OutputType>>()
			{
				new Tuple<bool, OutputType>(false, OutputType.Passed),
				new Tuple<bool, OutputType>(true, OutputType.Failed),
			};

			foreach (var testCase in isPAYGValid)
			{
				var isValid = testCase.Item2;
				var movingHouseRulesValidationResult = new MovingHouseRulesValidationResult
				{
					Output = testCase.Item2,
					MovingHouseValidationType = MovingHouseValidationType.IsNotPAYGCustomer
				};
				yield return new TestCaseData(testCase.Item1).Returns(movingHouseRulesValidationResult);
			}
		}

		[TestCaseSource(nameof(CanResolveDuelAccountCases))]
		public async Task<MovingHouseRulesValidationResult> CanResolveDuelAccounts(bool electricityIsPaygCustomer, bool gasIsPaygCustomer)
		{
			return await Context
				.WithElectricityIsPAYGCustomer(electricityIsPaygCustomer)
				.WithGasIsPAYGCustomer(gasIsPaygCustomer)
				.Sut
				.Resolve(Context.Query);
		}

		[TestCaseSource(nameof(CanResolveSingle))]
		public async Task<MovingHouseRulesValidationResult> CanResolveElectricity(bool isPAYGCustomer)
		{
			return await Context
				.WithElectricityIsPAYGCustomer(isPAYGCustomer)
				.Sut
				.Resolve(Context.Query);
		}

		[TestCaseSource(nameof(CanResolveSingle))]
		public async Task<MovingHouseRulesValidationResult> CanResolveGas(bool isPAYGCustomer)
		{
			return await Context
				.WithGasIsPAYGCustomer(isPAYGCustomer)
				.Sut
				.Resolve(Context.Query);
		}

		public class TestContext : UnitTestContext<IsNotPAYGCustomerValidator>
		{
			private bool? _isElectricityPAYGCustomer;
			private bool? _isGasPAYGCustomer;

			public MovingHouseValidationQuery Query { get; private set; }

			public TestContext WithElectricityIsPAYGCustomer(bool isPAYGCustomer)
			{
				_isElectricityPAYGCustomer = isPAYGCustomer;
				return this;
			}

			public TestContext WithGasIsPAYGCustomer(bool isPAYGCustomer)
			{
				_isGasPAYGCustomer = isPAYGCustomer;
				return this;
			}

			protected override IsNotPAYGCustomerValidator BuildSut(AutoMocker autoMocker)
			{
				var domainFacade = new DomainFacade();
				autoMocker.Use<ICompositeRuleValidatorOutputTypeResolver>(new CompositeRuleValidatorOutputResolver ());
				domainFacade.SetUpMocker(autoMocker);
				var cfg = new AppUserConfigurator(domainFacade);

				if (_isElectricityPAYGCustomer.HasValue)
					cfg.AddElectricityAccount();

				if (_isGasPAYGCustomer.HasValue)
					cfg.AddGasAccount();

				cfg.Execute();

				var electricityAccount = cfg.ElectricityAccount()?.Model;
				var gasAccount = cfg.GasAccount()?.Model;

				if (_isElectricityPAYGCustomer.HasValue)
				{
					electricityAccount.IsPAYGCustomer = _isElectricityPAYGCustomer.Value;
				}

				if (_isGasPAYGCustomer.HasValue)
				{
					gasAccount.IsPAYGCustomer = _isGasPAYGCustomer.Value;
				}

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