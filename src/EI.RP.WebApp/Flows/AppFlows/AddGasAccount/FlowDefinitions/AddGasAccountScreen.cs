namespace EI.RP.WebApp.Flows.AppFlows.AddGasAccount.FlowDefinitions
{
    public abstract class AddGasAccountScreen : ResidentialPortalScreen
    {
        public override ResidentialPortalFlowType IncludedInFlowType => ResidentialPortalFlowType.AddGasAccount;

        protected virtual string Title => "Add Gas";
    }
}
