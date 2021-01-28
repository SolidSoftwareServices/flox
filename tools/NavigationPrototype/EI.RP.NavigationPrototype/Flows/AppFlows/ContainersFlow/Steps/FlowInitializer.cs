using EI.RP.NavigationPrototype.Flows.AppFlows.ContainersFlow.FlowDefinitions;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Initialization;
using EI.RP.UiFlows.Core.Flows.Screens;


namespace EI.RP.NavigationPrototype.Flows.AppFlows.ContainersFlow.Steps
{
	public class FlowInitializer : UiFlowInitializationStep<SampleAppFlowType>
	{
		public override SampleAppFlowType InitializerOfFlowType => SampleAppFlowType.ContainersFlow;

		public override bool Authorize()
		{
			return true;
		}


		public override IScreenFlowConfigurator OnDefiningAdditionalInitialStateTransitions(IScreenFlowConfigurator preStartCfg,
			UiFlowContextData contextData)
		{
			return preStartCfg.OnEventNavigatesTo(ScreenEvent.Start, ContainersFlowScreenName.Number1ContainerScreen);
		}
	}
}