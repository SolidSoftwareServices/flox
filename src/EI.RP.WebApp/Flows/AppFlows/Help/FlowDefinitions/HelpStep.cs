using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;

namespace EI.RP.WebApp.Flows.AppFlows.Help.FlowDefinitions
{
	public static class HelpStep
    {
		public static readonly ScreenName ShowFlowGenericError = new ScreenName(nameof(ShowFlowGenericError));
        public static  readonly ScreenName Help = new ScreenName(nameof(Help));
    }
}