namespace EI.RP.WebApp.Flows.AppFlows.CompetitionEntry.FlowDefinitions
{
    public abstract class CompetitionEntryScreen : ResidentialPortalScreen
    {
        public override ResidentialPortalFlowType IncludedInFlowType => ResidentialPortalFlowType.CompetitionEntry;

        protected virtual string Title => "Competition";
    }
}
