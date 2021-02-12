using System;
using S3.App.Flows.AppFlows.StartFailure.FlowDefinitions;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows.Initialization;
using S3.UiFlows.Core.Flows.Initialization.Models;
using S3.UiFlows.Core.Flows.Screens;

namespace S3.App.Flows.AppFlows.StartFailure.Steps
{
	public class FlowInitializer : UiFlowInitializationStep<FlowInitializer.StartScreenModel>
	{

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