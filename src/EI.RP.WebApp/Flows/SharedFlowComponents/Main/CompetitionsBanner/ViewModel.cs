using EI.RP.UiFlows.Mvc.Components;
using EI.RP.WebApp.Infrastructure.Flows;

namespace EI.RP.WebApp.Flows.SharedFlowComponents.Main.CompetitionsBanner
{
	public class ViewModel:FlowComponentViewModel
	{
		public string Answer { get; set; }
		public bool Visible { get; set; }
        public string AccountNumber { get; set; }
        public NavigationItem.FlowActionItem FlowAction { get; set; }
        public NavigationItem.FlowActionItem DismissBannerFlowAction { get; set; }
		public string Heading { get; set; }
		public string ImageDesktop { get; set; }
		public string ImageMobile { get; set; }
		public string ImageAltText { get; set; }
	}
}
