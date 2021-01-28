namespace EI.RP.WebApp.Flows.AppFlows.UserContactDetails.FlowDefinitions
{
    public abstract class UserContactDetailsScreen:ResidentialPortalScreen
    {
        public override ResidentialPortalFlowType IncludedInFlowType => ResidentialPortalFlowType.UserContactDetails;

        protected virtual string Title => "My Details";
    }
}
