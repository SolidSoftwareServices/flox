using System.Threading.Tasks;
using S3.App.Flows.AppFlows.GreenFlow.FlowDefinitions;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;

namespace S3.App.Flows.AppFlows.GreenFlow.Steps
{
	public class StepAScreen : GreenFlowScreen
	{
		public static class StepEvent
		{
			public static readonly ScreenEvent Previous = new ScreenEvent(nameof(StepAScreen), "Previous");
			public static readonly ScreenEvent Next = new ScreenEvent(nameof(StepAScreen), "Next");
			public static readonly ScreenEvent Reset = new ScreenEvent(nameof(StepAScreen), "Reset");
		}

		protected override IScreenFlowConfigurator OnConfiguringScreenEventHandlersAndNavigations(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			return screenConfiguration.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
				.OnEventNavigatesTo(StepEvent.Reset, GreenFlowScreenName.Step0Screen)
				.OnEventNavigatesTo(StepEvent.Next, GreenFlowScreenName.StepBScreen)
				.OnEventNavigatesTo(StepEvent.Previous, GreenFlowScreenName.Step0Screen)

				.OnEventExecutes(StepEvent.Reset, (e, ctx) => ctx.Reset())
				.OnEventExecutes(StepEvent.Previous, (e, ctx) => ctx.GetCurrentStepData<StepAScreenScreenModel>().StepAValue1 = null); ;
			;
		}


		protected override bool OnValidateTransitionAttempt(ScreenEvent transitionTrigger,
            IUiFlowContextData contextData, out string errorMessage)
        {
			bool result = true;
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

		public override ScreenName ScreenStep => GreenFlowScreenName.StepAScreen;

		public override string ViewPath { get; } = "StepA";

		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			return new StepAScreenScreenModel(){ScreenTitle="Green Flow -Step A"};
		}

		public class StepAScreenScreenModel : UiFlowScreenModel
		{
			public string StepAValue1 { get; set; }
			
			public override bool IsValidFor(ScreenName screenStep)
			{
				return  screenStep== GreenFlowScreenName.StepAScreen;
			}
		}
	}
}