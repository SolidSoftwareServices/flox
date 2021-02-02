using S3.UiFlows.Core.Flows.Screens;

namespace S3.App.Flows.AppFlows.ModelTesterFlow.FlowDefinitions
{
	public abstract class ModelTesterFlowScreen : UiFlowScreen<SampleAppFlowType>
	{
		public override SampleAppFlowType IncludedInFlowType => SampleAppFlowType.ModelTesterFlow;
	}
}