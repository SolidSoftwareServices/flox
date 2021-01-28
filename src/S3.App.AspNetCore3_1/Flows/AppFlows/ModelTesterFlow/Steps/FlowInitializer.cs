using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using S3.CoreServices.System;
using S3.App.AspNetCore3_1.Flows.AppFlows.ModelTesterFlow.FlowDefinitions;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.Infrastructure.DataSources;
using S3.UiFlows.Core.Flows;
using S3.UiFlows.Core.Flows.Initialization;
using S3.UiFlows.Core.Flows.Initialization.Models;
using S3.UiFlows.Core.Flows.Screens;



namespace S3.App.AspNetCore3_1.Flows.AppFlows.ModelTesterFlow.Steps
{
	public class FlowInitializer : UiFlowInitializationStep<SampleAppFlowType, FlowInitializer.StartScreenModel>
	{
		public override SampleAppFlowType InitializerOfFlowType => SampleAppFlowType.ModelTesterFlow;
		

		public override IScreenFlowConfigurator OnDefiningAdditionalInitialStateTransitions(IScreenFlowConfigurator preStartCfg,
			UiFlowContextData contextData)
		{
			
			return preStartCfg.OnEventNavigatesTo(ScreenEvent.Start, ModelTesterFlowStep.InputScreen);
		}

		public override bool Authorize()
		{
			return true;
		}


		public class StartScreenModel : InitialFlowScreenModel, IModelTesterInput
		{
			
			public string Info { get; set; }
		}
	}
}