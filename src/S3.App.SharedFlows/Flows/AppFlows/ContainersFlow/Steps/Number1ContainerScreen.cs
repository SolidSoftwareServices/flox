using System.Threading.Tasks;
using S3.App.Flows.AppFlows.ContainersFlow.FlowDefinitions;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;

namespace S3.App.Flows.AppFlows.ContainersFlow.Steps
{
	public class Number1ContainerScreen : UiFlowContainerScreen
	{
		public override ScreenName ScreenNameId =>  ContainersFlowScreenName.Number1ContainerScreen;

		protected override IScreenFlowConfigurator OnRegisterUserActions(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			return screenConfiguration.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
				.OnEventNavigatesTo(ScreenInputEvent.Step2, ContainersFlowScreenName.Number2ContainerScreen);
		}

		protected override async Task<UiFlowScreenModel> OnCreateModelAsync(IUiFlowContextData contextData)
		{
			var result = await base.OnCreateModelAsync(contextData);
			result.SetContainedFlow("GreenFlow");
			return result;
		}

		public static class ScreenInputEvent
		{
			public static readonly ScreenEvent Step2 = new ScreenEvent(nameof(Number1ContainerScreen), nameof(Step2));
		}
	}
}