using System.Threading.Tasks;
using S3.App.Flows.AppFlows.GreenFlow.FlowDefinitions;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;

namespace S3.App.Flows.AppFlows.GreenFlow.Steps
{
	public class StepBScreen : GreenFlowScreen
	{
		public static class StepEvent
		{
			public static readonly ScreenEvent Previous = new ScreenEvent(nameof(StepBScreen), "Previous");
			public static readonly ScreenEvent Next = new ScreenEvent(nameof(StepBScreen), "Next");
			public static readonly ScreenEvent Reset = new ScreenEvent(nameof(StepBScreen), "Reset");
		}
		protected override bool OnValidateTransitionAttempt(ScreenEvent transitionTrigger,
            IUiFlowContextData contextData, out string errorMessage)
        {
			bool result = true;
			errorMessage = null;
			if (transitionTrigger == StepEvent.Next)
			{
				var viewModel = contextData.GetCurrentStepData<StepBScreenScreenModel>();
					result = !string.IsNullOrEmpty(viewModel.StepBValue1);
					if (!result)
					{
						errorMessage = "Value cannot be empty";
					}

			}


			return result;
		}

		public override ScreenName ScreenStep =>  GreenFlowScreenName.StepBScreen;
		public override string ViewPath { get; } = "StepB";

		protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			return screenConfiguration.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
				.OnEventNavigatesTo(StepEvent.Reset, GreenFlowScreenName.Step0Screen)
				.OnEventNavigatesTo(StepEvent.Next, GreenFlowScreenName.StepCScreen)
				.OnEventNavigatesTo(StepEvent.Previous, GreenFlowScreenName.StepAScreen);
		}

		protected override Task OnHandlingStepEvent(ScreenEvent triggeredEvent, IUiFlowContextData contextData)
		{
			 if (triggeredEvent == StepEvent.Reset)
			{
				contextData.Reset();
			 }
			 else if (triggeredEvent == StepEvent.Previous)
			 {
				 contextData.GetCurrentStepData<StepBScreenScreenModel>().StepBValue1 = null;
			 }
			return Task.CompletedTask;
		}

		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			return new StepBScreenScreenModel
			{
				//TODO: rest
			};
		}

		public class StepBScreenScreenModel : UiFlowScreenModel
		{
			public string StepBValue1 { get; set; }

			public override bool IsValidFor(ScreenName screenStep)
			{
				return  screenStep== GreenFlowScreenName.StepBScreen;
			}
		}
	}
}