using EI.RP.UiFlows.Core.Flows.Screens;


namespace EI.RP.NavigationPrototype.Flows.AppFlows.BlueFlow.FlowDefinitions
{
	public abstract class BlueFlowScreen : UiFlowScreen<SampleAppFlowType>
	{
		public override SampleAppFlowType IncludedInFlowType => SampleAppFlowType.BlueFlow;
	}
}