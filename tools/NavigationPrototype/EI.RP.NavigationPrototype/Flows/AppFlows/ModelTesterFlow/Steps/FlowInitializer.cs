using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using EI.RP.NavigationPrototype.Flows.AppFlows.ModelTesterFlow.FlowDefinitions;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Initialization;
using EI.RP.UiFlows.Core.Flows.Initialization.Models;
using EI.RP.UiFlows.Core.Flows.Screens;



namespace EI.RP.NavigationPrototype.Flows.AppFlows.ModelTesterFlow.Steps
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