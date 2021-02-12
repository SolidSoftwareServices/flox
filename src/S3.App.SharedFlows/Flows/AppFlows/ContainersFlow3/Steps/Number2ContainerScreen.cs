using System;
using System.Threading.Tasks;
using S3.App.Flows.AppFlows.ContainersFlow3.FlowDefinitions;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;

namespace S3.App.Flows.AppFlows.ContainersFlow3.Steps
{
	public class Number2ContainerScreen : ContainersFlow3Screen
	{
		public override ScreenName ScreenStep =>  ContainersFlow3ScreenName.Number2ContainerScreen;
		protected override IScreenFlowConfigurator OnRegisterUserActions(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			return screenConfiguration.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
				.OnEventNavigatesTo(ScreenInputEvent.Step1, ContainersFlow3ScreenName.Number1ContainerScreen);
		}



		public static class ScreenInputEvent
		{
			public static readonly ScreenEvent Step1 = new ScreenEvent(nameof(Number2ContainerScreen),nameof(Step1));
		}


		protected override async Task<UiFlowScreenModel> OnCreateModelAsync(IUiFlowContextData contextData)
		{
			var a = contextData.GetStepData<Number1ContainerScreen.StepData>(ContainersFlow3ScreenName.Number1ContainerScreen);
			return new ScreenModel()
			{
				BlueFlowSelectedInput = a.CalledFlowResult,
				Step1Loaded=a.InitialLoad
			};
		}

		public class ScreenModel : UiFlowScreenModel
		{
			public string BlueFlowSelectedInput { get; set; }
			public DateTime Step1Loaded { get; set; }
		}
	}
}