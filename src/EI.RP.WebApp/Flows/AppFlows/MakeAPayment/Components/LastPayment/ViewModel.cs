
using EI.RP.CoreServices.System;
using System;
using EI.RP.UiFlows.Mvc.Components;

namespace EI.RP.WebApp.Flows.AppFlows.MakeAPayment.Components.LastPayment
{
	public class ViewModel : FlowComponentViewModel
	{
		public string CurrentBalanceAmountLabel { get; set; }
		public EuroMoney CurrentBalanceAmount { get; set; }

		public DateTime? LatestBillDate { get; set; }
		public DateTime? NextBillDate { get; set; }
		public DateTime? DueDate { get; set; }

		public bool ShowPayDifferentAmount { get; set; } = true;
		public bool ShowDatesTable { get; set; } = true;
	}
}