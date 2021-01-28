using EI.RP.UiFlows.Core.Flows.Initialization;


namespace EI.RP.NavigationPrototype.Flows.AppFlows.GreenFlow.FlowDefinitions
{
	public interface IGreenInput : IFlowInputContract
	{
		string SampleParameter { get; set; }
	}
}