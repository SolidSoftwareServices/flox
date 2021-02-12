using S3.App.Flows.AppFlows.ContainersFlow2.FlowDefinitions;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows.Screens;

namespace S3.App.Flows.AppFlows.ContainersFlow2.Steps
{
	public class Number2ContainerScreen : UiFlowContainerScreen
	{
		public override ScreenName ScreenNameId =>  ContainersFlow2ScreenName.Number2ContainerScreen;
		protected override IScreenFlowConfigurator OnRegisterUserActions(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			return screenConfiguration.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
				.OnEventNavigatesTo(ScreenInputEvent.Step1, ContainersFlow2ScreenName.Number1ContainerScreen);
		}

		public static class ScreenInputEvent
		{
			public static readonly ScreenEvent Step1 = new ScreenEvent(nameof(Number2ContainerScreen),nameof(Step1));
		}
	}
}