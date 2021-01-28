using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.WebApp.Flows.AppFlows.BusinessPartnersSearch.Steps;

namespace EI.RP.WebApp.Flows.AppFlows.BusinessPartnersSearch.FlowDefinitions
{
	public static class BusinessPartnersSearchStep
	{
		public static readonly ScreenName ShowFlowGenericError = new ScreenName(nameof(ShowFlowGenericError));

		public static readonly ScreenName SearchAndShowResultsStep = new ScreenName(nameof(SearchAndShowResultsStep));
		public static readonly ScreenName SelectPartnerAndConnectToAccountsFlow = new ScreenName(nameof(SelectPartnerAndConnectToAccountsFlow));
    }
}