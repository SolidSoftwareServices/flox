using EI.RP.UiFlows.Core.Flows.Initialization;

namespace EI.RP.WebApp.Flows.AppFlows.Plan.FlowDefinitions
{
    /// <summary>
    ///Input parameter of the flow (Plan) 
    /// </summary>
    public interface IPlanInput : IFlowInputContract
    {
	    string AccountNumber { get; set; }
	}
}