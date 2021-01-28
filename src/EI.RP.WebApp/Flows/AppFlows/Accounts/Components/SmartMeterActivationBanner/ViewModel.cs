using EI.RP.UiFlows.Mvc.Components;
using EI.RP.WebApp.Infrastructure.Flows;
using System.Collections.Generic;

namespace EI.RP.WebApp.Flows.AppFlows.Accounts.Components.SmartMeterActivationBanner
{
	public class ViewModel : FlowComponentViewModel
	{
	    public bool CanOptToSmart { get; set; }

	    public List<NotificationViewModel> SmartActivationEligibleItems { get; set; } = new List<NotificationViewModel>();

	    public class NotificationViewModel
	    {
		    public string Mprn { get; set; }
		    public string AccountNumber { get; set; }
		    public NavigationItem.FlowActionItem FlowAction { get; set; }
		    public NavigationItem.FlowActionItem DismissNotificationAction { get; set; }
		}
	}
}
