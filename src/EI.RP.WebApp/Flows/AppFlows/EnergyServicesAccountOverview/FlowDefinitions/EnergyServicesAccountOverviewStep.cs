using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;

namespace EI.RP.WebApp.Flows.AppFlows.EnergyServicesAccountOverview.FlowDefinitions
{
	public static class EnergyServicesAccountOverviewStep
    {
		public static readonly ScreenName ShowFlowGenericError = new ScreenName(nameof(ShowFlowGenericError));
        public static readonly ScreenName EnergyServiceAccountOverviewDefault = new ScreenName(nameof(EnergyServiceAccountOverviewDefault));
    }
}