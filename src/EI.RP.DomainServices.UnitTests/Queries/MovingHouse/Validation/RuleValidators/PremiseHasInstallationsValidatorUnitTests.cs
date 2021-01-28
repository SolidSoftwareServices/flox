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
	internal class PremiseHasInstallationsValidatorUnitTests : UnitTestFixture<
		PremiseHasInstallationsValidatorUnitTests.TestContext,
		PremiseHasInstallationsValidator>
	{
		public enum PremiseInstallationNullStatus {
			NotNull,
			Installation
		}

		public static IEnumerable<TestCaseData> CanResolveDuelAccountCases()
		{
			var premiseInstallationNullTruthTable = new List<Tuple<PremiseInstallationNullStatus, PremiseInstallationNullStatus, OutputType>>()
			{
				new Tuple<PremiseInstallationNullStatus, PremiseInstallationNullStatus, OutputType>(
					PremiseInstallationNullStatus.NotNull, 
					PremiseInstallationNullStatus.NotNull,
					OutputType.Passed),
				new Tuple<PremiseInstallationNullStatus, PremiseInstallationNullStatus, OutputType>(
					PremiseInstallationNullStatus.Installation,
					PremiseInstallationNullStatus.NotNull,
					OutputType.Failed),
				new Tuple<PremiseInstallationNullStatus, PremiseInstallationNullStatus, OutputType>(
					PremiseInstallationNullStatus.NotNull,
					PremiseInstallationNullStatus.Installation,
					OutputType.Failed),
				new Tuple<PremiseInstallationNullStatus, PremiseInstallationNullStatus, OutputType>(
					PremiseInstallationNullStatus.Installation,
					PremiseInstallationNullStatus.Installation,
					OutputType.Failed),
			};

			foreach (var testCase in premiseInstallationNullTruthTable)
			{
				var movingHouseRulesValidationResult = new MovingHouseRulesValidationResult
				{
					Output = testCase.Item3,
					MovingHouseValidationType = MovingHouseValidationType.HasPremiseInstallations
				};
				yield return new TestCaseData(testCase.Item1, testCase.Item2).Returns(movingHouseRulesValidationResult);
			}
		}
		public static IEnumerable<TestCaseData> CanResolveSingle()
		{
			var permiseNullTruthTable = new List<Tuple<PremiseInstallationNullStatus, OutputType>>()
			{
				new Tuple<PremiseInstallationNullStatus,  OutputType>(PremiseInstallationNullStatus.Installation, OutputType.Failed),
				new Tuple<PremiseInstallationNullStatus,  OutputType>(PremiseInstallationNullStatus.NotNull, OutputType.Passed),
			};

			foreach (var testCase in permiseNullTruthTable)
			{
				var movingHouseRulesValidationResult = new MovingHouseRulesValidationResult
				{
					Output = testCase.Item2,
					MovingHouseValidationType = MovingHouseValidationType.HasPremiseInstallations
				};
				yield return new TestCaseData(testCase.Item1).Returns(movingHouseRulesValidationResult);
			}
		}

		[TestCaseSource(nameof(CanResolveDuelAccountCases))]
		public async Task<MovingHouseRulesValidationResult> CanResolveDuelAccounts(PremiseInstallationNullStatus elecPremiseNullStatus, PremiseInstallationNullStatus gasPremiseNullStatus)
		{
			return await Context
				.WithElectricityAccount()
				.WithElectricityPremiseNullStatus(elecPremiseNullStatus)
				.WithGasAccount()
				.WithGasPremiseNullStatus(gasPremiseNullStatus)
				.Sut
				.Resolve(Context.Query);
		}

		[TestCaseSource(nameof(CanResolveSingle))]
		public async Task<MovingHouseRulesValidationResult> CanResolveElectricity(PremiseInstallationNullStatus discStatus)
		{
			return await Context
				.WithElectricityAccount()
				.WithElectricityPremiseNullStatus(discStatus)
				.Sut
				.Resolve(Context.Query);
		}

		[TestCaseSource(nameof(CanResolveSingle))]
		public async Task<MovingHouseRulesValidationResult> CanResolveGas(PremiseInstallationNullStatus discStatus)
		{
			return await Context
				.WithGasAccount()
				.WithGasPremiseNullStatus(discStatus)
				.Sut
				.Resolve(Context.Query);
		}

		public class TestContext : UnitTestContext<PremiseHasInstallationsValidator>
		{
			private bool _addElectricity;
			private bool _addGas;
			private PremiseInstallationNullStatus _electricityPremiseNullStatus;
			private PremiseInstallationNullStatus _gasPremiseNullStatus;

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

			public TestContext WithElectricityPremiseNullStatus(PremiseInstallationNullStatus premiseStatus)
			{
				_electricityPremiseNullStatus = premiseStatus;
				return this;
			}

			public TestContext WithGasPremiseNullStatus(PremiseInstallationNullStatus premiseStatus)
			{
				_gasPremiseNullStatus = premiseStatus;
				return this;
			}

			protected override PremiseHasInstallationsValidator BuildSut(AutoMocker autoMocker)
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
					var query = new PremisesQuery
					{
						PremiseId = cfg.ElectricityAccount().Premise.ElectricityPrn.ToString()
					};

					switch (_electricityPremiseNullStatus)
					{
						case PremiseInstallationNullStatus.NotNull:
							break;
						case PremiseInstallationNullStatus.Installation:
							cfg.ElectricityAccount().Premise.PremiseInfo.Installations = null;
							break;
					}

					domainFacade.QueryResolver.ExpectQuery<PremisesQuery, Premise>(query, cfg.ElectricityAccount().Premise.PremiseInfo.ToOneItemArray());
				}

				if (_addGas)
				{
					var query = new PremisesQuery
					{
						PremiseId = cfg.GasAccount().Premise.GasPrn.ToString()
					};

					switch (_gasPremiseNullStatus)
					{
						case PremiseInstallationNullStatus.NotNull:
							break;
						case PremiseInstallationNullStatus.Installation:
							cfg.GasAccount().Premise.PremiseInfo.Installations = null;
							break;
					}

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