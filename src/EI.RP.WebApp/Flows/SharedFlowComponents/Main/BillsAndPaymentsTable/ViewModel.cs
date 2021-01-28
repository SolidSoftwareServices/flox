using EI.RP.UiFlows.Mvc.Components;
using EI.RP.WebApp.Flows.AppFlows;

namespace EI.RP.WebApp.Flows.SharedFlowComponents.Main.BillsAndPaymentsTable
{
	public class ViewModel : FlowPageableComponentViewModel<ViewModel.Row>
	{
		public string AccountNumber { get; set; }

		public ResidentialPortalFlowType ContainedInFlowType { get; set; }

		public string TableId { get; set; }
		public string PaginationId { get; set; }
		public string WhenChangingPageFocusOn { get; set; }

		public bool IsAccountClosed { get; set; }

		public class Row
		{
			public string Date { get; set; }
			public string Description { get; set; }
			public string DueAmount { get; set; }
			public string ReceivedAmount { get; set; }
			public bool HasInvoiceFile { get; set; }
			public string ReferenceDocNumber { get; set; }
		}
	}
}
