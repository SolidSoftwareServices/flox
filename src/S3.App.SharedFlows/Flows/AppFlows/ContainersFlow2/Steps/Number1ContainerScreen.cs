using S3.App.Flows.AppFlows.ContainersFlow2.FlowDefinitions;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows.Screens;

namespace S3.App.Flows.AppFlows.ContainersFlow2.Steps
{
	public class Number1ContainerScreen : ContainersFlow2Screen
	{
		public override ScreenName ScreenStep =>  ContainersFlow2ScreenName.Number1ContainerScreen;

		protected override IScreenFlowConfigurator OnRegisterUserActions(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			return screenConfiguration.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
				.OnEventNavigatesTo(ScreenInputEvent.Step2, ContainersFlow2ScreenName.Number2ContainerScreen);
		}

		

		public static class ScreenInputEvent
		{
			public static readonly ScreenEvent Step2 = new ScreenEvent(nameof(Number1ContainerScreen),nameof(Step2));
		}
	}
}