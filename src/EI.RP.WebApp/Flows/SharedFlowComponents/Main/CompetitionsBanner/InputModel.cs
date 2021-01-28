using EI.RP.UiFlows.Core.Flows.Screens;

namespace EI.RP.WebApp.Flows.SharedFlowComponents.Main.CompetitionsBanner
{
	public class InputModel
	{
        public string AccountNumber { get; set; }
		public ScreenEvent ToCompetitionEvent { get; set; }
		public ScreenEvent DismissBannerEvent { get; set; }
	}
}
