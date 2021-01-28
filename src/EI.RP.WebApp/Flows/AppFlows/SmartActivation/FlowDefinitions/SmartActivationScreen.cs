namespace EI.RP.WebApp.Flows.AppFlows.SmartActivation.FlowDefinitions
{
    /// <summary>
    ///Base class every screen in (SmartActivation) 
    /// </summary>
    public abstract class SmartActivationScreen : ResidentialPortalScreen
    {
        public override ResidentialPortalFlowType IncludedInFlowType => ResidentialPortalFlowType.SmartActivation;

        protected virtual string Title => "Smart sign up";
    }
}