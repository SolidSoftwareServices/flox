using EI.RP.UiFlows.Core.Flows.Initialization;


namespace EI.RP.NavigationPrototype.Flows.AppFlows.BlueFlow.FlowDefinitions
{
	public interface IBlueInput : IFlowInputContract
	{
		string GreenFlowData { get; set; }
	}
}