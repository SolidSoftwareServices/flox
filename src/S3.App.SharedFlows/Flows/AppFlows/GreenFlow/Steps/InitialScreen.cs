using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using S3.App.Flows.AppFlows.GreenFlow.FlowDefinitions;
using S3.CoreServices.System;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;

namespace S3.App.Flows.AppFlows.GreenFlow.Steps
{
	public class InitialScreen : UiFlowScreen
	{
		public static class ScreenInputEvent
		{
			public static readonly ScreenEvent Next = new ScreenEvent(nameof(InitialScreen),"Next");
			public static readonly ScreenEvent Reset = new ScreenEvent(nameof(InitialScreen), "Reset");
		}
		protected override IScreenFlowConfigurator OnRegisterUserActions(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			bool ToStepC()
			{
				var stepValue1 = contextData.GetCurrentStepData<InitialScreenScreenModel>().StepValue1;
				return stepValue1 != null && stepValue1.StartsWith('a');
			}

			return screenConfiguration
				.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
				.OnEventReentriesCurrent(ScreenInputEvent.Reset)
				.OnEventNavigatesTo(ScreenInputEvent.Next, GreenFlowScreenName.StepAScreen,()=>!ToStepC(),"input is NOT a*")
				.OnEventNavigatesTo(ScreenInputEvent.Next, GreenFlowScreenName.StepCScreen,ToStepC,"input is a*")

				.OnEventExecutes(ScreenInputEvent.Reset, (e, ctx) => ctx.Reset())
				;
		}

		

		protected override bool OnValidate(ScreenEvent transitionTrigger,
            IUiFlowContextData contextData, out string errorMessage)
        {
			bool result = true;
			errorMessage = null;
			if (transitionTrigger == ScreenInputEvent.Next)
			{
				var viewModel = contextData.GetCurrentStepData<InitialScreenScreenModel>();
				var b = int.TryParse(viewModel.StepValue1, out int value);
				errorMessage = b ? "Numeric only value  are not allowed" : string.Empty;
				result = !b;

			}

			return result;
		}

		public override ScreenName ScreenNameId =>  GreenFlowScreenName.Step0Screen;
		public override string ViewPath => "Init";

		

		protected override async Task<UiFlowScreenModel> OnCreateModelAsync(IUiFlowContextData contextData)
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
				base.DontValidateEvents.Union(ScreenInputEvent.Reset.ToOneItemArray());
		}
	}
}
