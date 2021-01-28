using EI.RP.NavigationPrototype.Flows.AppFlows.StartFailure.FlowDefinitions;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;

namespace EI.RP.NavigationPrototype.Flows.AppFlows.StartFailure.Steps
{
	public class FailureScreen : StartFailureFlowScreen
	{
		
		
		public override ScreenName ScreenStep =>  StartFailureFlowScreenName.FailureScreen;
		public override string ViewPath => "ErrorView";

		
	}
}
