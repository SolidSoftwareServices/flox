using EI.RP.CoreServices.System;
using System;

namespace EI.RP.WebApp.Flows.AppFlows.MakeAPayment.Components.LastPayment
{
	public class InputModel
	{
		public string AccountNumber { get; set; }

		public string CurrentBalanceAmountLabel { get; set; } = null;
		public EuroMoney CurrentBalanceAmount { get; set; } = null;

		public bool ShowLatestBillDate { get; set; } = true;
		public DateTime? LatestBillDate { get; set; }

		public bool ShowNextBillDate { get; set; } = true;
		public DateTime? NextBillDate { get; set; }

		public bool ShowDueDate { get; set; } = true;
		public DateTime? DueDate { get; set; }

		public bool ShowPayDifferentAmount { get; set; } = true;
		public bool ShowDatesTable { get; set; } = true;
	}
}
