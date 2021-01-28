using S3.UiFlows.Core.Flows.Screens;

namespace S3.App.AspNetCore3_1.Flows.AppFlows.ComponentsFlow.FlowDefinitions
{
	public abstract class ComponentsFlowScreen : UiFlowScreen<SampleAppFlowType>
	{
		public override SampleAppFlowType IncludedInFlowType => SampleAppFlowType.ComponentsFlow;
	}
}