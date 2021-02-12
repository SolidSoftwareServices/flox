using System.Diagnostics;
using AutoFixture;
using S3.UI.TestServices.Flows.Shared;
using S3.UiFlows.Core.Flows.Screens;


namespace S3.UI.TestServices.Flows.FlowScreenUnitTest
{

	public sealed partial class FlowScreenTestConfigurator<TFlowScreen>: FlowTestConfigurator<FlowScreenTestConfigurator<TFlowScreen>,FlowScreenWithLifecycleAdapter<TFlowScreen>>
		where TFlowScreen : class, IUiFlowScreen
	{
		public FlowScreenTestConfigurator(IFixture fixture=null) : base(fixture??new Fixture())
		{
		}

		protected override FlowScreenWithLifecycleAdapter<TFlowScreen> BuildAdapter()
		{
			
			var adapter = new FlowScreenWithLifecycleAdapter<TFlowScreen>(Mocker.CreateInstance<TFlowScreen>(),Fixture);
			return adapter;
		}


	}
}