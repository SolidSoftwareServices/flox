using S3.UiFlows.Core.Flows.Initialization;

namespace S3.App.Flows.AppFlows.GreenFlow.FlowDefinitions
{
	public interface IGreenInput : IFlowInputContract
	{
		string SampleParameter { get; set; }
	}
}