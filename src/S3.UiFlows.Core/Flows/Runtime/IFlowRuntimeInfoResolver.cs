namespace S3.UiFlows.Core.Flows.Runtime
{
	internal interface IFlowRuntimeInfoResolver
	{
		FlowRuntimeInfo GetFlowInfo(string flowType);

	}
}