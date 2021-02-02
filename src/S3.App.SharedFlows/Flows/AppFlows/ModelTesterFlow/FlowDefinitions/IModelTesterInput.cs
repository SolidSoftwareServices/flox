using S3.UiFlows.Core.Flows.Initialization;

namespace S3.App.Flows.AppFlows.ModelTesterFlow.FlowDefinitions
{
	public interface IModelTesterInput : IFlowInputContract
	{
		string Info { get; set; }
	}
}