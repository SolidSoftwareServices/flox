using S3.UiFlows.Core.Flows;
using S3.UiFlows.Core.Flows.Screens;

namespace S3.App.AspNetCore3_1.Flows.AppFlows.ModelTesterFlow.FlowDefinitions
{


	public static class ModelTesterFlowStep
	{
		public static ScreenName InputScreen = new ScreenName(nameof(InputScreen));

		
		public static ScreenName FlowCompletedScreen = new ScreenName(nameof(FlowCompletedScreen));
	}
}