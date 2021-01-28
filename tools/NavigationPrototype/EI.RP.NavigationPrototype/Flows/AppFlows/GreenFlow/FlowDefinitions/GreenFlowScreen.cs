using EI.RP.UiFlows.Core.Flows.Screens;


namespace EI.RP.NavigationPrototype.Flows.AppFlows.GreenFlow.FlowDefinitions
{
	public abstract class GreenFlowScreen : UiFlowScreen<SampleAppFlowType>
	{
		public override SampleAppFlowType IncludedInFlowType => SampleAppFlowType.GreenFlow;
	}
}