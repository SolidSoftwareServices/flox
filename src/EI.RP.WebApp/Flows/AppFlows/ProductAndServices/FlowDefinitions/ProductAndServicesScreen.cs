
namespace EI.RP.WebApp.Flows.AppFlows.ProductAndServices.FlowDefinitions
{
    public abstract class ProductAndServicesScreen : ResidentialPortalScreen
    {
        public override ResidentialPortalFlowType IncludedInFlowType => ResidentialPortalFlowType.ProductAndServices;

        protected virtual string Title => "Products & Services";
    }
}
