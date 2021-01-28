using EI.RP.NavigationPrototype.Flows.AppFlows.ContainersFlow4.FlowDefinitions;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;

namespace EI.RP.NavigationPrototype.Flows.AppFlows.ContainersFlow4.Steps
{
	public class Number2ContainerScreen : ContainersFlow4Screen
	{
		public override ScreenName ScreenStep =>  ContainersFlow4ScreenName.Number2ContainerScreen;
		protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			return screenConfiguration.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
				.OnEventNavigatesTo(StepEvent.Step1, ContainersFlow4ScreenName.Number1ContainerScreen);
		}

		public static class StepEvent
		{
			public static readonly ScreenEvent Step1 = new ScreenEvent(nameof(Number2ContainerScreen),nameof(Step1));
		}
	}
}