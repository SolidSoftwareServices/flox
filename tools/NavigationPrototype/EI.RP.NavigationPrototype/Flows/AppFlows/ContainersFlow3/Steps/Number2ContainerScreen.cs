using System;
using System.Threading.Tasks;
using EI.RP.NavigationPrototype.Flows.AppFlows.ContainersFlow3.FlowDefinitions;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;


namespace EI.RP.NavigationPrototype.Flows.AppFlows.ContainersFlow3.Steps
{
	public class Number2ContainerScreen : ContainersFlow3Screen
	{
		public override ScreenName ScreenStep =>  ContainersFlow3ScreenName.Number2ContainerScreen;
		protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			return screenConfiguration.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
				.OnEventNavigatesTo(StepEvent.Step1, ContainersFlow3ScreenName.Number1ContainerScreen);
		}



		public static class StepEvent
		{
			public static readonly ScreenEvent Step1 = new ScreenEvent(nameof(Number2ContainerScreen),nameof(Step1));
		}


		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
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