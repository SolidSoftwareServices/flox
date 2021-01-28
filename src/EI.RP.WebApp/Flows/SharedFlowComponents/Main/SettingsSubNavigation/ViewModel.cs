using EI.RP.UiFlows.Mvc.Components;
using System.Collections.Generic;
using EI.RP.WebApp.Infrastructure.Flows;

namespace EI.RP.WebApp.Flows.SharedFlowComponents.Main.SettingsSubNavigation
{
	public class ViewModel : FlowComponentViewModel
	{
        public IEnumerable<NavigationItem> NavigationItems { get; set; } = new NavigationItem[0];
    }
}
