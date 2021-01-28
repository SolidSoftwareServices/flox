

namespace EI.RP.WebApp.Flows.AppFlows.ChangePassword.FlowDefinitions
{
	public abstract class ChangePasswordScreen : ResidentialPortalScreen
	{
		public override ResidentialPortalFlowType IncludedInFlowType => ResidentialPortalFlowType.ChangePassword;

		protected virtual string Title => "Change Password";
	}
}