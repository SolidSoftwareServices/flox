using EI.RP.UiFlows.Mvc.Components;
using System.Collections.Generic;
using EI.RP.WebApp.Infrastructure.Flows;

namespace EI.RP.WebApp.Flows.AppFlows.AccountDashboard.Components.AccountDashboardSubNavigation
{
	public class ViewModel : FlowComponentViewModel
	{
		public string AccountNumber { get; set; }
		public IEnumerable<NavigationItem> NavigationItems { get; set; }
	}
}