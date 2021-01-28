using S3.UiFlows.Core.Facade.Delegates;
using S3.UiFlows.Core.Flows.Screens.Models;

namespace S3.UiFlows.Core.Facade.FlowResultResolver
{

	public class FlowResultResolverRequest<TResult>:ICallbackHandler<TResult>
	{
		
		public UiFlowScreenModel ScreenModel { get; set; }


		public OnExecuteEventDelegate<TResult> OnExecuteEvent { get; set; }
		public OnRedirectDelegate<TResult> OnExecuteRedirection { get; set; }
		public OnExecuteUnauthorizedDelegate<TResult> OnExecuteUnauthorized { get; set; }

		public OnRedirectToFlowDelegate<TResult> OnRedirectToCurrent { get; set; }
		public OnNewContainedScreenDelegate<TResult> OnNewContainedScreen { get; set; }
		public OnStartNewFlowDelegate<TResult> OnStartNewFlow { get; set; }

		public OnRedirectToFlowDelegate<TResult> OnRedirectToRoot { get; set; }
		public AddModelErrorDelegate OnAddModelError { get; set; }

	}



}
