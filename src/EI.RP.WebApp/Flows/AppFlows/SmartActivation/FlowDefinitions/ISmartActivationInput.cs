using EI.RP.UiFlows.Core.Flows.Initialization;

namespace EI.RP.WebApp.Flows.AppFlows.SmartActivation.FlowDefinitions
{
    public interface ISmartActivationInput : IFlowInputContract
    {
		ResidentialPortalFlowType SourceFlowType { get; set; }
		string AccountNumber { get; set; }
	}
}