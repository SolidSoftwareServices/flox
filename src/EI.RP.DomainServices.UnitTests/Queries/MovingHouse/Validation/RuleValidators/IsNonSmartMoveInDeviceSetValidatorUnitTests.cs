using AutoFixture;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.DomainServices.Queries.Metering.Devices;
using EI.RP.DomainServices.Queries.Metering.Premises;
using EI.RP.DomainServices.Queries.MovingHouse.Validation;
using EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators;
using EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators.OutputResolvers;
using EI.RP.DomainServices.UnitTests.Infrastructure.RepositoryMocks;
using EI.RP.TestServices;
using Moq;
using NUnit.Framework;
using Moq.AutoMock;


namespace EI.RP.DomainServices.UnitTests.Queries.MovingHouse.Validation.RuleValidators
{
	[TestFixture]
	internal class IsNonSmartMoveInDeviceSetValidatorUnitTests : UnitTestFixture<
		IsNonSmartMoveInDeviceSetValidatorUnitTests.TestContext,
		IsNonSmartMoveInDeviceSetValidator>
	{
		internal class CaseModel
		{
			public bool? ElectricityAccountHasSmartDevices { get; set; }
			public bool? GasAccountHasSmartDevices { get; set; }
			public OutputType Output { get; set; }
			public override string ToString()
			{
				return $"{nameof(ElectricityAccountHasSmartDevices)}: {ElectricityAccountHasSmartDevices} {nameof(GasAccountHasSmartDevices)}: {GasAccountHasSmartDevices}, {nameof(Output)}: {Output}";
			}
		}

		public static IEnumerable<TestCaseData> CanResolveCases()
		{
			var cases = new CaseModel[]
			{
				new CaseModel{ ElectricityAccountHasSmartDevices = null, GasAccountHasSmartDevices = null  , Output = OutputType.NotExecuted},
				new CaseModel{ ElectricityAccountHasSmartDevices = false, GasAccountHasSmartDevices = null , Output = OutputType.Passed},
				new CaseModel{ ElectricityAccountHasSmartDevices = true, GasAccountHasSmartDevices = null  , Output = OutputType.Failed},
				new CaseModel{ ElectricityAccountHasSmartDevices = false, GasAccountHasSmartDevices = false, Output = OutputType.Passed},
				new CaseModel{ ElectricityAccountHasSmartDevices = true, GasAccountHasSmartDevices = false , Output = OutputType.Failed}
			};

			foreach (var caseItem in cases)
			{
				var movingHouseRulesValidationResult = new MovingHouseRulesValidationResult
				{
					Output = caseItem.Output,
					MovingHouseValidationType = MovingHouseValidationType.IsNonSmartMoveInDeviceSet
				};


				yield return new TestCaseData(caseItem)
					.SetName(caseItem.ToString())
					.Returns(movingHouseRulesValidationResult);
			}
		}

		[TestCaseSource(nameof(CanResolveCases))]
		public async Task<MovingHouseRulesValidationResult> CanResolve(CaseModel caseModel)
		{
			ElectricityPointReferenceNumber electricityMprn = null;
			var fixture = new Fixture().CustomizeDomainTypeBuilders();

			if (caseModel.ElectricityAccountHasSmartDevices.HasValue)
			{
				electricityMprn = fixture.Create<ElectricityPointReferenceNumber>();
			}

			return await Context
				.WithElectricityAccount(caseModel.ElectricityAccountHasSmartDevices)
				.WithNewMPRN(electricityMprn)
				.WithGasAccount(caseModel.GasAccountHasSmartDevices)
				.Sut
				.Resolve(Context.Query);
		}

		public class TestContext : UnitTestContext<IsNonSmartMoveInDeviceSetValidator>
		{
			private bool _addElectricity;
			private bool _isElectricitySmart;
			private bool _addGas;
			private string _mprn;

			public MovingHouseValidationQuery Query { get; private set; }

			public TestContext WithElectricityAccount(bool? electricityAccountHasSmartDevices)
			{
				_addElectricity = electricityAccountHasSmartDevices.HasValue;
				_isElectricitySmart = electricityAccountHasSmartDevices.HasValue && electricityAccountHasSmartDevices.Value;
				return this;
			}

			public TestContext WithGasAccount(bool? gasAccountHasSmartDevices)
			{
				_addGas = gasAccountHasSmartDevices.HasValue;
				return this;
			}

			public TestContext WithNewMPRN(ElectricityPointReferenceNumber mprn)
			{
				if (mprn != null)
				{
					_mprn = mprn.ToString();
				}

				return this;
			}

			protected override IsNonSmartMoveInDeviceSetValidator BuildSut(AutoMocker autoMocker)
			{
				var domainFacade = new DomainFacade();
				autoMocker.Use<ICompositeRuleValidatorOutputTypeResolver>(new CompositeRuleValidatorOutputResolver());
				domainFacade.SetUpMocker(autoMocker);
				var cfg = new AppUserConfigurator(domainFacade);
				if (_addElectricity)
				{
					cfg.AddElectricityAccount(configureDefaultDevice: true);
				}

				if (_addGas)
				{
					cfg.AddGasAccount(configureDefaultDevice: true);
				}

				cfg.Execute();

				if (_addElectricity)
				{
					SetupPremisesQuery(domainFacade, _isElectricitySmart);
				}

				if (_addGas)
				{
					SetupDeviceQuery(domainFacade, cfg.GasAccount().Model.AccountNumber, false);
				}

				var electricityAccount = cfg.ElectricityAccount()?.Model;
				var gasAccount = cfg.GasAccount()?.Model;

				Query = Fixture.Build<MovingHouseValidationQuery>()
					.With(x => x.ElectricityAccountNumber, electricityAccount?.AccountNumber)
					.With(x => x.GasAccountNumber, gasAccount?.AccountNumber)
					.With(x => x.NewMPRN, _mprn)
					.Without(x => x.MovingHouseType)
					.Create();

				return base.BuildSut(autoMocker);
			}

			private void SetupPremisesQuery(DomainFacade domainFacade, bool isSmart)
			{
				domainFacade.QueryResolver.Current
					.Setup(x => x.FetchAsync<PremisesQuery, Premise>(new PremisesQuery
					{
						Prn =  new ElectricityPointReferenceNumber(_mprn)
					}, It.IsAny<bool>()))
					.Returns(
						Task.FromResult(
							new List<Premise>()
							{
								new Premise()
								{
									Installations = new List<InstallationInfo>()
									{
										new InstallationInfo()
										{
											Devices = new List<DeviceInfo>()
											{
												new DeviceInfo
												{
													IsSmart = isSmart
												}
											}
										}

									}
								}
							}.AsEnumerable()
						)
					);
			}

			private void SetupDeviceQuery(DomainFacade domainFacade, string accountNumber, bool isSmart)
			{
				domainFacade.QueryResolver.Current
					.Setup(x => x.FetchAsync<DevicesQuery, DeviceInfo>(new DevicesQuery
					{
						AccountNumber = accountNumber,
					}, It.IsAny<bool>()))
					.Returns(
						Task.FromResult(
							new List<DeviceInfo>()
							{
								new DeviceInfo
								{
									IsSmart = isSmart
								}

							}.AsEnumerable()
						)
					);
			}
		}
	}
}