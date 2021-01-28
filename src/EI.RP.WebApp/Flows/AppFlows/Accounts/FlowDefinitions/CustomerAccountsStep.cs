using EI.RP.UiFlows.Core.Flows.Screens;

namespace EI.RP.WebApp.Flows.AppFlows.Accounts.FlowDefinitions
{
	public static class CustomerAccountsStep
	{
        public static readonly ScreenName AccountSelection = new ScreenName(nameof(AccountSelection));
        public static readonly ScreenName SubmitMeterReading = new ScreenName(nameof(SubmitMeterReading));
        public static readonly ScreenName LoadAccountDashboard = new ScreenName(nameof(LoadAccountDashboard));
		//TODO: name as previous using strictly-typed nameof
		public static readonly ScreenName ShowFlowGenericError = new ScreenName($"Accounts{nameof(ShowFlowGenericError)}");
		public static readonly ScreenName ShowCollectiveAccountError = new ScreenName($"Accounts{nameof(ShowCollectiveAccountError)}");
	}
}