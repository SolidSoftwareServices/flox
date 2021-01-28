namespace EI.RP.UiFlows.Core.Flows.Runtime
{
	internal interface IFlowRuntimeInfoResolver
	{
		FlowRuntimeInfo GetFlowInfo(string flowType);

	}
}