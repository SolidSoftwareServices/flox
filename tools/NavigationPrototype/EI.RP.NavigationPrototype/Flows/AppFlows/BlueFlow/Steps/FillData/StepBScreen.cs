﻿using System.Threading.Tasks;
using EI.RP.NavigationPrototype.Flows.AppFlows.BlueFlow.FlowDefinitions;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;


namespace EI.RP.NavigationPrototype.Flows.AppFlows.BlueFlow.Steps.FillData
{
	public class StepBScreen : BlueFlowScreen
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
			var result = true;
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

		public override ScreenName ScreenStep =>  BlueFlowScreenName.FillDataStep_StepBScreen;
		public override string ViewPath { get; } = "StepB";

		protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			return screenConfiguration
				.SubStepOf(BlueFlowScreenName.FillDataStep)
				.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
				.OnEventNavigatesTo(StepEvent.Reset, BlueFlowScreenName.Step0Screen)
				.OnEventNavigatesTo(StepEvent.Next, BlueFlowScreenName.StepCScreen)
				.OnEventNavigatesTo(StepEvent.Previous, BlueFlowScreenName.FillDataStep_StepAScreen);
		}

		protected override Task OnHandlingStepEvent(ScreenEvent triggeredEvent,
			IUiFlowContextData contextData)
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