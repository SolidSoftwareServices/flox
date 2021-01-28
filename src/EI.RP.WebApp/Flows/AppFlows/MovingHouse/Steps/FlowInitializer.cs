using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Http.Session;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Queries.Metering.Devices;
using EI.RP.DomainServices.Queries.MovingHouse.Validation;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Initialization;
using EI.RP.UiFlows.Core.Flows.Initialization.Models;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.FlowDefinitions;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps.Extensions;

namespace EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps
{
	public class FlowInitializer : UiFlowInitializationStep<ResidentialPortalFlowType, FlowInitializer.RootScreenModel>
    {
        private readonly IUserSessionProvider _userSessionProvider;
        private readonly IDomainQueryResolver _queryResolver;
	    public override ResidentialPortalFlowType InitializerOfFlowType => ResidentialPortalFlowType.MovingHouse;

		public FlowInitializer(IUserSessionProvider userSessionProvider, IDomainQueryResolver queryResolver) 
        {
            _userSessionProvider = userSessionProvider;
            _queryResolver = queryResolver;
        }

        public override bool Authorize()
        {
            return !_userSessionProvider.IsAnonymous();
        }

        public override IScreenFlowConfigurator OnDefiningAdditionalInitialStateTransitions(
            IScreenFlowConfigurator preStartCfg, 
			UiFlowContextData contextData)
        {
			preStartCfg.OnEventNavigatesTo(ScreenEvent.ErrorOccurred, MovingHouseStep.MovingHouseNotPresent);
			preStartCfg.OnEventNavigatesToAsync(
				ScreenEvent.Start, 
				MovingHouseStep.Step0OperationSelection, 
				() => HasMovingHouseValidationError(contextData).ContinueWith(x => !x.Result),
				"Rules are valid");

			preStartCfg.OnEventNavigatesToAsync(
				ScreenEvent.Start,
				MovingHouseStep.ShowMovingHouseValidationError,
				() => HasMovingHouseValidationError(contextData),
				"Rules validation failed");
			return preStartCfg;
		}

		async Task<bool> HasMovingHouseValidationError(IUiFlowContextData contextData)
		{
			var rootData = contextData.GetStepData<FlowInitializer.RootScreenModel>(ScreenName.PreStart);
			var validation = await _queryResolver.GetMovingHouseValidationResult(
				rootData.ElectricityAccountNumber,
				rootData.GasAccountNumber,
				null);

			return validation.Any(x => x.Output == OutputType.Failed);
		}

		protected override async Task<RootScreenModel> OnBuildStartData(UiFlowContextData contextData,
	        RootScreenModel data)
        {
	        data.UserMeterInputFields = new InputFields();
			

			var accountInfos = await _queryResolver.GetFlowAccounts(data);
            data.ElectricityAccountNumber = accountInfos.SingleOrDefault(x => x.ClientAccountType == ClientAccountType.Electricity)?.AccountNumber;

            data.GasAccountNumber = accountInfos.SingleOrDefault(x => x.ClientAccountType == ClientAccountType.Gas)?.AccountNumber;
            await Task.WhenAll(MapElectricityData(), MapGasData());
            ThrowIfNotValid();
            return data;

            void ThrowIfNotValid()
            {
                if (data.ElectricityAccountNumber == null && data.GasAccountNumber == null || data.InitiatedFromAccountNumber == null)
                {
                    throw new InvalidProgramException("You have found a unicorn");
                }
            }
            async Task MapGasData()
            {
                if (data.GasAccountNumber != null)
                {
                    IEnumerable<DeviceRegisterUiModel> gasDevicesMeterReadingList = new DeviceRegisterUiModel[0];

                    var deviceInfoTask = _queryResolver.GetDevicesByAccount(data.GasAccountNumber);
                    var gasAccountTask = _queryResolver.GetAccountInfoByAccountNumber(data.GasAccountNumber);
                    var gasDevices = await deviceInfoTask;

                    gasDevicesMeterReadingList = gasDevices.Last().Registers
                        .Where(x => x.RegisterId == MeterReadingRegisterType.ActiveEnergyRegisterType)
                        .Select(x => x.MeterNumber).Distinct().Select(meter => new DeviceRegisterUiModel
                            {MeterType = MeterType.Gas, MeterNumber = meter, MeterUnit = MeterUnit.M3});

                    data.GasDevicesMeterReadings = gasDevicesMeterReadingList.ToArray();

                    if (data.GasDevicesMeterReadings.Any())
                    {
                        data.UserMeterInputFields.GasDevicesFieldRequired = true;
                    }
                    data.UserMeterInputFields.Gprn = (await gasAccountTask)?.PointReferenceNumber?.ToString();
                }
            }
            async Task MapElectricityData()
            {
                var electricityDevicesMeterReadingList = new List<DeviceRegisterUiModel>();

                if (data.ElectricityAccountNumber != null)
                {
                    var getAccountTask = _queryResolver.GetAccountInfoByAccountNumber(data.ElectricityAccountNumber);
                    var elecDevices = (await _queryResolver.GetDevicesByAccount(data.ElectricityAccountNumber)).ToArray();

                    var registers = elecDevices.SelectMany(x=>x.Registers).ToArray();
                    foreach (var register in registers)
                    {
	                    var electricityDevicesMeterReading = new DeviceRegisterUiModel(register);
	                    electricityDevicesMeterReadingList.Add(electricityDevicesMeterReading);

                        if (register.MeterType == MeterType.ElectricityNightStorageHeater)
                        {
	                        data.UserMeterInputFields.ElectricityNightStorageHeaterDevicesFieldRequired = true;
                        }

                        if (register.MeterType == MeterType.Electricity24h)
                        {
	                        data.UserMeterInputFields.Electricity24HrsDevicesFieldRequired = true;
                        }

                        if (register.MeterType == MeterType.ElectricityDay)
                        {
	                        data.UserMeterInputFields.ElectricityDayDevicesFieldRequired = true;
						}

                        if (register.MeterType == MeterType.ElectricityNight)
                        {
	                        data.UserMeterInputFields.ElectricityNightDevicesFieldRequired = true;
                        }
					}

                    data.ElectricityDevicesMeterReadings = electricityDevicesMeterReadingList.ToArray();
                    data.UserMeterInputFields.Mprn = (await getAccountTask)?.PointReferenceNumber?.ToString();
                }
            }
        }

        public class RootScreenModel : InitialFlowScreenModel, IMovingHouseInput
        {
            public string ElectricityAccountNumber { get; set; }
            public string GasAccountNumber { get; set; }
            public InputFields UserMeterInputFields { get; set; }
            public IEnumerable<DeviceRegisterUiModel> ElectricityDevicesMeterReadings { get; set; }
            public IEnumerable<DeviceRegisterUiModel> GasDevicesMeterReadings { get; set; } 
            /// <summary>
            /// The selected account when initiated the process
            /// </summary>
            public string InitiatedFromAccountNumber { get; set; }

            public string SecondaryAccountNumber() => InitiatedFromAccountNumber == ElectricityAccountNumber
	            ? GasAccountNumber
	            : ElectricityAccountNumber;

            public bool HasSecondaryAccount() => !string.IsNullOrEmpty(ElectricityAccountNumber) &&
                                          !string.IsNullOrEmpty(GasAccountNumber);

        }
    }
}