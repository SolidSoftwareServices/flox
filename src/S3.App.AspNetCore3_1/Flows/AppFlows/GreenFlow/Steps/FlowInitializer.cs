using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using S3.CoreServices.System;
using S3.App.AspNetCore3_1.Flows.AppFlows.GreenFlow.FlowDefinitions;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.Infrastructure.DataSources;
using S3.UiFlows.Core.Flows;
using S3.UiFlows.Core.Flows.Initialization;
using S3.UiFlows.Core.Flows.Initialization.Models;
using S3.UiFlows.Core.Flows.Screens;



namespace S3.App.AspNetCore3_1.Flows.AppFlows.GreenFlow.Steps
{
	public class FlowInitializer : UiFlowInitializationStep<SampleAppFlowType, FlowInitializer.StartScreenModel>
	{
		public override SampleAppFlowType InitializerOfFlowType => SampleAppFlowType.GreenFlow;

		public override IScreenFlowConfigurator OnDefiningAdditionalInitialStateTransitions(IScreenFlowConfigurator preStartCfg,
			UiFlowContextData contextData)
		{

			return preStartCfg.OnEventNavigatesTo(ScreenEvent.Start, GreenFlowScreenName.Step0Screen
					, () => contextData.GetCurrentStepData<StartScreenModel>().SampleParameter != "Finish"
					, "normal start"
				)
				.OnEventNavigatesTo(ScreenEvent.Start, GreenFlowScreenName.FlowCompletedScreen
					, () => contextData.GetCurrentStepData<StartScreenModel>().SampleParameter == "Finish"
					, "straight to the end"
				);
		}

		public override bool Authorize()
		{
			return true;
		}

		
		public class StartScreenModel : InitialFlowScreenModel, IGreenInput
		{
			
			public string SampleParameter { get; set; }
		}

	}
}