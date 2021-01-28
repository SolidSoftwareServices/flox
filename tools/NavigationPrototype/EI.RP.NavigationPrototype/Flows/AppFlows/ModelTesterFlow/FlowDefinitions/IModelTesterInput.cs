using EI.RP.UiFlows.Core.Flows.Initialization;


namespace EI.RP.NavigationPrototype.Flows.AppFlows.ModelTesterFlow.FlowDefinitions
{
	public interface IModelTesterInput : IFlowInputContract
	{
		string Info { get; set; }
	}
}