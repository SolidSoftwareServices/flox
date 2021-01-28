using S3.UiFlows.Core.Flows.Screens;


namespace S3.App.AspNetCore3_1.Flows.AppFlows.ContainersFlow4.FlowDefinitions
{
	public abstract class ContainersFlow4Screen : UiFlowContainerScreen<SampleAppFlowType>
	{
		public override SampleAppFlowType IncludedInFlowType => SampleAppFlowType.ContainersFlow4;

		
	}
}