using System.Linq;
using System.Threading.Tasks;
using S3.App.Flows.AppFlows.GreenFlow.FlowDefinitions;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;

namespace S3.App.Flows.AppFlows.GreenFlow.Steps
{
	public class FlowCompletedScreen : UiFlowScreen
	{
		
		public override ScreenName ScreenNameId => 
			GreenFlowScreenName.FlowCompletedScreen;

		public override string ViewPath { get; } = "Completed";




		protected override async Task<UiFlowScreenModel> OnCreateModelAsync(IUiFlowContextData contextData)
		{
			var stepData = new FlowCompletedScreenScreenModel
			{
			};
			var runBlueFlowStep = contextData.GetStepData<RunBlueFlowScreen.StepData>(GreenFlowScreenName.RunBlueFlow);

			if (contextData.CurrentEvents.Any(x=>x==RunBlueFlowScreen.ScreenInputEvent.BlueFlowCompleted) 
			    || contextData.EventsLog.Any(x =>x.Event == RunBlueFlowScreen.ScreenInputEvent.BlueFlowCompleted))
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