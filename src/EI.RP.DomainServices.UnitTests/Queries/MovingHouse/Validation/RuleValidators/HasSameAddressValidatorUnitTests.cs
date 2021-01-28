using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.DomainServices.InternalShared.Accounts;
using EI.RP.DomainServices.Queries.MovingHouse.Validation;
using EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators;
using EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators.OutputResolvers;
using EI.RP.DomainServices.UnitTests.Infrastructure.RepositoryMocks;
using EI.RP.TestServices;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;

namespace EI.RP.DomainServices.UnitTests.Queries.MovingHouse.Validation.RuleValidators
{
	[TestFixture]
	internal class HasSameAddressValidatorUnitTests : UnitTestFixture<
		HasSameAddressValidatorUnitTests.TestContext,
		HasSameAddressValidator>
	{
		public static IEnumerable<TestCaseData> CanResolveDuelAccountCases()
		{
			var hasSameAddressTruthTable = new List<Tuple<bool, bool, OutputType>>
			{
				new Tuple<bool, bool, OutputType>(
					false,
					false,
					OutputType.Failed),
				new Tuple<bool, bool, OutputType>(
					true,
					true,
					OutputType.Passed),
				new Tuple<bool, bool, OutputType>(
					false,
					true,
					OutputType.Failed)
			};

			foreach (var testCase in hasSameAddressTruthTable)
			{
				var movingHouseRulesValidationResult = new MovingHouseRulesValidationResult
				{
					Output = testCase.Item3,
					MovingHouseValidationType = MovingHouseValidationType.HasSameAddress
				};
				yield return new TestCaseData(testCase.Item1, testCase.Item2).Returns(movingHouseRulesValidationResult);
			}
		}

		public static IEnumerable<TestCaseData> CanResolveSingle()
		{
			var hasSameAddressTruthTable = new List<Tuple<bool, OutputType>>
			{
				new Tuple<bool, OutputType>(
					false,
					OutputType.Failed),
				new Tuple<bool, OutputType>(
					true,
					OutputType.Passed)
			};

			foreach (var testCase in hasSameAddressTruthTable)
			{
				var isValid = testCase.Item2;
				var movingHouseRulesValidationResult = new MovingHouseRulesValidationResult
				{
					Output = testCase.Item2,
					MovingHouseValidationType = MovingHouseValidationType.HasSameAddress
				};
				yield return new TestCaseData(testCase.Item1).Returns(movingHouseRulesValidationResult);
			}
		}

		[TestCaseSource(nameof(CanResolveDuelAccountCases))]
		public async Task<MovingHouseRulesValidationResult> CanResolveDuelAccounts(
			bool electricityIsOk, bool gasIsOk)
		{
			return await Context
				.WithElectricityAccount()
				.WithElectricityIsOk(electricityIsOk)
				.WithGasAccount()
				.WithGasIsOk(gasIsOk)
				.Sut
				.Resolve(Context.Query);
		}

		[TestCaseSource(nameof(CanResolveSingle))]
		public async Task<MovingHouseRulesValidationResult> CanResolveElectricity(bool isOk)
		{
			return await Context
				.WithElectricityAccount()
				.WithElectricityIsOk(isOk)
				.Sut
				.Resolve(Context.Query);
		}

		[TestCaseSource(nameof(CanResolveSingle))]
		public async Task<MovingHouseRulesValidationResult> CanResolveGas(bool isOk)
		{
			return await Context
				.WithGasAccount()
				.WithGasIsOk(isOk)
				.Sut
				.Resolve(Context.Query);
		}

		public class TestContext : UnitTestContext<HasSameAddressValidator>
		{
			private bool _addElectricity;
			private bool _addGas;
			private bool _electricityIsOk;
			private bool _gasIsOk;

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

			public TestContext WithElectricityIsOk(bool isOk)
			{
				_electricityIsOk = isOk;
				return this;
			}

			public TestContext WithGasIsOk(bool isOk)
			{
				_gasIsOk = isOk;
				return this;
			}

			protected override HasSameAddressValidator BuildSut(AutoMocker autoMocker)
			{
				var domainFacade = new DomainFacade();
				autoMocker.Use<ICompositeRuleValidatorOutputTypeResolver>(new CompositeRuleValidatorOutputResolver());
				domainFacade.SetUpMocker(autoMocker);
				var cfg = new AppUserConfigurator(domainFacade);
				if (_addElectricity)
					cfg.AddElectricityAccount()
						.WithElectricity24HrsDevices();

				if (_addGas)
					cfg.AddGasAccount()
						.WithGasDevice();

				cfg.Execute();
				var accountQueryResolverMock = autoMocker.GetMock<IAccountExtraInfoResolver>();
				if (_addElectricity)
					accountQueryResolverMock
						.Setup(x => x.AddressesMatchForBundle(cfg.ElectricityAccount().Model.AccountNumber))
						.ReturnsAsync(_electricityIsOk);

				if (_addGas)
					accountQueryResolverMock
						.Setup(x => x.AddressesMatchForBundle(cfg.GasAccount().Model.AccountNumber))
						.ReturnsAsync(_gasIsOk);

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