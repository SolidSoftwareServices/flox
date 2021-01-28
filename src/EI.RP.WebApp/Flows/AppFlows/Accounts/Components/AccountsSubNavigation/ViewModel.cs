using EI.RP.UiFlows.Mvc.Components;
using System.Collections.Generic;
using EI.RP.WebApp.Infrastructure.Flows;

namespace EI.RP.WebApp.Flows.AppFlows.Accounts.Components.AccountsSubNavigation
{
	public class ViewModel : FlowComponentViewModel
	{
        public bool IsMultiPageView { get; set; }
        public IEnumerable<NavigationItem> NavigationItems { get; set; } = new NavigationItem[0];
    }
}