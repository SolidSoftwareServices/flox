using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Infrastructure.IoC;

namespace S3.UiFlows.Core.Flows
{
	class AppUiFlowsCollection : IAppUiFlowsCollection
	{
		private readonly IIndex<string, IUiFlow> _flows;
		private readonly IUiFlowContextRepository _contextRepository;
		private readonly FlowNames _flowNames;

		public AppUiFlowsCollection(IIndex<string, IUiFlow> flows, IUiFlowContextRepository contextRepository, FlowNames flowNames)
		{
			_flows = flows;
			_contextRepository = contextRepository;
			_flowNames = flowNames;
		}

		public IUiFlow GetByFlowType(string flowType)
		{
			return _flows[flowType.ToLowerInvariant()];
		}

		public async Task<IUiFlow> GetByFlowHandler(string flowHandler)
		{
			if (string.IsNullOrWhiteSpace(flowHandler))
			{
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(flowHandler));
			}

			var uiFlowContextData = await _contextRepository.Get(flowHandler);
			var type = uiFlowContextData?.FlowType;
			return type == null ? null : _flows[type];
		}

		public async Task<IEnumerable<IUiFlow>> GetAll()
		{
			return _flowNames.Names.Select(GetByFlowType).ToArray();
		}
	}
}