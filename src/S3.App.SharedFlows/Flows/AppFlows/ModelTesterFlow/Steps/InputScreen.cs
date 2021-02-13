using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using S3.App.Flows.AppFlows.ModelTesterFlow.FlowDefinitions;
using S3.Mvc.Core.ViewModels.Validations;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;

namespace S3.App.Flows.AppFlows.ModelTesterFlow.Steps
{
	public class InputScreen : UiFlowScreen
	{

		public static class ScreenInputEvent
		{
			public static readonly ScreenEvent Submit = new ScreenEvent(nameof(InputScreen), nameof(Submit));
		}

		

		protected override IScreenFlowConfigurator OnRegisterUserActions(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{

			return screenConfiguration
				.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
				.OnEventNavigatesTo(ScreenInputEvent.Submit, ModelTesterFlowStep.FlowCompletedScreen);
		}

		

		protected override bool OnValidate(ScreenEvent transitionTrigger,
			IUiFlowContextData contextData, out string errorMessage)
		{
			var result = true;
			errorMessage = null;
			
			return result;
		
		}

		public override ScreenName ScreenNameId =>  ModelTesterFlowStep.InputScreen;
		public override string ViewPath => "Input";

		protected override async Task<UiFlowScreenModel> OnCreateModelAsync(IUiFlowContextData contextData)
		{
			var result = new ScreenModel()
			{
				StepValueNotUsedInTheViewShouldHaveAValue = Guid.NewGuid().ToString(),
				Nested1 = new ScreenModel.NestedStepData
				{
					Nested = new ScreenModel.NestedStepData
					{
						Nested = new ScreenModel.NestedStepData
						{
							NestedValue1 = "Nested1 level3"
						},
						NestedValue1 = "Nested1 level2"
					},
					NestedValue1 = "Nested1 level1"

				},
				Nested2 = new ScreenModel.NestedStepData
				{
					Nested = new ScreenModel.NestedStepData
					{
						Nested = new ScreenModel.NestedStepData
						{
							NestedValue1 = "Nested2 level3"
						},
						NestedValue1 = "Nested2 level2"
					},
					NestedValue1 = "Nested2 level1"

				}
				,
				StepValue1 = "Root value 1",
				StepValue2 = "Root value 2",
				
			};
			return result;
		}

		public class ScreenModel: UiFlowScreenModel
		{
			[DisplayName("Root Property 1")]
			[Required(ErrorMessage = "{0} is required")]
			public string StepValue1 { get; set; }

			[DisplayName("Root Property 2")]
			[Required(ErrorMessage = "{0} is required")]
			public string StepValue2 { get; set; }

			[DisplayName("Required If Nested1.NestedValue1 DoesNotHaveAValue")]
			[RequiredIf("Nested1.NestedValue1", IfValues =  new object[] {"",null}, ErrorMessage = "Required")]
			public string RequiredOnlyIfNestedValue1DoesNotHaveAValue { get; set; }

			public string StepValueNotUsedInTheViewShouldHaveAValue { get; set; }
			public override bool IsValidFor(ScreenName screenStep)
			{
				return  screenStep== ModelTesterFlowStep.InputScreen ;
			}

			public NestedStepData Nested1 { get; set; }
			public NestedStepData Nested2 { get; set; }

			public class NestedStepData
			{
				[DisplayName("Nested Property")]
				//[Required(ErrorMessage = "{0} is required")]
				public string NestedValue1 { get; set; }
				public NestedStepData Nested { get; set; }
			}
			[Required(ErrorMessage = "ERROR TEST VALUE")]
			public string StringValue { get; set; }
		}
	}
}
