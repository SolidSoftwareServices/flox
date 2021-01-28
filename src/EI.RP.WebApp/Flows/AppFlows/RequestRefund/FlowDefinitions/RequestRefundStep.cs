using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;

namespace EI.RP.WebApp.Flows.AppFlows.RequestRefund.FlowDefinitions
{
    public static class RequestRefundStep
	{
        public static readonly ScreenName ShowFlowGenericError = new ScreenName(nameof(ShowFlowGenericError));
        public static readonly ScreenName RequestRefund = new ScreenName(nameof(RequestRefund));
        public static readonly ScreenName ShowStatusMessage = new ScreenName(nameof(ShowStatusMessage));
    }
}
