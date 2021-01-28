using S3.UiFlows.Core.Facade.Delegates;
using S3.UiFlows.Core.Facade.FlowResultResolver;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;

namespace S3.UiFlows.Core.Facade.TriggerEventOnView
{
	public class TriggerEventOnView<TResult> : ICallbackHandler<TResult>
	{

		public UiFlowScreenModel Model { get; set; }

		public OnExecuteEventDelegate<TResult> OnExecuteEvent { get; set; }

		public OnRedirectDelegate<TResult> OnExecuteRedirection { get; set; }


		public OnExecuteUnauthorizedDelegate<TResult> OnExecuteUnauthorized { get; set; }


		public OnRedirectToFlowDelegate<TResult> OnRedirectToRoot { get; set; }
		public OnRedirectToFlowDelegate<TResult> OnRedirectToCurrent { get; set; }

		public OnNewContainedScreenDelegate<TResult> OnNewContainedScreen { get; set; }
		//TODO: MOVE ALL DELEGATES OUTSIDE

		public OnStartNewFlowDelegate<TResult> OnStartNewFlow { get; set; }

		

		public ScreenEvent Event { get; set; }


		public AddModelErrorDelegate OnAddModelError { get; set; }

		public FlowResultResolverRequest<TResult> ToFlowResolverRequest(UiFlowScreenModel model)
		{
			return new FlowResultResolverRequest<TResult>
			{
				ScreenModel = model ?? Model,
				OnExecuteEvent = this.OnExecuteEvent,
				OnExecuteRedirection = OnExecuteRedirection,
				OnExecuteUnauthorized = OnExecuteUnauthorized,
				OnRedirectToCurrent = OnRedirectToCurrent,
				OnNewContainedScreen = OnNewContainedScreen,
				OnStartNewFlow = OnStartNewFlow,
				OnRedirectToRoot = OnRedirectToRoot,

			};
		}

		public OnExecuteValidationDelegate OnExecuteValidation { get; set; }
	}
}
