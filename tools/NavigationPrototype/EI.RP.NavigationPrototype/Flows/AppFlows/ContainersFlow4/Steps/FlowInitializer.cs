﻿using EI.RP.NavigationPrototype.Flows.AppFlows.ContainersFlow4.FlowDefinitions;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Initialization;
using EI.RP.UiFlows.Core.Flows.Screens;

namespace EI.RP.NavigationPrototype.Flows.AppFlows.ContainersFlow4.Steps
{
	public class FlowInitializer : UiFlowInitializationStep<SampleAppFlowType>
	{

		public override SampleAppFlowType InitializerOfFlowType => SampleAppFlowType.ContainersFlow4;

		public override bool Authorize()
		{
			return true;
		}


		public override IScreenFlowConfigurator OnDefiningAdditionalInitialStateTransitions(IScreenFlowConfigurator preStartCfg,
			UiFlowContextData contextData)
		{
			return preStartCfg.OnEventNavigatesTo(ScreenEvent.Start, ContainersFlow4ScreenName.Number1ContainerScreen);
		}
	}
}