using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;
using Microsoft.CodeAnalysis.Operations;

namespace EI.RP.WebApp.Flows.AppFlows.ContactUs.FlowDefinitions
{
    public static class ContactUsStep
    {
        public static readonly ScreenName ShowFlowGenericError = new ScreenName(nameof(ShowFlowGenericError));
        public static readonly ScreenName ContactUs = new ScreenName(nameof(ContactUs));
        public static readonly ScreenName ShowContactUsStatusMessage = new ScreenName(nameof(ShowContactUsStatusMessage));
    }
}
