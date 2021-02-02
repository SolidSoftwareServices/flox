using S3.UiFlows.Core.Flows.Screens;

namespace S3.App.Flows.AppFlows.GreenFlow.FlowDefinitions
{


	public static class GreenFlowScreenName
	{
		public static readonly ScreenName Step0Screen = new ScreenName($"GreenFlow_{nameof(Step0Screen)}");
		public static readonly ScreenName StepAScreen = new ScreenName($"GreenFlow_{nameof(StepAScreen)}");
		public static readonly ScreenName StepBScreen = new ScreenName($"GreenFlow_{nameof(StepBScreen)}");
		public static readonly ScreenName StepCScreen = new ScreenName($"GreenFlow_{nameof(StepCScreen)}");
		public static readonly ScreenName FlowCompletedScreen = new ScreenName($"GreenFlow_{nameof(FlowCompletedScreen)}");
		public static readonly ScreenName RunBlueFlow = new ScreenName($"GreenFlow_{nameof(RunBlueFlow)}");
	}
}