using System.Diagnostics;
using EI.RP.UI.TestServices.Flows.Shared;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Initialization;
using EI.RP.UiFlows.Core.Flows.Screens;

namespace EI.RP.UI.TestServices.Flows.FlowInitializerUnitTest
{
#if !FrameworkDeveloper
	[DebuggerStepThrough]
#endif
	class TestFlowCreateStepDataConfigurator : TestFlowNavigationHelper
	{
		private readonly IUiFlowInitializationStep _initializationTarget;

		public TestFlowCreateStepDataConfigurator(IUiFlowInitializationStep initializationTarget) : base(ScreenName.PreStart)
		{
			_initializationTarget = initializationTarget;
		}

		protected override ScreenName OnExecute(NavigationResolver navigationResolver)
		{
			return navigationResolver.Resolve();
		}

	}
}