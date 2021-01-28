using EI.RP.UiFlows.Core.Flows.Screens.Models;


namespace EI.RP.WebApp.Flows.AppFlows.MovingHouse.FlowDefinitions
{
    public interface IForwardingAddressPartMovingHouse : IUiFlowScreenModel
    {
        ROIAddressInfo ROIAddress { get; set; }
        NIAddressInfo NIAddress { get; set; }
        POBoxFields PostalAddress { get; set; }
    }
}