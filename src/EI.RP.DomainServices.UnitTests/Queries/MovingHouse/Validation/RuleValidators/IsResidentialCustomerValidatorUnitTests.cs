using AutoFixture;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.DomainServices.Queries.MovingHouse.Validation;
using EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators;
using EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators.OutputResolvers;
using EI.RP.DomainServices.UnitTests.Infrastructure.RepositoryMocks;
using EI.RP.TestServices;
using NUnit.Framework;
using Moq.AutoMock;

namespace EI.RP.DomainServices.UnitTests.Queries.MovingHouse.Validation.RuleValidators
{
	[TestFixture]
	internal class IsResidentialCustomerValidatorUnitTests : UnitTestFixture<
		IsResidentialCustomerValidatorUnitTests.TestContext,
		IsResidentialCustomerValidator>
	{
		public static IEnumerable<TestCaseData> CanResolveDuelAccountCases()
		{
			var paymentTypeTruthTable = new List<Tuple<AccountDeterminationType, AccountDeterminationType, OutputType>>()
			{
				new Tuple<AccountDeterminationType, AccountDeterminationType, OutputType>(AccountDeterminationType.ResidentialCustomer, AccountDeterminationType.ResidentialCustomer, OutputType.Passed),
				new Tuple<AccountDeterminationType, AccountDeterminationType, OutputType>(AccountDeterminationType.ServiceProvider, AccountDeterminationType.ServiceProvider, OutputType.Failed),
				new Tuple<AccountDeterminationType, AccountDeterminationType, OutputType>(AccountDeterminationType.NonResidentialCustomer, AccountDeterminationType.NonResidentialCustomer, OutputType.Failed),
				new Tuple<AccountDeterminationType, AccountDeterminationType, OutputType>(AccountDeterminationType.NonResidentialCustomer, AccountDeterminationType.ResidentialCustomer, OutputType.Failed)
			};

			foreach (var testCase in paymentTypeTruthTable)
			{
				var movingHouseRulesValidationResult = new MovingHouseRulesValidationResult
				{
					Output = testCase.Item3,
					MovingHouseValidationType = MovingHouseValidationType.IsResidentialCustomer
				};
				yield return new TestCaseData(testCase.Item1, testCase.Item2).Returns(movingHouseRulesValidationResult);
			}
		}
		public static IEnumerable<TestCaseData> CanResolveSingle()
		{
			var accountDeterminationTruthTable = new List<Tuple<AccountDeterminationType, OutputType>>()
			{
				new Tuple<AccountDeterminationType, OutputType>(AccountDeterminationType.ResidentialCustomer, OutputType.Passed),
				new Tuple<AccountDeterminationType, OutputType>(AccountDeterminationType.NonResidentialCustomer, OutputType.Failed),
				new Tuple<AccountDeterminationType, OutputType>(AccountDeterminationType.ServiceProvider, OutputType.Failed),
			};

			foreach (var testCase in accountDeterminationTruthTable)
			{
				var movingHouseRulesValidationResult = new MovingHouseRulesValidationResult
				{
					Output = testCase.Item2,
					MovingHouseValidationType = MovingHouseValidationType.IsResidentialCustomer
				};
				yield return new TestCaseData(testCase.Item1).Returns(movingHouseRulesValidationResult);
			}
		}

		[TestCaseSource(nameof(CanResolveDuelAccountCases))]
		public async Task<MovingHouseRulesValidationResult> CanResolveDuelAccounts(AccountDeterminationType electricityAccountDeterminationType, AccountDeterminationType gasAccountDeterminationType)
		{
			return await Context
				.WithElectricityAccount()
				.WithElectricityAccountDeterminationType(electricityAccountDeterminationType)
				.WithGasAccount()
				.WithGasAccountDeterminationType(gasAccountDeterminationType)
				.Sut
				.Resolve(Context.Query);
		}

		[TestCaseSource(nameof(CanResolveSingle))]
		public async Task<MovingHouseRulesValidationResult> CanResolveElectricity(AccountDeterminationType accountDeterminationType)
		{
			return await Context
				.WithElectricityAccount()
				.WithElectricityAccountDeterminationType(accountDeterminationType)
				.Sut
				.Resolve(Context.Query);
		}

		[TestCaseSource(nameof(CanResolveSingle))]
		public async Task<MovingHouseRulesValidationResult> CanResolveGas(AccountDeterminationType accountDeterminationType)
		{
			return await Context
				.WithGasAccount()
				.WithGasAccountDeterminationType(accountDeterminationType)
				.Sut
				.Resolve(Context.Query);
		}

		public class TestContext : UnitTestContext<IsResidentialCustomerValidator>
		{
			private bool _addElectricity;
			private bool _addGas;
			private AccountDeterminationType _electricityAccountDeterminationType;
			private AccountDeterminationType _gasAccountDeterminationType;

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

			public TestContext WithElectricityAccountDeterminationType(AccountDeterminationType accountDeterminationType)
			{
				_electricityAccountDeterminationType = accountDeterminationType;
				return this;
			}

			public TestContext WithGasAccountDeterminationType(AccountDeterminationType accountDeterminationType)
			{
				_gasAccountDeterminationType = accountDeterminationType;
				return this;
			}

			protected override IsResidentialCustomerValidator BuildSut(AutoMocker autoMocker)
			{
				var domainFacade = new DomainFacade();
				autoMocker.Use<ICompositeRuleValidatorOutputTypeResolver>(new CompositeRuleValidatorOutputResolver ());
				domainFacade.SetUpMocker(autoMocker);
				var cfg = new AppUserConfigurator(domainFacade);
				if (_addElectricity)
				{
					cfg.AddElectricityAccount()
						.WithElectricity24HrsDevices();
				}

				if (_addGas)
				{
					cfg.AddGasAccount()
						.WithGasDevice();
				}

				cfg.Execute();

				if (_addElectricity)
				{
					cfg.ElectricityAccount().Model.BusinessAgreement.AccountDeterminationID =
						_electricityAccountDeterminationType;
					
				}

				if (_addGas)
				{
					cfg.GasAccount().Model.BusinessAgreement.AccountDeterminationID =
						_gasAccountDeterminationType;
				}

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