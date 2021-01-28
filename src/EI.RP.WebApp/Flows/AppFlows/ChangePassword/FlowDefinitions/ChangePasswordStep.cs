using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;

namespace EI.RP.WebApp.Flows.AppFlows.ChangePassword.FlowDefinitions
{
	public static class ChangePasswordStep
    {
		public static readonly ScreenName ShowFlowGenericError = new ScreenName(nameof(ShowFlowGenericError));
        public static  readonly ScreenName ChangePassword = new ScreenName(nameof(ChangePassword));
    }
}