using EI.RP.UiFlows.Mvc.Components;
using EI.RP.WebApp.Infrastructure.Flows;

namespace EI.RP.WebApp.Flows.SharedFlowComponents.Main.PromotionsBanner
{
	public class ViewModel:FlowComponentViewModel
	{
		public bool Visible { get; set; }
		public NavigationItem.FlowActionItem FlowAction { get; set; }
		public NavigationItem.FlowActionItem DismissBannerFlowAction { get; set; }
		public string Heading { get; set; }
		public string ImageDesktop { get; set; }
		public string ImageMobile { get; set; }
	}
}
