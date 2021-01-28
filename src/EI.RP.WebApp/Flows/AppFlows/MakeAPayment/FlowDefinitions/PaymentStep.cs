using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;

namespace EI.RP.WebApp.Flows.AppFlows.MakeAPayment.FlowDefinitions
{
    public static class PaymentStep
    {
        public static readonly ScreenName ShowFlowGenericError = new ScreenName(nameof(ShowFlowGenericError));

        public static readonly ScreenName PaymentGatewayInteractionAndMainOptions = new ScreenName(nameof(PaymentGatewayInteractionAndMainOptions));
        public static readonly ScreenName ChangePaymentAmount = new ScreenName(nameof(ChangePaymentAmount));

    }
}
