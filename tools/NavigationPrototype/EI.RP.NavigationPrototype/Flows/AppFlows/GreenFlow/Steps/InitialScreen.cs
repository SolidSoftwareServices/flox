using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using EI.RP.NavigationPrototype.Flows.AppFlows.GreenFlow.FlowDefinitions;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;


namespace EI.RP.NavigationPrototype.Flows.AppFlows.GreenFlow.Steps
{
	public class InitialScreen : GreenFlowScreen
	{
		public static class StepEvent
		{
			public static readonly ScreenEvent Next = new ScreenEvent(nameof(InitialScreen),"Next");
			public static readonly ScreenEvent Reset = new ScreenEvent(nameof(InitialScreen), "Reset");
		}
		protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			bool ToStepC()
			{
				var stepValue1 = contextData.GetCurrentStepData<InitialScreenScreenModel>().StepValue1;
				return stepValue1 != null && stepValue1.StartsWith('a');
			}

			return screenConfiguration
				.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
				.OnEventReentriesCurrent(StepEvent.Reset)
				.OnEventNavigatesTo(StepEvent.Next, GreenFlowScreenName.StepAScreen,()=>!ToStepC(),"input is NOT a*")
				.OnEventNavigatesTo(StepEvent.Next, GreenFlowScreenName.StepCScreen,ToStepC,"input is a*");
		}

		protected override Task OnHandlingStepEvent(ScreenEvent triggeredEvent, IUiFlowContextData contextData)
		{
			if (triggeredEvent == StepEvent.Reset)
			{
				contextData.Reset();

			}

			return Task.CompletedTask;
		}

		protected override bool OnValidateTransitionAttempt(ScreenEvent transitionTrigger,
            IUiFlowContextData contextData, out string errorMessage)
        {
			bool result = true;
			errorMessage = null;
			if (transitionTrigger == StepEvent.Next)
			{
				var viewModel = contextData.GetCurrentStepData<InitialScreenScreenModel>();
				var b = int.TryParse(viewModel.StepValue1, out int value);
				errorMessage = b ? "Numeric only value  are not allowed" : string.Empty;
				result = !b;

			}

			return result;
		}

		public override ScreenName ScreenStep =>  GreenFlowScreenName.Step0Screen;
		public override string ViewPath => "Init";

		

		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			return new InitialScreenScreenModel();
		}


		public class InitialScreenScreenModel: UiFlowScreenModel
		{
			[Required(ErrorMessage = "{0} is required")]
			public string StepValue1 { get; set; }
			
			public override bool IsValidFor(ScreenName screenStep)
			{
				return  screenStep== GreenFlowScreenName.Step0Screen;
			}

			public override IEnumerable<ScreenEvent> DontValidateEvents =>
				base.DontValidateEvents.Union(StepEvent.Reset.ToOneItemArray());
		}
	}
}
