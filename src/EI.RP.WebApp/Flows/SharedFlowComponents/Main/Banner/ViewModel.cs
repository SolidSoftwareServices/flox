using EI.RP.UiFlows.Mvc.Components;
using EI.RP.WebApp.Infrastructure.Flows;

namespace EI.RP.WebApp.Flows.SharedFlowComponents.Main.Banner
{
	public class ViewModel : FlowComponentViewModel
	{
        public string ContainerTitle { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public NavigationItem.AnchorLinkItem AnchorLink { get; set; }
        public string ImagePath { get; set; }
	}
}
