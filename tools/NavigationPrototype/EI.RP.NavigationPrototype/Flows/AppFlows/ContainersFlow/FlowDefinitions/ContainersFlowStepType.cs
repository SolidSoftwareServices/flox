using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;

namespace EI.RP.NavigationPrototype.Flows.AppFlows.ContainersFlow.FlowDefinitions
{


	public static class ContainersFlowScreenName
	{
		public static readonly ScreenName Number1ContainerScreen = new ScreenName(nameof(ContainersFlowScreenName)+nameof(Number1ContainerScreen));
		public static readonly ScreenName Number2ContainerScreen = new ScreenName(nameof(ContainersFlowScreenName) + nameof(Number2ContainerScreen));
	}
}