using S3.UiFlows.Core.Flows.Screens;


namespace S3.App.AspNetCore3_1.Flows.AppFlows.ContainersFlow.FlowDefinitions
{
	public abstract class ContainersFlowScreen : UiFlowContainerScreen<SampleAppFlowType>
	{
		public override SampleAppFlowType IncludedInFlowType => SampleAppFlowType.ContainersFlow;

		
	}
}