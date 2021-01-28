using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Facade.FlowResultResolver;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens.Models.DefaultModels;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;

namespace EI.RP.UiFlows.Core.Facade.SetContainedView
{
	class SetContainedViewRequestHandler<TResult> : ISetContainedViewRequestHandler<TResult>
	{
		private readonly IFlowResultResolverRequestHandler<TResult> _flowResultResolver;
		private readonly IAppUiFlowsCollection _flows;
		private readonly IUiFlowContextRepository _contextRepository;

		public SetContainedViewRequestHandler(IFlowResultResolverRequestHandler<TResult> flowResultResolver, IAppUiFlowsCollection flows,IUiFlowContextRepository contextRepository)
		{
			_flowResultResolver = flowResultResolver;
			_flows = flows;
			_contextRepository = contextRepository;
		}

		public async Task<TResult> Execute(SetContainedViewRequest<TResult> input)
		{
			var ctx=await _contextRepository.CreateSnapshotOfContext(await _contextRepository.Get(input.FlowHandler));

			var newFlowHandler = (await _contextRepository.Get(input.FlowHandler)).NextFlowHandler;

			var flow = await _flows.GetByFlowHandler(newFlowHandler);
			if (flow == null)
			{
				return await NotFoundResult(input);
			}
			var stepData = await flow.GetCurrentStepData(newFlowHandler,
				stepViewCustomizations: input.ViewCustomizations);

			//update contained step data
			var stepMetaData = stepData.Metadata;
			stepMetaData.ContainedFlowType = input.NewContainedFlowType;
			stepMetaData.ContainedFlowHandler = null;
			stepMetaData.ContainedFlowStartType = input.NewContainedFlowStartType;

			await flow.SetCurrentStepData(stepData);

			var result = await input.OnRedirectToCurrent(flow.FlowTypeId, newFlowHandler);
			await _contextRepository.Flush();
			return result;
		}

		private async Task<TResult> NotFoundResult(SetContainedViewRequest<TResult> input)
		{
			var r = new FlowResultResolverRequest<TResult>
			{
				ScreenModel = new UiFlowStepUnauthorized()
			};
			r.CopyCallbacksFrom(input);

			return await _flowResultResolver.Execute(r);
		}
	}
}