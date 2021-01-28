using S3.App.AspNetCore3_1.Flows.AppFlows.ContainersFlow4.FlowDefinitions;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.Infrastructure.DataSources;
using S3.UiFlows.Core.Flows;
using S3.UiFlows.Core.Flows.Screens;

namespace S3.App.AspNetCore3_1.Flows.AppFlows.ContainersFlow4.Steps
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