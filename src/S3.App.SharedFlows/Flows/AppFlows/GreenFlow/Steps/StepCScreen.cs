using System.Collections.Generic;
using System.Threading.Tasks;
using S3.App.Flows.AppFlows.GreenFlow.FlowDefinitions;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;

namespace S3.App.Flows.AppFlows.GreenFlow.Steps
{
	public class StepCScreen : GreenFlowScreen
	{
		public static class StepEvent
		{
			public static readonly ScreenEvent Previous = new ScreenEvent(nameof(StepCScreen), "Previous");
			public static readonly ScreenEvent FlowTransitionCompleted = new ScreenEvent(nameof(StepCScreen), "FlowCompleted");
			public static readonly ScreenEvent Reset = new ScreenEvent(nameof(StepCScreen), "Reset");
			public static readonly ScreenEvent StartBlueFlow = new ScreenEvent(nameof(StepCScreen), nameof(StartBlueFlow));

		}
		protected override bool OnValidate(ScreenEvent transitionTrigger,
            IUiFlowContextData contextData, out string errorMessage)
        {
			errorMessage = null;
			return true;
		}

		public override ScreenName ScreenStep =>  GreenFlowScreenName.StepCScreen;
		public override string ViewPath { get; } = "StepC";

		protected override IScreenFlowConfigurator OnRegisterUserActions(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			bool ByPassedStepAandB()
			{
				var stepValue1 = contextData.GetStepData<InitialScreen.InitialScreenScreenModel>(GreenFlowScreenName.Step0Screen).StepValue1;
				return stepValue1 != null && stepValue1.StartsWith('a');
			}

			return screenConfiguration.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
				.OnEventNavigatesTo(StepEvent.Reset, GreenFlowScreenName.Step0Screen)
				.OnEventNavigatesTo(StepEvent.FlowTransitionCompleted, GreenFlowScreenName.FlowCompletedScreen)
				.OnEventNavigatesTo(StepEvent.Previous, GreenFlowScreenName.StepBScreen, () => !ByPassedStepAandB(),"Comes from B")
				.OnEventNavigatesTo(StepEvent.Previous, GreenFlowScreenName.Step0Screen, ByPassedStepAandB, "Comes from step0")
				.OnEventNavigatesTo(StepEvent.StartBlueFlow, GreenFlowScreenName.RunBlueFlow)

				.OnEventExecutes(StepEvent.Reset, (e, ctx) => ctx.Reset());
		}


		protected override async Task<UiFlowScreenModel> OnCreateModelAsync(IUiFlowContextData contextData)
		{
			return new StepCScreenScreenModel
			{
				FlowInputData = contextData.GetStepData<FlowInitializer.StartScreenModel>(ScreenName.PreStart).SampleParameter,
				InitialValue = contextData.GetStepData<InitialScreen.InitialScreenScreenModel>(GreenFlowScreenName.Step0Screen).StepValue1,
				StepAValue = contextData.GetStepData<StepAScreen.StepAScreenScreenModel>(GreenFlowScreenName.StepAScreen)?.StepAValue1,
				StepBValue = contextData.GetStepData<StepBScreen.StepBScreenScreenModel>(GreenFlowScreenName.StepBScreen)?.StepBValue1,
			};
		}

		protected override async Task<UiFlowScreenModel> OnRefreshModelAsync(IUiFlowContextData contextData,
			UiFlowScreenModel originalScreenModel, IDictionary<string, object> stepViewCustomizations = null)
		{
			var result = (StepCScreenScreenModel)originalScreenModel;


			result.FlowInputData = contextData.GetStepData<FlowInitializer.StartScreenModel>(ScreenName.PreStart).SampleParameter;
			result.InitialValue = contextData
				.GetStepData<InitialScreen.InitialScreenScreenModel>(GreenFlowScreenName.Step0Screen).StepValue1;
			result.StepAValue = contextData
				.GetStepData<StepAScreen.StepAScreenScreenModel>(GreenFlowScreenName.StepAScreen)
				?.StepAValue1;
			result.StepBValue = contextData
				.GetStepData<StepBScreen.StepBScreenScreenModel>(GreenFlowScreenName.StepBScreen)
				?.StepBValue1;
			return result;
		}

		public class StepCScreenScreenModel : UiFlowScreenModel
		{
			public override bool IsValidFor(ScreenName screenStep)
			{
				return screenStep== GreenFlowScreenName.StepCScreen;
			}

			public string InitialValue { get; set; }
			public string StepAValue { get; set; }
			public string StepBValue { get; set; }
			public string FlowInputData { get; set; }
		}
	}
}