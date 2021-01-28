using S3.UiFlows.Core.Flows.Initialization;


namespace S3.App.AspNetCore3_1.Flows.AppFlows.BlueFlow.FlowDefinitions
{
	public interface IBlueInput : IFlowInputContract
	{
		string GreenFlowData { get; set; }
	}
}