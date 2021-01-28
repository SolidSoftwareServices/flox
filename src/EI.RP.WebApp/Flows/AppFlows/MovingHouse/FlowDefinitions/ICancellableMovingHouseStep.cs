using EI.RP.UiFlows.Core.Flows.Screens.Models;


namespace EI.RP.WebApp.Flows.AppFlows.MovingHouse.FlowDefinitions
{
    public interface ICancellableMovingHouseStep: IUiFlowScreenModel
    {
        string InitiatedFromAccountNumber { get; set; }
    }
}