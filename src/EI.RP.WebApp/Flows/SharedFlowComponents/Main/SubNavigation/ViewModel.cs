using EI.RP.UiFlows.Mvc.Components;
using EI.RP.WebApp.Flows.AppFlows;
using System.Collections.Generic;
using EI.RP.WebApp.Infrastructure.Flows;

namespace EI.RP.WebApp.Flows.SharedFlowComponents.Main.SubNavigation
{
	public class ViewModel : FlowComponentViewModel
	{
        public ResidentialPortalFlowType? CurrentFlowType { get; set; }
		public IEnumerable<NavigationItem> NavigationItems { get; set; } = new NavigationItem[0];
	}
}