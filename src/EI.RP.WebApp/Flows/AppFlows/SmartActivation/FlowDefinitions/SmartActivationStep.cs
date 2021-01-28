using EI.RP.UiFlows.Core.Flows.Screens;

namespace EI.RP.WebApp.Flows.AppFlows.SmartActivation.FlowDefinitions
{
	public static class SmartActivationStep
    {
        public static readonly ScreenName ShowFlowGenericError = new ScreenName(nameof(ShowFlowGenericError));
        public static readonly ScreenName Step1EnableSmartFeatures = new ScreenName(nameof(Step1EnableSmartFeatures));
		public static readonly ScreenName Step2SelectPlan = new ScreenName(nameof(Step2SelectPlan));
		public static readonly ScreenName Step3PaymentDetails = new ScreenName(nameof(Step3PaymentDetails));
		public static readonly ScreenName Step4BillingFrequency = new ScreenName(nameof(Step4BillingFrequency));
		public static readonly ScreenName Step5Summary = new ScreenName(nameof(Step5Summary));
		public static readonly ScreenName Confirmation = new ScreenName(nameof(Confirmation));
	}
}