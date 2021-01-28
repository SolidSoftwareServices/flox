namespace EI.RP.WebApp.Flows.AppFlows.MovingHouse.FlowDefinitions
{
	public abstract class MovingHouseScreen : ResidentialPortalScreen
	{
		public override ResidentialPortalFlowType IncludedInFlowType => ResidentialPortalFlowType.MovingHouse;

		protected virtual string Title => "Moving House";
	}
}