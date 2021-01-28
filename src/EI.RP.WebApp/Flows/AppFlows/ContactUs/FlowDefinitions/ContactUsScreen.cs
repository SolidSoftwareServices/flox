

namespace EI.RP.WebApp.Flows.AppFlows.ContactUs.FlowDefinitions
{
    public abstract class ContactUsScreen : ResidentialPortalScreen
    {
        public override ResidentialPortalFlowType IncludedInFlowType => ResidentialPortalFlowType.ContactUs;

        protected virtual string Title => "Contact Us";
    }
}
