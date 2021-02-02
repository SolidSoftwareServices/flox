using System;
using System.Linq;
using System.Threading.Tasks;
using S3.App.AspNetCore3_1.Flows.AppFlows.GreenFlow.FlowDefinitions;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;


namespace S3.App.AspNetCore3_1.Flows.AppFlows.GreenFlow.Steps
{
	public class FlowCompletedScreen : GreenFlowScreen
	{
		
		public override ScreenName ScreenStep => 
			GreenFlowScreenName.FlowCompletedScreen;

		public override string ViewPath { get; } = "Completed";




		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			var stepData = new FlowCompletedScreenScreenModel
			{
			};
			var runBlueFlowStep = contextData.GetStepData<RunBlueFlowScreen.StepData>(GreenFlowScreenName.RunBlueFlow);

			if (contextData.CurrentEvents.Any(x=>x==RunBlueFlowScreen.StepEvent.BlueFlowCompleted) 
			    || contextData.EventsLog.Any(x =>x.Event == RunBlueFlowScreen.StepEvent.BlueFlowCompleted))
			{
				stepData.BlueFlowInitialScreenValue =  runBlueFlowStep.CalledFlowResult;
			}

			stepData.BlueFlowEventHandled = runBlueFlowStep != null && runBlueFlowStep.BlueFlowCompletedEventHandled;

			return stepData;
		}

		public class FlowCompletedScreenScreenModel : UiFlowScreenModel
		{
			public override bool IsValidFor(ScreenName screenStep)
			{
					return  screenStep== GreenFlowScreenName.FlowCompletedScreen;
			}

			public string BlueFlowInitialScreenValue { get; set; }
			public bool BlueFlowEventHandled { get; set; }
		}
	}
}