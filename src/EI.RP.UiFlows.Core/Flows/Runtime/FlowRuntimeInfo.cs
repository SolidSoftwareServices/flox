using System.Collections.Generic;
using EI.RP.UiFlows.Core.Flows.Initialization;
using EI.RP.UiFlows.Core.Flows.Screens;

namespace EI.RP.UiFlows.Core.Flows.Runtime
{
	class FlowRuntimeInfo
	{

		public FlowRuntimeInfo(IUiFlowInitializationStep initializationStep,
			IReadOnlyDictionary<ScreenName, IUiFlowScreen> screens)
		{
			InitializationStep = initializationStep;
			Screens = screens;
		}

		public IUiFlowInitializationStep InitializationStep { get; }
		public IReadOnlyDictionary<ScreenName, IUiFlowScreen> Screens { get; }

	}
}