using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.CoreServices.System;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.DomainServices.Queries.Metering.Premises;
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
	internal class DiscStatusIsValidValidatorUnitTests : UnitTestFixture<
		DiscStatusIsValidValidatorUnitTests.TestContext,
		DiscStatusIsNewValidator>
	{
		public static IEnumerable<TestCaseData> CanResolveDuelAccountCases()
		{
			var discStatusIsNewTruthTable = new List<Tuple<InstallationDiscStatusType, InstallationDiscStatusType, OutputType>>()
			{
				new Tuple<InstallationDiscStatusType, InstallationDiscStatusType, OutputType>(
					InstallationDiscStatusType.AllRemovedAndDisconnected,
					InstallationDiscStatusType.Completed,
					OutputType.Failed),
				new Tuple<InstallationDiscStatusType, InstallationDiscStatusType, OutputType>(
					InstallationDiscStatusType.New,
					InstallationDiscStatusType.New,
					OutputType.Passed),
				new Tuple<InstallationDiscStatusType, InstallationDiscStatusType, OutputType>(
					InstallationDiscStatusType.DeRegistered,
					InstallationDiscStatusType.DisconnectionCarriedOut,
					OutputType.Failed),
			};

			foreach (var testCase in discStatusIsNewTruthTable)
			{
				var movingHouseRulesValidationResult = new MovingHouseRulesValidationResult
				{
					Output = testCase.Item3,
					MovingHouseValidationType = MovingHouseValidationType.DiscStatusIsNew
				};
				yield return new TestCaseData(testCase.Item1, testCase.Item2).Returns(movingHouseRulesValidationResult);
			}
		}
		public static IEnumerable<TestCaseData> CanResolveSingle()
		{
			var discStatusIsNewTruthTable = new List<Tuple<InstallationDiscStatusType, OutputType>>()
			{
				new Tuple<InstallationDiscStatusType, OutputType>(InstallationDiscStatusType.AllRemovedAndDisconnected, OutputType.Failed),
				new Tuple<InstallationDiscStatusType, OutputType>(InstallationDiscStatusType.New, OutputType.Passed),
				new Tuple<InstallationDiscStatusType, OutputType>(InstallationDiscStatusType.InstallationCarriedOut, OutputType.Failed),
			};

			foreach (var testCase in discStatusIsNewTruthTable)
			{
				var isValid = testCase.Item2;
				var movingHouseRulesValidationResult = new MovingHouseRulesValidationResult
				{
					Output = testCase.Item2,
					MovingHouseValidationType = MovingHouseValidationType.DiscStatusIsNew
				};
				yield return new TestCaseData(testCase.Item1).Returns(movingHouseRulesValidationResult);
			}
		}

		[TestCaseSource(nameof(CanResolveDuelAccountCases))]
		public async Task<MovingHouseRulesValidationResult> CanResolveDuelAccounts(InstallationDiscStatusType electricityDiscStatus, InstallationDiscStatusType gasDiscStatus)
		{
			return await Context
				.WithElectricityAccount()
				.WithElectricityDiscStatus(electricityDiscStatus)
				.WithGasAccount()
				.WithGasDiscStatus(gasDiscStatus)
				.Sut
				.Resolve(Context.Query);
		}

		[TestCaseSource(nameof(CanResolveSingle))]
		public async Task<MovingHouseRulesValidationResult> CanResolveElectricity(InstallationDiscStatusType discStatus)
		{
			return await Context
				.WithElectricityAccount()
				.WithElectricityDiscStatus(discStatus)
				.Sut
				.Resolve(Context.Query);
		}

		[TestCaseSource(nameof(CanResolveSingle))]
		public async Task<MovingHouseRulesValidationResult> CanResolveGas(InstallationDiscStatusType discStatus)
		{
			return await Context
				.WithGasAccount()
				.WithGasDiscStatus(discStatus)
				.Sut
				.Resolve(Context.Query);
		}

		public class TestContext : UnitTestContext<DiscStatusIsNewValidator>
		{
			private bool _addElectricity;
			private bool _addGas;
			private InstallationDiscStatusType _electrictyDiscStatus;
			private InstallationDiscStatusType _gasDiscStatus;

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

			public TestContext WithElectricityDiscStatus(InstallationDiscStatusType discStatus)
			{
				_electrictyDiscStatus = discStatus;
				return this;
			}

			public TestContext WithGasDiscStatus(InstallationDiscStatusType discStatus)
			{
				_gasDiscStatus = discStatus;
				return this;
			}

			protected override DiscStatusIsNewValidator BuildSut(AutoMocker autoMocker)
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
					cfg.ElectricityAccount().Premise.InstallationInfo.DiscStatus = _electrictyDiscStatus;

					var query = new PremisesQuery
					{
						Prn = cfg.ElectricityAccount().Premise.ElectricityPrn
					};

					domainFacade.QueryResolver.ExpectQuery<PremisesQuery, Premise>(query, cfg.ElectricityAccount().Premise.PremiseInfo.ToOneItemArray());
				}

				if (_addGas)
				{
					cfg.GasAccount().Premise.InstallationInfo.DiscStatus = _gasDiscStatus;

					var query = new PremisesQuery
					{
						Prn = cfg.GasAccount().Premise.GasPrn
					};

					domainFacade.QueryResolver.ExpectQuery<PremisesQuery, Premise>(query, cfg.GasAccount().Premise.PremiseInfo.ToOneItemArray());
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