using System;
using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;

namespace EI.RP.UiFlows.Core.Facade.InitialView
{
	class FlowInitialViewRequestHandler<TResult> : IFlowInitialViewRequestHandler<TResult>
	{
		private readonly IAppUiFlowsCollection _flows;
		private readonly IUiFlowContextRepository _contextRepository;
		public FlowInitialViewRequestHandler(IAppUiFlowsCollection flows, IUiFlowContextRepository contextRepository)
		{
			_flows = flows;
			_contextRepository = contextRepository;
		}

		public async Task<TResult> Execute(InitialViewRequest<TResult> input)
		{
			if (string.IsNullOrWhiteSpace(input.FlowType))
				throw new ArgumentException("Invalid request: flow type is missing");
			var flow = _flows.GetByFlowType(input.FlowType);

			var viewModel = await flow.StartNew(input.ContainerFlowHandler, input.FlowInput);
			var result = await input.OnBuildView(viewModel);
			await _contextRepository.Flush();
			return result;
		}
	}
}