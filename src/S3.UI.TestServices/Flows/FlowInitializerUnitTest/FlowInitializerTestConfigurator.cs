using AutoFixture;
using S3.UI.TestServices.Flows.Shared;
using S3.UiFlows.Core.Flows.Initialization;

namespace S3.UI.TestServices.Flows.FlowInitializerUnitTest
{

	public sealed partial class FlowInitializerTestConfigurator<TInitializer> : FlowTestConfigurator<
		FlowInitializerTestConfigurator<TInitializer>,
		FlowInitializerWithLifecycleAdapter<TInitializer>>
		where TInitializer : class, IUiFlowInitializationStep
	{
		public FlowInitializerTestConfigurator(IFixture fixture = null) : base(fixture ?? new Fixture())
		{
		}

		protected override FlowInitializerWithLifecycleAdapter<TInitializer> BuildAdapter()
		{
			var adapter =
				new FlowInitializerWithLifecycleAdapter<TInitializer>(Mocker.CreateInstance<TInitializer>());
			return adapter;
		}
	}
}