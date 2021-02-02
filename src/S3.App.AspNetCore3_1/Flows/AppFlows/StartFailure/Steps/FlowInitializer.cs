using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using S3.CoreServices.System;
using S3.App.AspNetCore3_1.Flows.AppFlows.GreenFlow.FlowDefinitions;
using S3.App.AspNetCore3_1.Flows.AppFlows.StartFailure.FlowDefinitions;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows;
using S3.UiFlows.Core.Flows.Initialization;
using S3.UiFlows.Core.Flows.Initialization.Models;
using S3.UiFlows.Core.Flows.Screens;



namespace S3.App.AspNetCore3_1.Flows.AppFlows.StartFailure.Steps
{
	public class FlowInitializer : UiFlowInitializationStep<SampleAppFlowType,FlowInitializer.StartScreenModel>
	{
		public override SampleAppFlowType InitializerOfFlowType => SampleAppFlowType.StartFailure;

		public override IScreenFlowConfigurator OnDefiningAdditionalInitialStateTransitions(IScreenFlowConfigurator preStartCfg,
			UiFlowContextData contextData)
		{
			return preStartCfg.OnEventNavigatesTo(ScreenEvent.ErrorOccurred, StartFailureFlowScreenName.FailureScreen);
		}

		public override bool Authorize()
		{
			return true;
		}


		public class StartScreenModel : InitialFlowScreenModel
		{
			public StartScreenModel()
			{
				throw new Exception("Simulated exception on startup");
			}
			public override bool IsValidFor(ScreenName screenStep)
			{
				return screenStep == ScreenName.PreStart;
			}

			public string Info { get; set; }
		}
	}
}