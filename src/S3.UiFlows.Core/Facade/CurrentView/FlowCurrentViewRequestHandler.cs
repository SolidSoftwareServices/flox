using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows;
using S3.UiFlows.Core.Flows.Screens.Models;
using S3.UiFlows.Core.Flows.Screens.Models.Interop;

namespace S3.UiFlows.Core.Facade.CurrentView
{
	class FlowCurrentViewRequestHandler<TResult> : IFlowCurrentViewRequestHandler<TResult>
	{
		private readonly IAppUiFlowsCollection _flows;
		private readonly IUiFlowContextRepository _contextRepository;

		public FlowCurrentViewRequestHandler(IAppUiFlowsCollection flows, IUiFlowContextRepository contextRepository)
		{
			_flows = flows;
			_contextRepository = contextRepository;
		}

		public async Task<TResult> Execute(CurrentViewRequest<TResult> input)
		{
			TResult result;
			var flow = await _flows.GetByFlowHandler(input.FlowHandler);
			if (flow == null)
			{
				return await input.BuildResultOnFlowNotFound();
			}
			var stepData = await flow.GetCurrentStepData(input.FlowHandler, input.ViewParameters);
			UiFlowScreenModel actualStepData=null;
			//TODO: is component view property
			if (!input.ResolveAsComponentOnly)
			{
				actualStepData = await InitializeNewContainedFlows(stepData, input.ViewParameters);
			}

			if (actualStepData is ExitReturnToCaller)
			{
				result = await input.OnCallbackCallerFlow(((ExitReturnToCaller)actualStepData));
			}
			else if (actualStepData!=null && stepData.FlowHandler != actualStepData.FlowHandler)
			{
				result= await input.BuildResultOnRequestRedirectTo(actualStepData.FlowHandler);
			}
			else
			{
				if (input.OnAddModelError != null)
				{
					foreach (var error in stepData.Errors)
					{
						input.OnAddModelError(error.MemberName, error.ErrorMessage);
					}
				}

				result = await input.OnBuildView(new CurrentViewRequest<TResult>.BuildViewInput
				{
					BuildPartial = stepData.Metadata.IsContainedInController() ||
					               (await _contextRepository.Get(input.FlowHandler)).IsInContainer(),
					ScreenModel = stepData,
					ViewName = flow.CurrentState.ViewName,
					FlowType = flow.FlowTypeId

				});
				
			}
			if (!input.ResolveAsComponentOnly)
			{
				await _contextRepository.Flush();
			}

			return result;

		}

		async Task<UiFlowScreenModel> InitializeNewContainedFlows(UiFlowScreenModel sData,
			IDictionary<string, object> stepViewCustomizations)
		{
			bool refresh = false;

			var stepMetaData = sData.Metadata;
			while (stepMetaData.ContainedFlowType != null && stepMetaData.ContainedFlowHandler != null)
			{
				var ctx = await _contextRepository.Get(stepMetaData.ContainedFlowHandler);
				stepMetaData = ctx.GetCurrentStepData<UiFlowScreenModel>().Metadata;
				refresh = true;
			}

			while (stepMetaData.ContainedFlowType != null && stepMetaData.ContainedFlowHandler == null)
			{
				var c = _flows.GetByFlowType((await _contextRepository.Get(sData.FlowHandler)).FlowType);
				var uiFlowScreen = await c.GetCurrentScreen(sData.FlowHandler);
				IDictionary<string, object> containedFlowStartInfo = await uiFlowScreen.ResolveContainedFlowStartInfo(c.CurrentState.ContextData, stepViewCustomizations);

				var f = _flows.GetByFlowType(stepMetaData.ContainedFlowType);
				sData = await f.StartNew(sData.FlowHandler, containedFlowStartInfo);
				if (sData is ExitReturnToCaller)
				{
					return sData;
				}
				stepMetaData = sData.Metadata;
				refresh = true;
			}

			if (refresh)
			{
				var uiFlowContextData = (await _contextRepository.GetRootContainerContext(sData.FlowHandler));
				var flow = await _flows.GetByFlowHandler(uiFlowContextData.FlowHandler);
				sData = await flow.GetCurrentStepData(uiFlowContextData.FlowHandler, stepViewCustomizations);
			}

			return sData;
		}
	}
}