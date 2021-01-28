using System.Diagnostics;
using S3.UI.TestServices.Flows.Shared;
using S3.UiFlows.Core.Flows;
using S3.UiFlows.Core.Flows.Initialization;
using S3.UiFlows.Core.Flows.Screens;

namespace S3.UI.TestServices.Flows.FlowInitializerUnitTest
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