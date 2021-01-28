using S3.CoreServices.System.Paging;

namespace S3.UiFlows.Mvc.Components
{
	public abstract class FlowPageableComponentViewModel<TRow>:FlowComponentViewModel
	{
		public bool IsPagingEnabled { get; set; }
		public int NumberOfPageLinks { get; set; }
		public object RouteValues { get; set; }

		public PagedData<TRow> Paging { get; set; }= new PagedData<TRow>();
	}
}