using System.Collections.Generic;
using S3.UiFlows.Core.Flows.Initialization;
using S3.UiFlows.Core.Flows.Screens;

namespace S3.UiFlows.Core.Flows.Runtime
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