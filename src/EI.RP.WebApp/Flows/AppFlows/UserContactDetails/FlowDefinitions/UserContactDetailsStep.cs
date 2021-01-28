using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;

namespace EI.RP.WebApp.Flows.AppFlows.UserContactDetails.FlowDefinitions
{
    public static class UserContactDetailsStep
    {
        public static readonly ScreenName ShowFlowGenericError = new ScreenName(nameof(ShowFlowGenericError));
        public static readonly ScreenName UserContactDetails = new ScreenName(nameof(UserContactDetails));
        public static readonly ScreenName MarketingPreferences = new ScreenName(nameof(MarketingPreferences));
    }
}
