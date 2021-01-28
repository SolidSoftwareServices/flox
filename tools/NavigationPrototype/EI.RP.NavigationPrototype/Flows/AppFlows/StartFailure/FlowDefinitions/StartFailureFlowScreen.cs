using EI.RP.UiFlows.Core.Flows.Screens;


namespace EI.RP.NavigationPrototype.Flows.AppFlows.StartFailure.FlowDefinitions
{
	public abstract class StartFailureFlowScreen : UiFlowScreen<SampleAppFlowType>
	{
		public override SampleAppFlowType IncludedInFlowType => SampleAppFlowType.StartFailure;
	}
}