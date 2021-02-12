using System;
using System.Dynamic;
using S3.UiFlows.Core.Flows;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;


namespace S3.UI.TestServices.Flows.FlowScreenUnitTest
{
	public partial class FlowScreenTestConfigurator<TFlowScreen>
	{

		/// <summary>
		/// Retrieves a test runner specialized in testing the screen creation
		/// </summary>
		/// <returns></returns>
		public TestCreateStepDataRunner NewTestCreateStepDataRunner() => new TestCreateStepDataRunner(Adapter);
		public class TestCreateStepDataRunner
		{
			private readonly FlowScreenWithLifecycleAdapter<TFlowScreen> _adapter;

			internal TestCreateStepDataRunner(FlowScreenWithLifecycleAdapter<TFlowScreen> adapter)
			{
				_adapter = adapter;
			}

			public TestCreateStepDataRunner WithExistingStepData<TStepData>(ScreenName step, TStepData stepData) where TStepData : UiFlowScreenModel
			{
				_adapter.SetStepData(stepData,step);
				return this;
			}

			public Assert WhenCreated()
			{
				UiFlowScreenModel actual = _adapter.CreateStepData();
				return new Assert(_adapter, actual);
			}

			public class Assert
			{
				private readonly FlowScreenWithLifecycleAdapter<TFlowScreen> _adapter;
				private readonly UiFlowScreenModel _actual;

				internal Assert(FlowScreenWithLifecycleAdapter<TFlowScreen> adapter,
					UiFlowScreenModel actual)
				{
					_adapter = adapter;
					_actual = actual;
				}

				public Assert ThenTheStepDataIs<TStepData>(Action<TStepData> asserter)
					where TStepData : UiFlowScreenModel
				{
					asserter((TStepData)_actual);
					return this;
				}

				
			}
		}
	}
}