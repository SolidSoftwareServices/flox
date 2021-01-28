using System.Threading.Tasks;
using EI.RP.NavigationPrototype.Flows.AppFlows.ContainersFlow.FlowDefinitions;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;


namespace EI.RP.NavigationPrototype.Flows.AppFlows.ContainersFlow.Steps
{
	public class Number1ContainerScreen : ContainersFlowScreen
	{
		public override ScreenName ScreenStep =>  ContainersFlowScreenName.Number1ContainerScreen;

		protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			return screenConfiguration.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
				.OnEventNavigatesTo(StepEvent.Step2, ContainersFlowScreenName.Number2ContainerScreen);
		}

		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			var result = await base.OnCreateStepDataAsync(contextData);
			result.SetContainedFlow(SampleAppFlowType.GreenFlow);
			return result;
		}

		public static class StepEvent
		{
			public static readonly ScreenEvent Step2 = new ScreenEvent(nameof(Number1ContainerScreen), nameof(Step2));
		}
	}
}