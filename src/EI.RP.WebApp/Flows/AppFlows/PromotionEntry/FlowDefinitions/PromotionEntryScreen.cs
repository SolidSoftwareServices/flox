namespace EI.RP.WebApp.Flows.AppFlows.PromotionEntry.FlowDefinitions
{
	public abstract class PromotionEntryScreen : ResidentialPortalScreen
    {
	    public override ResidentialPortalFlowType IncludedInFlowType => ResidentialPortalFlowType.PromotionEntry;

	    protected virtual string Title => "Promotion";
    }
}
