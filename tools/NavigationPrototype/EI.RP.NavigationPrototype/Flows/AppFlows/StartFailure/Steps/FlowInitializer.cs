using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using EI.RP.NavigationPrototype.Flows.AppFlows.GreenFlow.FlowDefinitions;
using EI.RP.NavigationPrototype.Flows.AppFlows.StartFailure.FlowDefinitions;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Initialization;
using EI.RP.UiFlows.Core.Flows.Initialization.Models;
using EI.RP.UiFlows.Core.Flows.Screens;



namespace EI.RP.NavigationPrototype.Flows.AppFlows.StartFailure.Steps
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