﻿using EI.RP.UiFlows.Mvc.Components;
using System.Collections.Generic;
using EI.RP.WebApp.Infrastructure.Flows;

namespace EI.RP.WebApp.Flows.AppFlows.Accounts.Components.CollectiveAccountMainNavigation
{
	public class ViewModel : FlowComponentViewModel
	{
		public IEnumerable<NavigationItem> NavigationItems { get; set; }
        public IEnumerable<NavigationItem> SettingsItems { get; set; }
	}
}