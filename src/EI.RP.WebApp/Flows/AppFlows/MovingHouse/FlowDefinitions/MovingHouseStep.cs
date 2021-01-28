using EI.RP.UiFlows.Core.Flows.Screens;

namespace EI.RP.WebApp.Flows.AppFlows.MovingHouse.FlowDefinitions
{
    public static class MovingHouseStep
    {
        public static readonly ScreenName ShowFlowGenericError = new ScreenName(nameof(ShowFlowGenericError));

        public static readonly ScreenName ShowMovingHouseReEnterPrnError = new ScreenName(nameof(ShowMovingHouseReEnterPrnError));
        public static readonly ScreenName ShowMovingHouseUnhandledError = new ScreenName(nameof(ShowMovingHouseUnhandledError));
		public static readonly ScreenName ShowMovingHouseValidationError = new ScreenName(nameof(ShowMovingHouseValidationError));
        public static readonly ScreenName MovingHouseNotPresent = new ScreenName(nameof(MovingHouseNotPresent));

		//USE THE FOLLOWING FOR EACH STEP!!!!
		public static readonly ScreenName Step0OperationSelection = new ScreenName(nameof(Step0OperationSelection));
		public static readonly ScreenName Step1InputMoveOutPropertyDetails = new ScreenName(nameof(Step1InputMoveOutPropertyDetails));
        public static readonly ScreenName Step2InputPrns = new ScreenName(nameof(Step2InputPrns));
        public static readonly ScreenName Step2ConfirmAddress = new ScreenName(nameof(Step2ConfirmAddress));
        public static readonly ScreenName Step3InputMoveInPropertyDetails = new ScreenName(nameof(Step3InputMoveInPropertyDetails));
		public static readonly ScreenName Step4ConfigurePayment = new ScreenName(nameof(Step4ConfigurePayment));
		public static readonly ScreenName Step5ReviewAndComplete = new ScreenName(nameof(Step5ReviewAndComplete));
		public static readonly ScreenName Step5ReviewConfirmation = new ScreenName(nameof(Step5ReviewConfirmation));
		public static readonly ScreenName StepCloseAccounts = new ScreenName(nameof(StepCloseAccounts));
        public static readonly ScreenName CloseAccountConfirmation = new ScreenName(nameof(CloseAccountConfirmation));
    }
}