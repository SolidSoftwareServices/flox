using System.Threading.Tasks;
using S3.App.Flows.AppFlows.GreenFlow.FlowDefinitions;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;

namespace S3.App.Flows.AppFlows.GreenFlow.Steps
{
	public class StepAScreen : UiFlowScreen
	{
		public static class ScreenInputEvent
		{
			public static readonly ScreenEvent Previous = new ScreenEvent(nameof(StepAScreen), "Previous");
			public static readonly ScreenEvent Next = new ScreenEvent(nameof(StepAScreen), "Next");
			public static readonly ScreenEvent Reset = new ScreenEvent(nameof(StepAScreen), "Reset");
		}

		protected override IScreenFlowConfigurator OnRegisterUserActions(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			return screenConfiguration.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
				.OnEventNavigatesTo(ScreenInputEvent.Reset, GreenFlowScreenName.Step0Screen)
				.OnEventNavigatesTo(ScreenInputEvent.Next, GreenFlowScreenName.StepBScreen)
				.OnEventNavigatesTo(ScreenInputEvent.Previous, GreenFlowScreenName.Step0Screen)

				.OnEventExecutes(ScreenInputEvent.Reset, (e, ctx) => ctx.Reset())
				.OnEventExecutes(ScreenInputEvent.Previous, (e, ctx) => ctx.GetCurrentStepData<StepAScreenScreenModel>().StepAValue1 = null); ;
			;
		}


		protected override bool OnValidate(ScreenEvent transitionTrigger,
            IUiFlowContextData contextData, out string errorMessage)
        {
			bool result = true;
			errorMessage = null;
			if (transitionTrigger == ScreenInputEvent.Next)
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

		public override ScreenName ScreenNameId => GreenFlowScreenName.StepAScreen;

		public override string ViewPath { get; } = "StepA";

		protected override async Task<UiFlowScreenModel> OnCreateModelAsync(IUiFlowContextData contextData)
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