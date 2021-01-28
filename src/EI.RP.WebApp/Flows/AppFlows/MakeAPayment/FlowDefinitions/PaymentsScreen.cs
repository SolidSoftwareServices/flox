

namespace EI.RP.WebApp.Flows.AppFlows.MakeAPayment.FlowDefinitions
{
    public abstract class PaymentsScreen : ResidentialPortalScreen
    {
        public override ResidentialPortalFlowType IncludedInFlowType => ResidentialPortalFlowType.MakeAPayment;

        protected virtual string Title => "Payments";
    }
}
