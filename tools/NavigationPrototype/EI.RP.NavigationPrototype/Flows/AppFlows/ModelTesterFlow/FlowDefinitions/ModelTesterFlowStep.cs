using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;

namespace EI.RP.NavigationPrototype.Flows.AppFlows.ModelTesterFlow.FlowDefinitions
{


	public static class ModelTesterFlowStep
	{
		public static ScreenName InputScreen = new ScreenName(nameof(InputScreen));

		
		public static ScreenName FlowCompletedScreen = new ScreenName(nameof(FlowCompletedScreen));
	}
}