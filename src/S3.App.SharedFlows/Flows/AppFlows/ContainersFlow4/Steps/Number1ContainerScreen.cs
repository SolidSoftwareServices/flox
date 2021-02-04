using S3.App.Flows.AppFlows.ContainersFlow4.FlowDefinitions;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows.Screens;

namespace S3.App.Flows.AppFlows.ContainersFlow4.Steps
{
	public class Number1ContainerScreen : ContainersFlow4Screen
	{
		public override ScreenName ScreenStep =>  ContainersFlow4ScreenName.Number1ContainerScreen;

		protected override IScreenFlowConfigurator OnRegisterUserActions(
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