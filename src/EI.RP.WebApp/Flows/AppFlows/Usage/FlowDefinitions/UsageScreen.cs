namespace EI.RP.WebApp.Flows.AppFlows.Usage.FlowDefinitions
{
    public abstract class UsageScreen : ResidentialPortalScreen
    {
        public override ResidentialPortalFlowType IncludedInFlowType => ResidentialPortalFlowType.Usage;

        protected virtual string Title => "Usage";
    }
}
