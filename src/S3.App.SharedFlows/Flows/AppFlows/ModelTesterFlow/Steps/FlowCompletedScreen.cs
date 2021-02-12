using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using S3.App.Flows.AppFlows.ModelTesterFlow.FlowDefinitions;
using S3.CoreServices.System;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;

namespace S3.App.Flows.AppFlows.ModelTesterFlow.Steps
{
	public class FlowCompletedScreen : UiFlowScreen
	{
		public class ScreenInputEvent
		{
			public static readonly ScreenEvent BackToEditValues=new ScreenEvent(nameof(FlowCompletedScreen), nameof(BackToEditValues));
		}
		public override ScreenName ScreenNameId => 
			ModelTesterFlowStep.FlowCompletedScreen;

		public override string ViewPath { get; } = "Completed";

		protected override IScreenFlowConfigurator OnRegisterUserActions(IScreenFlowConfigurator screenConfiguration,
			IUiFlowContextData contextData)
		{
			return screenConfiguration.OnEventNavigatesTo(ScreenInputEvent.BackToEditValues, ModelTesterFlowStep.InputScreen);
		}


		protected override async Task<UiFlowScreenModel> OnCreateModelAsync(IUiFlowContextData contextData)
		{
			var stepData = contextData.GetStepData<InputScreen.ScreenModel>();

			return Map(stepData, new FlowCompletedScreen.ScreenModel());
		}

		private static UiFlowScreenModel Map(InputScreen.ScreenModel src,ScreenModel destination)
		{
			var map = destination;
			map.StepValue1 = src.StepValue1;
			map.StepValue2 = src.StepValue2;
			map.RequiredOnlyIfNestedValue1DoesNotHaveAValue = src.RequiredOnlyIfNestedValue1DoesNotHaveAValue;
			map.StepValueNotUsedInTheViewShouldHaveAValue = src.StepValueNotUsedInTheViewShouldHaveAValue;
			map.Nested1 = src.Nested1.CloneDeep();
			map.Nested2 = src.Nested2.CloneDeep();
			map.SampleInput = src.StringValue;
			return map;
		}
			
		protected override async Task<UiFlowScreenModel> OnRefreshModelAsync(IUiFlowContextData contextData,
			UiFlowScreenModel originalScreenModel, IDictionary<string, object> stepViewCustomizations = null)
		{
			var result = Map(contextData.GetStepData<InputScreen.ScreenModel>(ModelTesterFlowStep.InputScreen),
				contextData.GetCurrentStepData<ScreenModel>());
			return result;
		}

		public class ScreenModel:UiFlowScreenModel
		{
			[DisplayName("Required value only if Nested1.NestedValue1 is empty")]
			public string RequiredOnlyIfNestedValue1DoesNotHaveAValue { get; set; }


			[DisplayName("Root Property 1:")]
			public string StepValue1 { get; set; }

			[DisplayName("Root Property 2:")]
			public string StepValue2 { get; set; }
			[DisplayName("This should have a value:")]
			public string StepValueNotUsedInTheViewShouldHaveAValue { get; set; }
			public override bool IsValidFor(ScreenName screenStep)
			{
				return screenStep == ModelTesterFlowStep.FlowCompletedScreen;
			}

			public InputScreen.ScreenModel.NestedStepData Nested1 { get; set; }
			public InputScreen.ScreenModel.NestedStepData Nested2 { get; set; }
			public string SampleInput { get; set; }
		}
	}
}