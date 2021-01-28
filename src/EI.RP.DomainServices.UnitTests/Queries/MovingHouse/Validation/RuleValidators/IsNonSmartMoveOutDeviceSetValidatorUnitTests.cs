using AutoFixture;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.DomainServices.Queries.Metering.Devices;
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
	internal class IsNonSmartMoveOutDeviceSetValidatorUnitTests : UnitTestFixture<
		IsNonSmartMoveOutDeviceSetValidatorUnitTests.TestContext,
		IsNonSmartMoveOutDeviceSetValidator>
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
					MovingHouseValidationType = MovingHouseValidationType.IsNonSmartMoveOutDeviceSet
				};


				yield return new TestCaseData(caseItem)
					.SetName(caseItem.ToString())
					.Returns(movingHouseRulesValidationResult);
			}
		}

		[TestCaseSource(nameof(CanResolveCases))]
		public async Task<MovingHouseRulesValidationResult> CanResolve(CaseModel caseModel)
		{
			return await Context
				.WithElectricityAccount(caseModel.ElectricityAccountHasSmartDevices)
				.WithGasAccount(caseModel.GasAccountHasSmartDevices)
				.Sut
				.Resolve(Context.Query);
		}

		public class TestContext : UnitTestContext<IsNonSmartMoveOutDeviceSetValidator>
		{
			private bool _addElectricity;
			private bool _isElectricitySmart;
			private bool _addGas;

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

			protected override IsNonSmartMoveOutDeviceSetValidator BuildSut(AutoMocker autoMocker)
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
					SetupDeviceQuery(domainFacade, cfg.ElectricityAccount().Model.AccountNumber, _isElectricitySmart);
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
					.Without(x => x.MovingHouseType)
					.Create();

				return base.BuildSut(autoMocker);
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