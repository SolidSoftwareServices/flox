using System.Diagnostics;
using AutoFixture;
using S3.UI.TestServices.Flows.Shared;
using S3.UiFlows.Core.Flows.Screens;


namespace S3.UI.TestServices.Flows.FlowScreenUnitTest
{
#if !FrameworkDeveloper
	[DebuggerStepThrough]
#endif
	public sealed partial class FlowScreenTestConfigurator<TFlowScreen, TFlowType>: FlowTestConfigurator<FlowScreenTestConfigurator<TFlowScreen, TFlowType>,FlowScreenWithLifecycleAdapter<TFlowScreen, TFlowType>>
		where TFlowScreen : class, IUiFlowScreen<TFlowType>
	{
		public FlowScreenTestConfigurator(IFixture fixture=null) : base(fixture??new Fixture())
		{
		}

		protected override FlowScreenWithLifecycleAdapter<TFlowScreen, TFlowType> BuildAdapter()
		{
			
			var adapter = new FlowScreenWithLifecycleAdapter<TFlowScreen, TFlowType>(Mocker.CreateInstance<TFlowScreen>(),Fixture);
			return adapter;
		}


	}
}