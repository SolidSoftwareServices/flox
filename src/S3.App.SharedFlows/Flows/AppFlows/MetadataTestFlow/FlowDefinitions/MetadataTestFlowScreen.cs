using S3.UiFlows.Core.Flows.Screens;

namespace S3.App.Flows.AppFlows.MetadataTestFlow.FlowDefinitions
{
	public abstract class MetadataTestFlowScreen : UiFlowScreen<SampleAppFlowType>
	{
		public override SampleAppFlowType IncludedInFlowType => SampleAppFlowType.MetadataTestFlow;
	}
}