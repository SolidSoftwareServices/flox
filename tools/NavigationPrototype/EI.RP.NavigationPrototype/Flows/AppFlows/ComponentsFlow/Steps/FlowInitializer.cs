using EI.RP.NavigationPrototype.Flows.AppFlows.MetadataTestFlow.FlowDefinitions;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Initialization;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;

namespace EI.RP.NavigationPrototype.Flows.AppFlows.ComponentsFlow.Steps
{
	public class FlowInitializer : UiFlowInitializationStep<SampleAppFlowType>
	{
		public override SampleAppFlowType InitializerOfFlowType => SampleAppFlowType.ComponentsFlow;

		public override IScreenFlowConfigurator OnDefiningAdditionalInitialStateTransitions(IScreenFlowConfigurator preStartCfg,
			UiFlowContextData contextData)
		{
			return preStartCfg.OnEventNavigatesTo(ScreenEvent.Start, MetadataTestFlowScreenScreenName.Step0Screen);
		}

		public override bool Authorize()
		{
			return true;
		}

		
	}
}