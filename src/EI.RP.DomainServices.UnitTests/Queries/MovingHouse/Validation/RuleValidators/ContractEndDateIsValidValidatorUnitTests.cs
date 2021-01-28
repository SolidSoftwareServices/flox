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
	internal class ContractEndDateIsValidValidatorUnitTests : UnitTestFixture<
		ContractEndDateIsValidValidatorUnitTests.TestContext,
		ContractEndDateIsValidValidator>
	{
		public static IEnumerable<TestCaseData> CanResolveDuelAccountCases()
		{
			var isElecGasPAYGValid = new List<Tuple<DateTime?, DateTime?, OutputType>>()
			{
				new Tuple<DateTime?, DateTime?, OutputType>(DateTime.UtcNow, DateTime.UtcNow, OutputType.Failed),
				new Tuple<DateTime?, DateTime?, OutputType>(DateTime.Now.AddDays(4), DateTime.Now.AddDays(4), OutputType.Failed),
				new Tuple<DateTime?, DateTime?, OutputType>(new DateTime(9999,12,31), new DateTime(9999,12,31), OutputType.Passed),
			};
			
			foreach (var testCase in isElecGasPAYGValid)
			{
				var movingHouseRulesValidationResult = new MovingHouseRulesValidationResult
				{
					Output = testCase.Item3,
					MovingHouseValidationType = MovingHouseValidationType.ContractEndDateIsValid
				};
				yield return new TestCaseData(testCase.Item1, testCase.Item2).Returns(movingHouseRulesValidationResult);
			}
		}
		public static IEnumerable<TestCaseData> CanResolveSingle()
		{
			var isContractEndDataValidTruthTable = new List<Tuple<DateTime?, OutputType>>()
			{
				new Tuple<DateTime?, OutputType>(DateTime.UtcNow, OutputType.Failed),
				new Tuple<DateTime?, OutputType>(DateTime.Now.AddDays(4), OutputType.Failed),
				new Tuple<DateTime?, OutputType>(new DateTime(9999,12,31), OutputType.Passed),
			};

			foreach (var testCase in isContractEndDataValidTruthTable)
			{
				var movingHouseRulesValidationResult = new MovingHouseRulesValidationResult
				{
					Output = testCase.Item2,
					MovingHouseValidationType = MovingHouseValidationType.ContractEndDateIsValid
				};
				yield return new TestCaseData(testCase.Item1).Returns(movingHouseRulesValidationResult);
			}
		}

		[TestCaseSource(nameof(CanResolveDuelAccountCases))]
		public async Task<MovingHouseRulesValidationResult> CanResolveDuelAccounts(DateTime? electricityContractEndDate, DateTime? gasContractEndDate)
		{
			return await Context
				.WithElectricityAccount()
				.WithGasAccount()
				.WithElectricityContractEndDate(electricityContractEndDate)
				.WithGasContractEndDate(gasContractEndDate)
				.Sut
				.Resolve(Context.Query);
		}

		[TestCaseSource(nameof(CanResolveSingle))]
		public async Task<MovingHouseRulesValidationResult> CanResolveElectricity(DateTime? contractEndDate)
		{
			return await Context
				.WithElectricityAccount()
				.WithElectricityContractEndDate(contractEndDate)
				.Sut
				.Resolve(Context.Query);
		}

		[TestCaseSource(nameof(CanResolveSingle))]
		public async Task<MovingHouseRulesValidationResult> CanResolveGas(DateTime? contractEndDate)
		{
			return await Context
				.WithGasAccount()
				.WithGasContractEndDate(contractEndDate)
				.Sut
				.Resolve(Context.Query);
		}

		public class TestContext : UnitTestContext<ContractEndDateIsValidValidator>
		{
			private DateTime? _electricityContractEndDate;
			private DateTime? _gasContractEndDate;

			private bool _addElectricity;
			private bool _addGas;

			public MovingHouseValidationQuery Query { get; private set; }

			public TestContext WithElectricityAccount()
			{
				_addElectricity = true;
				return this;
			}

			public TestContext WithGasAccount()
			{
				_addGas = true;
				return this;
			}

			public TestContext WithElectricityContractEndDate(DateTime? contractEndDate)
			{
				_electricityContractEndDate = contractEndDate;
				return this;
			}

			public TestContext WithGasContractEndDate(DateTime? contractEndDate)
			{
				_gasContractEndDate = contractEndDate;
				return this;
			}

			protected override ContractEndDateIsValidValidator BuildSut(AutoMocker autoMocker)
			{
				var domainFacade = new DomainFacade();
				autoMocker.Use<ICompositeRuleValidatorOutputTypeResolver>(new CompositeRuleValidatorOutputResolver ());
				domainFacade.SetUpMocker(autoMocker);
				var cfg = new AppUserConfigurator(domainFacade);
				if (_addElectricity)
					cfg.AddElectricityAccount()
						.WithContractEndDate(_electricityContractEndDate);

				if (_addGas)
					cfg.AddGasAccount()
						.WithContractEndDate(_gasContractEndDate);

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