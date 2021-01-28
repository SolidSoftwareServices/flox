using S3.UiFlows.Core.Flows.Screens;


namespace S3.App.AspNetCore3_1.Flows.AppFlows.BlueFlow.FlowDefinitions
{
	public abstract class BlueFlowScreen : UiFlowScreen<SampleAppFlowType>
	{
		public override SampleAppFlowType IncludedInFlowType => SampleAppFlowType.BlueFlow;
	}
}