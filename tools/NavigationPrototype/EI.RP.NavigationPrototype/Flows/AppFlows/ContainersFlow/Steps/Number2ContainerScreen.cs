using System.Collections.Generic;
using System.Threading.Tasks;
using EI.RP.NavigationPrototype.Flows.AppFlows.ContainersFlow.FlowDefinitions;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;

namespace EI.RP.NavigationPrototype.Flows.AppFlows.ContainersFlow.Steps
{
	public class Number2ContainerScreen : ContainersFlowScreen
	{
		public override ScreenName ScreenStep =>  ContainersFlowScreenName.Number2ContainerScreen;
		protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			return screenConfiguration.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
				.OnEventNavigatesTo(StepEvent.Step1, ContainersFlowScreenName.Number1ContainerScreen);
		}

		public static class StepEvent
		{
			public static readonly ScreenEvent Step1 = new ScreenEvent(nameof(Number2ContainerScreen), nameof(Step1));
		}

		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			var model = new Number2ContainerScreenModel();
			await SetTitle(contextData, model);
			return model;
		}
		protected override async Task<UiFlowScreenModel> OnRefreshStepDataAsync(IUiFlowContextData contextData, UiFlowScreenModel originalScreenModel,
			IDictionary<string, object> stepViewCustomizations = null)
		{
			await SetTitle(contextData, (Number2ContainerScreenModel)originalScreenModel);
			return originalScreenModel;
		}
		private  async Task SetTitle(IUiFlowContextData contextData, Number2ContainerScreenModel model)
		{
			model.NextContainedFlowScreenTitle = (await contextData.GetCurrentStepContainedData(ContainedFlowQueryOption.Immediate))?.ScreenTitle;
			model.ScreenTitle = (await contextData.GetCurrentStepContainedData(ContainedFlowQueryOption.Last))?.ScreenTitle;
		}

		
		public class Number2ContainerScreenModel : UiFlowScreenModel
		{
			public Number2ContainerScreenModel() : base(isContainer:true) { }

			public override bool IsValidFor(ScreenName screenStep)
			{
				return screenStep == ContainersFlowScreenName.Number2ContainerScreen;
			}

			public string NextContainedFlowScreenTitle { get; set; }
		}
		
	}
}