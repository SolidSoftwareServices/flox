using System.Diagnostics;
using AutoFixture;
using S3.UI.TestServices.Flows.Shared;
using S3.UiFlows.Core.Flows.Initialization;

namespace S3.UI.TestServices.Flows.FlowInitializerUnitTest
{

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