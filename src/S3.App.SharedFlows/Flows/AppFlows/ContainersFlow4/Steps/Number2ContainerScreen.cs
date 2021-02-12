using S3.App.Flows.AppFlows.ContainersFlow4.FlowDefinitions;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows.Screens;

namespace S3.App.Flows.AppFlows.ContainersFlow4.Steps
{
	public class Number2ContainerScreen : ContainersFlow4Screen
	{
		public override ScreenName ScreenStep =>  ContainersFlow4ScreenName.Number2ContainerScreen;
		protected override IScreenFlowConfigurator OnRegisterUserActions(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			return screenConfiguration.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
				.OnEventNavigatesTo(ScreenInputEvent.Step1, ContainersFlow4ScreenName.Number1ContainerScreen);
		}

		public static class ScreenInputEvent
		{
			public static readonly ScreenEvent Step1 = new ScreenEvent(nameof(Number2ContainerScreen),nameof(Step1));
		}
	}
}