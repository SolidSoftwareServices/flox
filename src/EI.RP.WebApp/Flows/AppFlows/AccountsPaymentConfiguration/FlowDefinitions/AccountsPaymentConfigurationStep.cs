using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;

namespace EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.FlowDefinitions
{
	public static class AccountsPaymentConfigurationStep
    {
		public static readonly ScreenName ShowFlowGenericError = new ScreenName("ShowFlowGenericError");
		public static readonly ScreenName SetupDirectDebitWithPaymentOptions = new ScreenName($"DD{nameof(SetupDirectDebitWithPaymentOptions)}");
        public static readonly ScreenName InputDirectDebitDetailsStep = new ScreenName($"DD{nameof(InputDirectDebitDetailsStep)}");
        public static readonly ScreenName ConfirmationOfChangesStep = new ScreenName($"DD{nameof(ConfirmationOfChangesStep)}");
        public static readonly ScreenName ShowPaymentsHistory = new ScreenName(nameof(ShowPaymentsHistory));
        public static readonly ScreenName ShowAccountCostEstimation = new ScreenName($"DD{nameof(ShowAccountCostEstimation)}");
        public static readonly ScreenName EqualizerMonthlyPayments = new ScreenName($"E{nameof(EqualizerMonthlyPayments)}");
        public static readonly ScreenName SetUpEqualizerMonthlyPayments = new ScreenName(nameof(SetUpEqualizerMonthlyPayments));


        public static readonly ScreenName ChoosePaymentOption = new ScreenName(nameof(ChoosePaymentOption));
        public static readonly ScreenName CompleteFlowAndReturnToCaller = new ScreenName(nameof(CompleteFlowAndReturnToCaller));
    }
}