using S3.UiFlows.Core.Flows;
using S3.UiFlows.Core.Flows.Screens;

namespace S3.App.AspNetCore3_1.Flows.AppFlows.ContainersFlow4.FlowDefinitions
{


	public static class ContainersFlow4ScreenName
	{
		public static readonly ScreenName Number1ContainerScreen = new ScreenName(nameof(ContainersFlow4ScreenName) + nameof(Number1ContainerScreen));
		public static readonly ScreenName Number2ContainerScreen = new ScreenName(nameof(ContainersFlow4ScreenName) + nameof(Number2ContainerScreen));
	}
}