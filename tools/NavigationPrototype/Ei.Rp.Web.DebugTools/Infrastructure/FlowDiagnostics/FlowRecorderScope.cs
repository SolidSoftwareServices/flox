using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using EI.RP.CoreServices.System.Async;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.UiFlows.Core.Infrastructure.DataSources.Repositories;

namespace Ei.Rp.Web.DebugTools.Infrastructure.FlowDiagnostics
{
	class FlowRecorderScope : IDisposable
	{
		private readonly ConcurrentQueue<FlowRecorderState> _states;
		private readonly UiFlowContextDataRepositoryDecorator _repository;
		private readonly AsyncLazy<FlowRecorderState> _state;

		public FlowRecorderScope(string scopeId, ConcurrentQueue<FlowRecorderState> states,
			IUiFlowContextRepository repository)
		{
			_states = states;
			_repository = (UiFlowContextDataRepositoryDecorator)repository;
			_state = new AsyncLazy<FlowRecorderState>(async()=>new FlowRecorderState
			{
				ScopeId = scopeId,
				InitialState = (await _repository.GetAll()).CloneDeep()
			});
		}

		private async Task Record()
		{
			var state = await _state;
			state.EndState = await _repository.GetAll().CloneDeep(); 

			_states.Enqueue(state);
		}

		public void Dispose()
		{
			Record().GetAwaiter().GetResult();
		}

		public async Task Initialise()
		{
			var a=await _state.Value;
		}
	}
}