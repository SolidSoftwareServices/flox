using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;

namespace EI.RP.WebApp.Flows.AppFlows.AccountAndMeterDetails.FlowDefinitions
{
	public static class AccountAndMeterDetailsStep
    {
		public static readonly ScreenName ShowFlowGenericError = new ScreenName("ShowFlowGenericError");
        public static readonly ScreenName ShowAccountAndMeterDetails = new ScreenName(nameof(ShowAccountAndMeterDetails));
    }
}