namespace EI.RP.WebApp.Flows.AppFlows.RequestRefund.FlowDefinitions
{
    public abstract class RequestRefundScreen : ResidentialPortalScreen
    {
        public override ResidentialPortalFlowType IncludedInFlowType => ResidentialPortalFlowType.RequestRefund;

        protected virtual string Title => "Request Refund";
    }
}
