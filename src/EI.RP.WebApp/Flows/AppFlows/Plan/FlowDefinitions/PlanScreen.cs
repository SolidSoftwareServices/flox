namespace EI.RP.WebApp.Flows.AppFlows.Plan.FlowDefinitions
{
    /// <summary>
    ///Base class every screen in (Plan) 
    /// </summary>
    public abstract class PlanScreen : ResidentialPortalScreen
    {
        public override ResidentialPortalFlowType IncludedInFlowType => ResidentialPortalFlowType.Plan;

        protected virtual string Title => "Plan";
    }
}