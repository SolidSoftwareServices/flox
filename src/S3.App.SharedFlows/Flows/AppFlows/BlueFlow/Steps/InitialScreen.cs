using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using S3.App.Flows.AppFlows.BlueFlow.FlowDefinitions;
using S3.CoreServices.System;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Metadata;
using S3.UiFlows.Core.Flows.Screens.Models;

namespace S3.App.Flows.AppFlows.BlueFlow.Steps
{
	public class InitialScreen : UiFlowScreen
	{
		public override ScreenName ScreenNameId => BlueFlowScreenName.Step0Screen;
		public override string ViewPath => "Init";

		protected override IScreenFlowConfigurator OnRegisterUserActions(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			

			ThrowIfMustFail(contextData, ScreenLifecycleStage.DefiningTransitionsFromCurrentScreen);


			return screenConfiguration
				.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred,()=>contextData.LastError.LifecycleStage==ScreenLifecycleStage.ValidateTransitionCompletedWithErrors,"Validation error")
				.OnEventNavigatesTo(ScreenEvent.ErrorOccurred, BlueFlowScreenName.ErrorScreen, () => contextData.LastError.LifecycleStage != ScreenLifecycleStage.ValidateTransitionCompletedWithErrors, "Not a validation error")
				.OnEventReentriesCurrent(ScreenInputEvent.Reset)
				.OnEventNavigatesTo(ScreenInputEvent.Next, BlueFlowScreenName.FillDataStep_StepAScreen, () => !ToStepC(),
					"input is NOT a*")
				.OnEventNavigatesTo(ScreenInputEvent.Next, BlueFlowScreenName.StepCScreen, ToStepC, "input is a*")
				.OnEventExecutes(ScreenInputEvent.Reset,OnReset);
			bool ToStepC()
			{
				var stepValue1 = contextData.GetCurrentStepData<InitialScreenScreenModel>().StepValue1;
				return stepValue1 != null && stepValue1.StartsWith('a');
			}
		}

		private void ThrowIfMustFail(IUiFlowContextData contextData, ScreenLifecycleStage stage)
		{
			var root = contextData.GetStepData<FlowInitializer.StartScreenModel>();
			if (root != null && (ScreenName) root.FailOnStep == ScreenNameId && root.FailOnEvent == stage)
				throw new Exception($"Failing on {ScreenNameId}.{stage}");
		}

		protected  Task OnReset(ScreenEvent triggeredEvent, IUiFlowContextData contextData)
		{
			ThrowIfMustFail(contextData, ScreenLifecycleStage.HandlingEvent);
			contextData.Reset();
			return Task.CompletedTask;
		}

		protected override bool OnValidate(ScreenEvent transitionTrigger,
			IUiFlowContextData contextData, out string errorMessage)
		{
			ThrowIfMustFail(contextData, ScreenLifecycleStage.ValidatingTransition);
			var result = true;
			errorMessage = null;
			if (transitionTrigger == ScreenInputEvent.Next)
			{
				var viewModel = contextData.GetCurrentStepData<InitialScreenScreenModel>();
				var b = int.TryParse(viewModel.StepValue1, out var value);
				errorMessage = b ? "Numeric only value  are not allowed" : string.Empty;
				result = !b;
			}

			return result;
		}

		protected override async Task<UiFlowScreenModel> OnCreateModelAsync(IUiFlowContextData contextData)
		{
			ThrowIfMustFail(contextData, ScreenLifecycleStage.CreatingStepData);
			var result = new InitialScreenScreenModel();
			return result;
		}

		protected override Task<UiFlowScreenModel> OnRefreshModelAsync(IUiFlowContextData contextData,
			UiFlowScreenModel originalScreenModel,
			IDictionary<string, object> stepViewCustomizations = null)
		{
			ThrowIfMustFail(contextData, ScreenLifecycleStage.RefreshingStepData);
			return base.OnRefreshModelAsync(contextData, originalScreenModel, stepViewCustomizations);
		}

		public static class ScreenInputEvent
		{
			public static readonly ScreenEvent Next = new ScreenEvent(nameof(InitialScreen), nameof(Next));
			public static readonly ScreenEvent Reset = new ScreenEvent(nameof(InitialScreen), nameof(Reset));
		}

		public class InitialScreenScreenModel : UiFlowScreenModel
		{
			[Required(ErrorMessage = "{0} is required")]
			public string StepValue1 { get; set; }

			public override IEnumerable<ScreenEvent> DontValidateEvents =>
				base.DontValidateEvents.Union(ScreenInputEvent.Reset.ToOneItemArray());

			//we use the same as the input
			[Required(ErrorMessage = "ERROR TEST VALUE")]
			public string StringValue { get; set; }

			public override bool IsValidFor(ScreenName screenStep)
			{
				return screenStep == BlueFlowScreenName.Step0Screen;
			}
		}
	}
}