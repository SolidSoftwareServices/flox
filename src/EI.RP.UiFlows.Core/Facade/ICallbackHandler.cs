using EI.RP.UiFlows.Core.Facade.CurrentView;
using EI.RP.UiFlows.Core.Facade.Delegates;

namespace EI.RP.UiFlows.Core.Facade
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