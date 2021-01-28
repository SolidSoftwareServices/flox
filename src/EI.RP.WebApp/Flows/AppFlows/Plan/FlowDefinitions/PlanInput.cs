using EI.RP.UiFlows.Core.Flows.Initialization;

namespace EI.RP.WebApp.Flows.AppFlows.Plan.FlowDefinitions
{
	/// <summary>
	///Input parameter of the flow (Plan) 
	/// </summary>
	public class PlanInput : IPlanInput
	{
		//add your flow(Plan) input parameters here
		public string AccountNumber { get; set; }
	}
}