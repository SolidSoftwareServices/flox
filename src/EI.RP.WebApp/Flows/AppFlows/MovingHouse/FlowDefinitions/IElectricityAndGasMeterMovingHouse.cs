using System;
using System.Collections.Generic;
using Ei.Rp.DomainModels.Metering;
using EI.RP.UiFlows.Core.Flows.Screens.Models;


namespace EI.RP.WebApp.Flows.AppFlows.MovingHouse.FlowDefinitions
{
    public interface IElectricityAndGasMeterMovingHouse : IUiFlowScreenModel
    {
        InputFields UserMeterInputFields { get; set; }
        IEnumerable<DeviceRegisterUiModel> GasDevicesMeterReadings { get; set; }
        IEnumerable<DeviceRegisterUiModel> ElectricityDevicesMeterReadings { get; set; }
    }
}