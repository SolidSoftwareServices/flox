using S3.UiFlows.Core.Flows.Screens;

namespace S3.App.Flows.AppFlows.GreenFlow.FlowDefinitions
{
	public abstract class GreenFlowScreen : UiFlowScreen<SampleAppFlowType>
	{
		public override SampleAppFlowType IncludedInFlowType => SampleAppFlowType.GreenFlow;
	}
}