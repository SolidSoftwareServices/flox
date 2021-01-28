using S3.UiFlows.Core.Flows.Screens;


namespace S3.App.AspNetCore3_1.Flows.AppFlows.ContainersFlow2.FlowDefinitions
{
	public abstract class ContainersFlow2Screen : UiFlowContainerScreen<SampleAppFlowType>
	{
		public override SampleAppFlowType IncludedInFlowType => SampleAppFlowType.ContainersFlow2;

		
	}
}