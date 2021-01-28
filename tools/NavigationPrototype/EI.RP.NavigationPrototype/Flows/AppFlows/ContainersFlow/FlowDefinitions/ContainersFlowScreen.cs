using EI.RP.UiFlows.Core.Flows.Screens;


namespace EI.RP.NavigationPrototype.Flows.AppFlows.ContainersFlow.FlowDefinitions
{
	public abstract class ContainersFlowScreen : UiFlowContainerScreen<SampleAppFlowType>
	{
		public override SampleAppFlowType IncludedInFlowType => SampleAppFlowType.ContainersFlow;

		
	}
}