using System.Threading.Tasks;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Facade.TriggerEventOnView;
using S3.UiFlows.Core.Flows;
using S3.UiFlows.Core.Flows.Screens.Models;
using S3.UiFlows.Core.Flows.Screens.Models.DefaultModels;
using S3.UiFlows.Core.Flows.Screens.Models.Interop;

namespace S3.UiFlows.Core.Facade.FlowResultResolver
{
	class FlowResultResolverRequestHandler<TResult> : IFlowResultResolverRequestHandler<TResult>
	{
		private readonly IAppUiFlowsCollection _flows;
		private readonly IUiFlowContextRepository _contextRepository;

		public FlowResultResolverRequestHandler(IAppUiFlowsCollection flows, IUiFlowContextRepository contextRepository)
		{
			_flows = flows;
			_contextRepository = contextRepository;
		}

		public async Task<TResult> Execute(FlowResultResolverRequest<TResult> input)
		{
			return await ResolveEventResult(input);
		}

		private async Task<TResult> ResolveFlowActionResult(FlowResultResolverRequest<TResult> input)
		{
			TResult result = default(TResult);
			var screenModel = input.ScreenModel;
			result = await HandleIfOnConnectToFlow();
			if (result != null) return result;
			
			if (screenModel is ExitReturnToCaller)
			{
				var redirectTo = (ExitReturnToCaller)screenModel;
				var ctx = await _contextRepository.Get(redirectTo.CallbackFlowHandler);
				
				var interopData = (IStartFlowScreenModel)ctx.GetCurrentStepData<UiFlowScreenModel>();
				interopData.CallbackFromFlowHandler = redirectTo.FlowHandler;
				interopData.SetFlowResult(redirectTo.FlowResult);
				return await input.OnExecuteEvent(redirectTo.CallbackEvent, (UiFlowScreenModel)interopData);

			}
			if (screenModel is ExitRedirect)
			{
				var redirectTo = (ExitRedirect)screenModel;
				return await input.OnExecuteRedirection(redirectTo);

			}

			if (screenModel is UiFlowStepUnauthorized) return await input.OnExecuteUnauthorized((UiFlowStepUnauthorized)screenModel);

			//post-redirect-get
			var uiFlowContextData = await _contextRepository.Get(screenModel.FlowHandler);
			if (screenModel.Metadata.IsContainedInController() || uiFlowContextData.IsInContainer())
			{
				return await InputOnRedirectToRoot(input, uiFlowContextData);
			}

			return await input.OnRedirectToCurrent(uiFlowContextData.FlowType, screenModel.FlowHandler);


			async Task<TResult> HandleIfOnConnectToFlow()
			{
				TResult handleIfOnConnectToFlow = default(TResult);
				if (screenModel is IStartFlowScreenModel)
				{
					var ctx = await _contextRepository.Get(screenModel.FlowHandler);
					var redirectTo = (IStartFlowScreenModel)screenModel;
					if (redirectTo.AsContained)
					{
						redirectTo.SetContainedFlow(redirectTo.StartFlowType);
						handleIfOnConnectToFlow = input.OnNewContainedScreen(redirectTo.FlowHandler, ctx.FlowType, redirectTo.StartFlowType,
							redirectTo.StartDataAsObject());

					}
					else if (ctx.IsInContainer())
					{
						handleIfOnConnectToFlow = input.OnNewContainedScreen(ctx.ContainerFlowHandler, ctx.FlowType, redirectTo.StartFlowType,
							redirectTo.StartDataAsObject());

					}
					else
					{
						handleIfOnConnectToFlow = await input.OnStartNewFlow(redirectTo.StartFlowType, redirectTo.StartDataAsObject());

					}
				}

				return handleIfOnConnectToFlow;
			}


		}

		private async Task<TResult> ResolveEventResult(FlowResultResolverRequest<TResult> input)
		{
			var screenModel = input.ScreenModel;
			var flow = await _flows.GetByFlowHandler(screenModel.FlowHandler);
			
			var uiFlowStepData = await flow.GetCurrentStepData(screenModel.FlowHandler);

			var uiFlowContextData = await _contextRepository.Get(screenModel.FlowHandler);
			if (
				!uiFlowContextData.IsInContainer()
			    || uiFlowStepData is ExitRedirect
			    || uiFlowStepData is IStartFlowScreenModel
			    || uiFlowStepData is ExitReturnToCaller
			    || uiFlowStepData is UiFlowStepUnauthorized)
				return await ResolveFlowActionResult(input);


			//when is contained in flow
			return await InputOnRedirectToRoot(input, uiFlowContextData);
		}



		private async Task<TResult> InputOnRedirectToRoot(FlowResultResolverRequest<TResult> input, UiFlowContextData uiFlowContextData)
		{
			var root = await _contextRepository.GetRootContainerContext(uiFlowContextData);
			return await input.OnRedirectToRoot(root.FlowType, root.FlowHandler);
		}
	}
}