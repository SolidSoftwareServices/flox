using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Autofac.Features.Indexed;
using EI.RP.CoreServices.System;
using EI.RP.UiFlows.Core.Flows.Initialization;
using EI.RP.UiFlows.Core.Flows.Screens;

namespace EI.RP.UiFlows.Core.Flows.Runtime
{
	class FlowRuntimeResolver : IFlowRuntimeInfoResolver
	{
		private readonly IIndex<string, IEnumerable<IUiFlowScreen>> _screenHandlersIndex;
		private readonly IIndex<string, IUiFlowInitializationStep> _initializationStepIndex;
		
		private readonly ConcurrentDictionary<string,FlowRuntimeInfo> _runtimes=new ConcurrentDictionary<string, FlowRuntimeInfo>();

		public FlowRuntimeResolver(IIndex<string, IEnumerable<IUiFlowScreen>> screenHandlersIndex, 
			IIndex<string, IUiFlowInitializationStep> initializationStepIndex)
		{
			_screenHandlersIndex = screenHandlersIndex;
			_initializationStepIndex = initializationStepIndex;
		}


		public FlowRuntimeInfo GetFlowInfo(string flowType)
		{
			return _runtimes.GetOrAdd(flowType, (key) =>
			{
				var screenHandlers = _screenHandlersIndex[key].ToDictionary(x => x.ScreenStep, x => x);
				return new FlowRuntimeInfo(_initializationStepIndex[key], screenHandlers);
			});

		}

	}
}