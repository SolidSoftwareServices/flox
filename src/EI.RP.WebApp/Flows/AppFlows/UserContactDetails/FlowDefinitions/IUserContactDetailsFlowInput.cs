using EI.RP.UiFlows.Core.Flows.Initialization;


namespace EI.RP.WebApp.Flows.AppFlows.UserContactDetails.FlowDefinitions
{
	public interface IUserContactDetailsFlowInput : IFlowInputContract
	{
		UserContactFlowType InitialFlowStartType { get; set; }
		string AccountNumber { get; set; }
	}
}