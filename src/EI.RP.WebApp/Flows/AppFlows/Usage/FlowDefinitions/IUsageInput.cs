using EI.RP.UiFlows.Core.Flows.Initialization;

namespace EI.RP.WebApp.Flows.AppFlows.Usage.FlowDefinitions
{
    public interface IUsageInput : IFlowInputContract
    {
        string AccountNumber { get; set; }
    }

    public class UsageInput : IUsageInput
    {
        public string AccountNumber { get; set; }
    }
}
