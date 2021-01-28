using EI.RP.UiFlows.Core.Flows.Screens.Models;

namespace EI.RP.WebApp.Flows.AppFlows.Plan.Components.PlanDetails
{
	/// <summary>
	///     This class defines the the component PlanDetails input parameter
	/// </summary>
	public class InputModel
	{
		public string AccountNumber { get; set; }

		public UiFlowScreenModel FlowScreenModel { get; set; }
	}
}