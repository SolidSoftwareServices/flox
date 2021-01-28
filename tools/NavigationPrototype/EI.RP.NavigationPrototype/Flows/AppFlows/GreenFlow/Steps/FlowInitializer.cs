using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using EI.RP.NavigationPrototype.Flows.AppFlows.GreenFlow.FlowDefinitions;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Initialization;
using EI.RP.UiFlows.Core.Flows.Initialization.Models;
using EI.RP.UiFlows.Core.Flows.Screens;



namespace EI.RP.NavigationPrototype.Flows.AppFlows.GreenFlow.Steps
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