namespace EI.RP.WebApp.Flows.AppFlows.SmartActivation.FlowDefinitions
{
	public class SmartActivationInput : ISmartActivationInput
    {
	    public ResidentialPortalFlowType SourceFlowType { get; set; }
	    public string AccountNumber { get; set; }
    }
}