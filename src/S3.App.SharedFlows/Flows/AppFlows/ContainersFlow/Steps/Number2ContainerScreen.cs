using System.Collections.Generic;
using System.Threading.Tasks;
using S3.App.Flows.AppFlows.ContainersFlow.FlowDefinitions;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;

namespace S3.App.Flows.AppFlows.ContainersFlow.Steps
{
	public class Number2ContainerScreen : UiFlowContainerScreen
	{
		public override ScreenName ScreenNameId =>  ContainersFlowScreenName.Number2ContainerScreen;
		protected override IScreenFlowConfigurator OnRegisterUserActions(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			return screenConfiguration.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
				.OnEventNavigatesTo(ScreenInputEvent.Step1, ContainersFlowScreenName.Number1ContainerScreen);
		}

		public static class ScreenInputEvent
		{
			public static readonly ScreenEvent Step1 = new ScreenEvent(nameof(Number2ContainerScreen), nameof(Step1));
		}

		protected override async Task<UiFlowScreenModel> OnCreateModelAsync(IUiFlowContextData contextData)
		{
			var model = new Number2ContainerScreenModel();
			await SetTitle(contextData, model);
			return model;
		}
		protected override async Task<UiFlowScreenModel> OnRefreshModelAsync(IUiFlowContextData contextData, UiFlowScreenModel originalScreenModel,
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