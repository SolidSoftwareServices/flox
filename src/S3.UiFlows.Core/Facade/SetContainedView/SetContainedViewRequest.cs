using System.Dynamic;
using S3.UiFlows.Core.Facade.Delegates;

namespace S3.UiFlows.Core.Facade.SetContainedView
{
	public class SetContainedViewRequest<TResult>:ICallbackHandler<TResult>
	{
		public string FlowHandler { get; set; }
		public OnExecuteEventDelegate<TResult> OnExecuteEvent { get; set; }
		public OnRedirectDelegate<TResult> OnExecuteRedirection { get; set; }
		public OnExecuteUnauthorizedDelegate<TResult> OnExecuteUnauthorized { get; set; }
		public OnRedirectToFlowDelegate<TResult> OnRedirectToRoot { get; set; }
		public OnRedirectToFlowDelegate<TResult> OnRedirectToCurrent { get; set; }
		public OnNewContainedScreenDelegate<TResult> OnNewContainedScreen { get; set; }
		public OnStartNewFlowDelegate<TResult> OnStartNewFlow { get; set; }
		public AddModelErrorDelegate OnAddModelError { get; set; }
		public ExpandoObject ViewCustomizations { get; set; }
		public string NewContainedFlowType { get; set; }
		public string NewContainedFlowStartType { get; set; }
	}
}
