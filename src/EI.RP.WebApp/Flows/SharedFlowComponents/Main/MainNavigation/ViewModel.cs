using System.Collections.Generic;
using EI.RP.UiFlows.Mvc.Components;
using EI.RP.WebApp.Infrastructure.Flows;

namespace EI.RP.WebApp.Flows.SharedFlowComponents.Main.MainNavigation
{
	public class ViewModel : FlowComponentViewModel
	{
        public string SettingsLabel { get; set; }
        public IEnumerable<NavigationItem> NavigationItems { get; set; }
        public IEnumerable<NavigationItem> SettingsItems { get; set; }
        public bool IsAgentUser { get; set; }
	}
}
