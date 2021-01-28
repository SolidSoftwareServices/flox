using S3.UiFlows.Core.Flows.Screens;

namespace S3.App.AspNetCore3_1.Flows.AppFlows.BlueFlow.FlowDefinitions
{


	public static class BlueFlowScreenName
	{
		public static ScreenName Step0Screen = new ScreenName("BlueFlow_Step0Screen");

		public static ScreenName FillDataStep = new ScreenName("BlueFlow_FillDataStep");

		public static ScreenName FillDataStep_StepAScreen = new ScreenName("BlueFlow_StepAScreen");
		public static ScreenName FillDataStep_StepBScreen = new ScreenName("BlueFlow_StepBScreen");


		public static ScreenName StepCScreen = new ScreenName("BlueFlow_StepCScreen");
		public static ScreenName FlowCompletedScreen = new ScreenName("BlueFlow_FlowCompletedScreen");
		public static ScreenName EndAndReturnToCaller = new ScreenName(nameof(EndAndReturnToCaller));
		public static ScreenName ErrorScreen = new ScreenName(nameof(ErrorScreen));
	}
}