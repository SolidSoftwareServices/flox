using EI.RP.WebApp.Flows.AppFlows;

namespace EI.RP.WebApp.Flows.SharedFlowComponents.Main.MeterReadingHistory
{
	public class InputModel
	{
		public string AccountNumber { get; set; }
		public ResidentialPortalFlowType ContainedInFlowType { get; set; }

		public int PageIndex { get; set; } = 0;
		public int PageSize { get; set; } = int.MaxValue;
		public int NumberPagingLinks { get; set; } = 4;
		public bool IsPagingEnabled { get; set; } = false;

		public string TableId { get; set; } = "meter-reading-history-table";
		public string PaginationId { get; set; } = "meter-reading-history-pagination";
		public string WhenChangingPageFocusOn { get; set; }
	}
}
