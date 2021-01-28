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
using EI.RP.DomainServices.UnitTests.Infrastructure.RepositoryMocks;
using EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators.OutputResolvers;
using EI.RP.TestServices;
using Moq.AutoMock;
using NUnit.Framework;

namespace EI.RP.DomainServices.UnitTests.Queries.MovingHouse.Validation.RuleValidators
{
	[TestFixture]
	internal class ContractStartedBeforeTodayValidatorUnitTests : UnitTestFixture<
		ContractStartedBeforeTodayValidatorUnitTests.TestContext,
		ContractStartedBeforeTodayValidator>
	{
		public static IEnumerable<TestCaseData> CanResolveCases()
		{
			var datetimeValues = new DateTime?[]
			{
				null,
				DateTime.UtcNow.Date,
				DateTime.UtcNow.Date.AddDays(-1),
				DateTime.UtcNow.Date.AddDays(1)
			};

			foreach (var electricityDate in datetimeValues)
			foreach (var gasDate in datetimeValues)
			{
				var isValid = electricityDate.HasValue && electricityDate.Value < DateTime.UtcNow.Date
				              || gasDate.HasValue && gasDate.Value < DateTime.UtcNow.Date;
				var movingHouseRulesValidationResult = new MovingHouseRulesValidationResult
				{
					Output = isValid ? OutputType.Passed : OutputType.Failed,
					MovingHouseValidationType =
						MovingHouseValidationType.AccountContractStartedBeforeToday
				};
				yield return new TestCaseData(electricityDate, gasDate).Returns(movingHouseRulesValidationResult);
			}
		}


		[TestCaseSource(nameof(CanResolveCases))]
		public async Task<MovingHouseRulesValidationResult> CanResolve(DateTime? electricityDate, DateTime? gasDate)
		{
			return await Context
				.WithElectricityContractStartDate(electricityDate)
				.WithGasContractStartDate(gasDate)
				.Sut
				.Resolve(Context.Query);
		}


		public class TestContext : UnitTestContext<ContractStartedBeforeTodayValidator>
		{
			private DateTime? _electricityDate;
			private DateTime? _gasDate;

			public MovingHouseValidationQuery Query { get; private set; }

			public TestContext WithElectricityContractStartDate(DateTime? electricityDate)
			{
				_electricityDate = electricityDate;
				return this;
			}

			public TestContext WithGasContractStartDate(DateTime? gasDate)
			{
				_gasDate = gasDate;
				return this;
			}

			protected override ContractStartedBeforeTodayValidator BuildSut(AutoMocker autoMocker)
			{
				var domainFacade = new DomainFacade();
				autoMocker.Use<ICompositeRuleValidatorOutputTypeResolver>(new CompositeRuleValidatorOutputResolver ());
				domainFacade.SetUpMocker(autoMocker);
				var cfg = new AppUserConfigurator(domainFacade);
				if (_electricityDate != null)
					cfg.AddElectricityAccount()
						.WithContractStartDate(_electricityDate);

				if (_gasDate != null)
					cfg.AddGasAccount()
						.WithContractStartDate(_gasDate);

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