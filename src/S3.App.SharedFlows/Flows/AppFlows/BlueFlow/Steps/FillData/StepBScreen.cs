using System.Threading.Tasks;
using S3.App.Flows.AppFlows.BlueFlow.FlowDefinitions;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;

namespace S3.App.Flows.AppFlows.BlueFlow.Steps.FillData
{
	public class StepBScreen : BlueFlowScreen
	{
		public static class ScreenInputEvent
		{
			public static readonly ScreenEvent Previous = new ScreenEvent(nameof(StepBScreen), "Previous");
			public static readonly ScreenEvent Next = new ScreenEvent(nameof(StepBScreen), "Next");
			
			public static readonly ScreenEvent Reset = new ScreenEvent(nameof(StepBScreen), "Reset");
		}
		protected override bool OnValidate(ScreenEvent transitionTrigger,
            IUiFlowContextData contextData, out string errorMessage)
        {
			var result = true;
			errorMessage = null;
			if (transitionTrigger == ScreenInputEvent.Next)
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

		public override ScreenName ScreenStep =>  BlueFlowScreenName.FillDataStep_StepBScreen;
		public override string ViewPath { get; } = "StepB";

		protected override IScreenFlowConfigurator OnRegisterUserActions(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			return screenConfiguration
				.SubStepOf(BlueFlowScreenName.FillDataStep)
				.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
				.OnEventNavigatesTo(ScreenInputEvent.Reset, BlueFlowScreenName.Step0Screen)
				.OnEventNavigatesTo(ScreenInputEvent.Next, BlueFlowScreenName.StepCScreen)
				.OnEventNavigatesTo(ScreenInputEvent.Previous, BlueFlowScreenName.FillDataStep_StepAScreen)

				.OnEventExecutes(ScreenInputEvent.Reset, (e, ctx) => ctx.Reset())
				.OnEventExecutes(ScreenInputEvent.Previous, (e, ctx) => ctx.GetCurrentStepData<StepBScreenScreenModel>().StepBValue1 = null); ;
		}

		
		protected override async Task<UiFlowScreenModel> OnCreateModelAsync(IUiFlowContextData contextData)
		{
			return new StepBScreenScreenModel();
		}


		public class StepBScreenScreenModel : UiFlowScreenModel
		{
			public string StepBValue1 { get; set; }

			public override bool IsValidFor(ScreenName screenStep)
			{
				return  screenStep== BlueFlowScreenName.FillDataStep_StepBScreen;
			}
		}
	}
}