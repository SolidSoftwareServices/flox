using System.Threading.Tasks;
using EI.RP.NavigationPrototype.Flows.AppFlows.ContainersFlow2.FlowDefinitions;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;


namespace EI.RP.NavigationPrototype.Flows.AppFlows.ContainersFlow2.Steps
{
	public class Number1ContainerScreen : ContainersFlow2Screen
	{
		public override ScreenName ScreenStep =>  ContainersFlow2ScreenName.Number1ContainerScreen;

		protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			return screenConfiguration.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
				.OnEventNavigatesTo(StepEvent.Step2, ContainersFlow2ScreenName.Number2ContainerScreen);
		}

		

		public static class StepEvent
		{
			public static readonly ScreenEvent Step2 = new ScreenEvent(nameof(Number1ContainerScreen),nameof(Step2));
		}
	}
}