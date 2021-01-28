using S3.UiFlows.Core.Facade.CurrentView;
using S3.UiFlows.Core.Facade.Delegates;

namespace S3.UiFlows.Core.Facade
{
	public interface ICallbackHandler<TResult>
	{
		OnExecuteEventDelegate<TResult> OnExecuteEvent { get; set; }

		OnRedirectDelegate<TResult> OnExecuteRedirection { get; set; }


		OnExecuteUnauthorizedDelegate<TResult> OnExecuteUnauthorized { get; set; }


		OnRedirectToFlowDelegate<TResult> OnRedirectToRoot { get; set; }
		OnRedirectToFlowDelegate<TResult> OnRedirectToCurrent { get; set; }

		OnNewContainedScreenDelegate<TResult> OnNewContainedScreen { get; set; }
		//TODO: MOVE ALL DELEGATES OUTSIDE

		OnStartNewFlowDelegate<TResult> OnStartNewFlow { get; set; }

		AddModelErrorDelegate OnAddModelError { get; set; }
	}
}