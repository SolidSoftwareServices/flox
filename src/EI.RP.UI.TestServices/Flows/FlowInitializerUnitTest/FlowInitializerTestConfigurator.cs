using System.Diagnostics;
using AutoFixture;
using EI.RP.UI.TestServices.Flows.Shared;
using EI.RP.UiFlows.Core.Flows.Initialization;

namespace EI.RP.UI.TestServices.Flows.FlowInitializerUnitTest
{
#if !FrameworkDeveloper
	[DebuggerStepThrough]
#endif
	public sealed partial class FlowInitializerTestConfigurator<TInitializer, TFlowType> : FlowTestConfigurator<
		FlowInitializerTestConfigurator<TInitializer, TFlowType>,
		FlowInitializerWithLifecycleAdapter<TInitializer, TFlowType>>
		where TInitializer : class, IUiFlowInitializationStep<TFlowType>
	{
		public FlowInitializerTestConfigurator(IFixture fixture = null) : base(fixture ?? new Fixture())
		{
		}

		protected override FlowInitializerWithLifecycleAdapter<TInitializer, TFlowType> BuildAdapter()
		{
			var adapter =
				new FlowInitializerWithLifecycleAdapter<TInitializer, TFlowType>(Mocker.CreateInstance<TInitializer>());
			return adapter;
		}
	}
}