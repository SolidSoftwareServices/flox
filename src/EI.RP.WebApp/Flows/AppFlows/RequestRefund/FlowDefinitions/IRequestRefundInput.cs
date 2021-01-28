using EI.RP.UiFlows.Core.Flows.Initialization;


namespace EI.RP.WebApp.Flows.AppFlows.RequestRefund.FlowDefinitions
{
	public interface IRequestRefundInput : IFlowInputContract
	{
		string AccountNumber { get; set; }
	}
}