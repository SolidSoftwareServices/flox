using System;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.NavigationPrototype.Flows.AppFlows.GreenFlow.FlowDefinitions;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;


namespace EI.RP.NavigationPrototype.Flows.AppFlows.GreenFlow.Steps
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