using EI.RP.NavigationPrototype.Flows.AppFlows.ContainersFlow4.FlowDefinitions;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;

namespace EI.RP.NavigationPrototype.Flows.AppFlows.ContainersFlow4.Steps
{
	public class Number1ContainerScreen : ContainersFlow4Screen
	{
		public override ScreenName ScreenStep =>  ContainersFlow4ScreenName.Number1ContainerScreen;

		protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			return screenConfiguration.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
				.OnEventNavigatesTo(StepEvent.Step2, ContainersFlow4ScreenName.Number2ContainerScreen);
		}

		

		public static class StepEvent
		{
			public static readonly ScreenEvent Step2 = new ScreenEvent(nameof(Number1ContainerScreen),nameof(Step2));
		}
	}
}