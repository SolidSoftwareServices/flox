using EI.RP.UiFlows.Core.Flows.Screens;

namespace EI.RP.WebApp.Flows.AppFlows.MeterReadings.FlowDefinitions
{
    public static class MeterReadingStep
    {
        public static readonly ScreenName ShowFlowGenericError = new ScreenName(nameof(ShowFlowGenericError));
        public static readonly ScreenName SubmitMeterReading = new ScreenName(nameof(SubmitMeterReading));
        public static readonly ScreenName ShowMeterReadingStatus = new ScreenName(nameof(ShowMeterReadingStatus));
        public static readonly ScreenName MeterReadingNotPresent = new ScreenName(nameof(MeterReadingNotPresent));
        public static readonly ScreenName ErrorSubmittingMeterReadingStep = new ScreenName(nameof(ErrorSubmittingMeterReadingStep));
		public static readonly ScreenName ErrorSubmittedLessThanActual = new ScreenName(nameof(ErrorSubmittedLessThanActual));
	}
}
