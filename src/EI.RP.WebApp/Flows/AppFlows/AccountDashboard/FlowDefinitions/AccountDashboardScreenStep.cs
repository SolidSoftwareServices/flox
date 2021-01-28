using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;

namespace EI.RP.WebApp.Flows.AppFlows.AccountDashboard.FlowDefinitions
{
	public static class AccountDashboardScreenStep
	{
        public static readonly ScreenName MyAccountDashboard = new ScreenName(nameof(MyAccountDashboard));
		//TODO: name as previous using strictly-typed nameof
		public static readonly ScreenName ShowFlowGenericError = new ScreenName(nameof(ShowFlowGenericError));
    }
}