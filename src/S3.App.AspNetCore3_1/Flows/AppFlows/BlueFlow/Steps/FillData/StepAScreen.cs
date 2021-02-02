using System.Threading.Tasks;
using S3.App.AspNetCore3_1.Flows.AppFlows.BlueFlow.FlowDefinitions;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;


namespace S3.App.AspNetCore3_1.Flows.AppFlows.BlueFlow.Steps.FillData
{
	public class StepAScreen : BlueFlowScreen
	{

		public static class StepEvent
		{
			public static readonly ScreenEvent Previous = new ScreenEvent(nameof(StepAScreen),"Previous");
			public static readonly ScreenEvent Next = new ScreenEvent(nameof(StepAScreen), "Next");
			
			public static readonly ScreenEvent Reset = new ScreenEvent(nameof(StepAScreen), "Reset");
		}
		protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			return screenConfiguration
				.SubStepOf(BlueFlowScreenName.FillDataStep)
				.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
				.OnEventNavigatesTo(StepEvent.Reset, BlueFlowScreenName.Step0Screen)
				.OnEventNavigatesTo(StepEvent.Next, BlueFlowScreenName.FillDataStep_StepBScreen)
				.OnEventNavigatesTo(StepEvent.Previous, BlueFlowScreenName.Step0Screen);
		}
		
		protected override Task OnHandlingStepEvent(ScreenEvent triggeredEvent, IUiFlowContextData contextData)
		{
			if (triggeredEvent == StepEvent.Reset)
			{
				contextData.Reset();

			}else if (triggeredEvent == StepEvent.Previous)
			{
				contextData.GetCurrentStepData<StepAScreenScreenModel>().StepAValue1 = null;
			}

			return Task.CompletedTask;
		}

        protected override bool OnValidateTransitionAttempt(ScreenEvent transitionTrigger,
            IUiFlowContextData contextData, out string errorMessage)
        {
			var result = true;
			errorMessage = null;
			if (transitionTrigger == StepEvent.Next)
			{
				var viewModel = contextData.GetCurrentStepData<StepAScreenScreenModel>();
				result = !string.IsNullOrEmpty(viewModel.StepAValue1);
				if (!result)
				{
					errorMessage = "Value cannot be empty";
				}

			}


			return result;
		}

		public override ScreenName ScreenStep =>
			 BlueFlowScreenName.FillDataStep_StepAScreen;

		public override string ViewPath { get; } = "StepA";
		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			return new StepAScreenScreenModel();
		}


		public class StepAScreenScreenModel : UiFlowScreenModel
		{
			public string StepAValue1 { get; set; }
			
			public override bool IsValidFor(ScreenName screenStep)
			{
				return  screenStep== BlueFlowScreenName.FillDataStep_StepAScreen;
			}
		}
	}
}