using S3.UiFlows.Core.Flows.Screens;

namespace S3.App.Flows.AppFlows.ContainersFlow3.FlowDefinitions
{
	public abstract class ContainersFlow3Screen : UiFlowContainerScreen<SampleAppFlowType>
	{
		public override SampleAppFlowType IncludedInFlowType => SampleAppFlowType.ContainersFlow3;

		
	}
}