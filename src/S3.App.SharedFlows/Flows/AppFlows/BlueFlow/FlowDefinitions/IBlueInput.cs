using S3.UiFlows.Core.Flows.Initialization;

namespace S3.App.Flows.AppFlows.BlueFlow.FlowDefinitions
{
	public interface IBlueInput : IFlowInputContract
	{
		string GreenFlowData { get; set; }
	}
}