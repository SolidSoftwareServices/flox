using System.Threading.Tasks;
using S3.App.Flows.AppFlows.GreenFlow.FlowDefinitions;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;

namespace S3.App.Flows.AppFlows.GreenFlow.Steps
{
	public class StepBScreen : UiFlowScreen
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
			bool result = true;
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

		public override ScreenName ScreenNameId =>  GreenFlowScreenName.StepBScreen;
		public override string ViewPath { get; } = "StepB";

		protected override IScreenFlowConfigurator OnRegisterUserActions(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			return screenConfiguration.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
				.OnEventNavigatesTo(ScreenInputEvent.Reset, GreenFlowScreenName.Step0Screen)
				.OnEventNavigatesTo(ScreenInputEvent.Next, GreenFlowScreenName.StepCScreen)
				.OnEventNavigatesTo(ScreenInputEvent.Previous, GreenFlowScreenName.StepAScreen)
				
				.OnEventExecutes(ScreenInputEvent.Reset,  (e,ctx)=>ctx.Reset())
				.OnEventExecutes(ScreenInputEvent.Previous, (e, ctx) => ctx.GetCurrentStepData<StepBScreenScreenModel>().StepBValue1 = null);
		}


		protected override async Task<UiFlowScreenModel> OnCreateModelAsync(IUiFlowContextData contextData)
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