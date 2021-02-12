using S3.UiFlows.Core.Flows.Screens;

namespace S3.App.Flows.AppFlows.ContainersFlow.FlowDefinitions
{


	public static class ContainersFlowScreenName
	{
		public static readonly ScreenName Number1ContainerScreen = new ScreenName(nameof(ContainersFlowScreenName)+nameof(Number1ContainerScreen));
		public static readonly ScreenName Number2ContainerScreen = new ScreenName(nameof(ContainersFlowScreenName) + nameof(Number2ContainerScreen));
	}
}