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
	internal class IsNotEqualizerValidatorUnitTests : UnitTestFixture<
		IsNotEqualizerValidatorUnitTests.TestContext,
		IsNotEqualizerValidator>
	{
		public static IEnumerable<TestCaseData> CanResolveDuelAccountCases()
		{
			var paymentTypeTruthTable = new List<Tuple<PaymentMethodType, PaymentMethodType, OutputType>>()
			{
				new Tuple<PaymentMethodType, PaymentMethodType, OutputType>(
					PaymentMethodType.DirectDebit,
					PaymentMethodType.Manual,
					OutputType.Passed),
				new Tuple<PaymentMethodType, PaymentMethodType, OutputType>(
					PaymentMethodType.Manual,
					PaymentMethodType.Manual,
					OutputType.Passed),
				new Tuple<PaymentMethodType, PaymentMethodType, OutputType>(
					PaymentMethodType.DirectDebit,
					PaymentMethodType.Equalizer,
					OutputType.Failed),
				new Tuple<PaymentMethodType, PaymentMethodType, OutputType>(
					PaymentMethodType.Equalizer,
					PaymentMethodType.Equalizer,
					OutputType.Failed),
				new Tuple<PaymentMethodType, PaymentMethodType, OutputType>(
					PaymentMethodType.DirectDebitNotAvailable,
					PaymentMethodType.Manual,
					OutputType.Failed),
			};

			foreach (var testCase in paymentTypeTruthTable)
			{
				var movingHouseRulesValidationResult = new MovingHouseRulesValidationResult
				{
					Output = testCase.Item3,
					MovingHouseValidationType = MovingHouseValidationType.IsNotEqualizer
				};
				yield return new TestCaseData(testCase.Item1, testCase.Item2).Returns(movingHouseRulesValidationResult);
			}
		}
		public static IEnumerable<TestCaseData> CanResolveSingle()
		{
			var paymentTypeTruthTable = new List<Tuple<PaymentMethodType, OutputType>>()
			{
				new Tuple<PaymentMethodType, OutputType>(PaymentMethodType.Equalizer, OutputType.Failed),
				new Tuple<PaymentMethodType, OutputType>(PaymentMethodType.DirectDebitNotAvailable, OutputType.Failed),
				new Tuple<PaymentMethodType, OutputType>(PaymentMethodType.Manual, OutputType.Passed),
			};

			foreach (var testCase in paymentTypeTruthTable)
			{
				var movingHouseRulesValidationResult = new MovingHouseRulesValidationResult
				{
					Output = testCase.Item2,
					MovingHouseValidationType = MovingHouseValidationType.IsNotEqualizer
				};
				yield return new TestCaseData(testCase.Item1).Returns(movingHouseRulesValidationResult);
			}
		}

		[TestCaseSource(nameof(CanResolveDuelAccountCases))]
		public async Task<MovingHouseRulesValidationResult> CanResolveDuelAccounts(PaymentMethodType electricityPaymentMethodType, PaymentMethodType gasPaymentMethodType)
		{
			return await Context
				.WithElectricityAccount()
				.WithElectricityPaymentMethodType(electricityPaymentMethodType)
				.WithGasAccount()
				.WithGasPaymentMethodType(gasPaymentMethodType)
				.Sut
				.Resolve(Context.Query);
		}

		[TestCaseSource(nameof(CanResolveSingle))]
		public async Task<MovingHouseRulesValidationResult> CanResolveElectricity(PaymentMethodType paymentMethodType)
		{
			return await Context
				.WithElectricityAccount()
				.WithElectricityPaymentMethodType(paymentMethodType)
				.Sut
				.Resolve(Context.Query);
		}

		[TestCaseSource(nameof(CanResolveSingle))]
		public async Task<MovingHouseRulesValidationResult> CanResolveGas(PaymentMethodType paymentMethodType)
		{
			return await Context
				.WithGasAccount()
				.WithGasPaymentMethodType(paymentMethodType)
				.Sut
				.Resolve(Context.Query);
		}

		public class TestContext : UnitTestContext<IsNotEqualizerValidator>
		{
			private bool _addElectricity;
			private bool _addGas;
			private PaymentMethodType _electricityPaymentMethodType;
			private PaymentMethodType _gasPaymentMethodType;

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

			public TestContext WithElectricityPaymentMethodType(PaymentMethodType paymentMethodType)
			{
				_electricityPaymentMethodType = paymentMethodType;
				return this;
			}

			public TestContext WithGasPaymentMethodType(PaymentMethodType paymentMethodType)
			{
				_gasPaymentMethodType = paymentMethodType;
				return this;
			}

			protected override IsNotEqualizerValidator BuildSut(AutoMocker autoMocker)
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
					cfg.ElectricityAccount().Model.PaymentMethod = _electricityPaymentMethodType;
				}

				if (_addGas)
				{
					cfg.GasAccount().Model.PaymentMethod = _gasPaymentMethodType;
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