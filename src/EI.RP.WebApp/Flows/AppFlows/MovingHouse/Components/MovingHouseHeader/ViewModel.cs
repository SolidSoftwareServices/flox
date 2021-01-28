

using EI.RP.UiFlows.Mvc.Components;

namespace EI.RP.WebApp.Flows.AppFlows.MovingHouse.Components.MovingHouseHeader
{
	public class ViewModel:FlowComponentViewModel
	{
		public string HeaderText { get; set; }
		
		public string PrnText { get; set; }
		public int CurrentStepNumber { get; set; }
		public bool ShowProcess { get; set; } = true;
	}
}