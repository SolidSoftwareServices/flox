using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EI.RP.CoreServices.System.DependencyInjection;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;

namespace Ei.Rp.Web.DebugTools.Infrastructure.FlowDiagnostics
{
	class FlowChangesRecorder : IFlowChangesRecorder
	{
		private readonly IServiceProvider _provider;
		private readonly ConcurrentQueue<FlowRecorderState> _states = new ConcurrentQueue<FlowRecorderState>();

		public FlowChangesRecorder(IServiceProvider provider)
		{
			_provider = provider;
		}

		public async Task<IDisposable> NewScopeAsync(string scopeId)
		{
			var flowRecorderScope = new FlowRecorderScope(scopeId, _states, _provider.Resolve<IUiFlowContextRepository>());
			await flowRecorderScope.Initialise();
			return flowRecorderScope;
		}

		public IEnumerable<FlowRecorderState> Requests()
		{
			return _states.ToArray().OrderBy(_=>_.Timestamp);
		}

		public IEnumerable<FlowContextTrace> GetFlowStateDifferences(string flowHandler)
		{
			return Requests()
				.OrderBy(x => x.Timestamp)
				.Select(x =>
				{
					var diff = x.Diff(flowHandler);
					return new FlowContextTrace
					(
						x.ScopeId,
						diff,
						x.InitialState.SingleOrDefault(c => c.FlowHandler == flowHandler),
						x.EndState.SingleOrDefault(c => c.FlowHandler == flowHandler)
					);
				})
				//.Where(x => x.diff.Differences.Any())
				.ToArray();
		}

		
	}


}