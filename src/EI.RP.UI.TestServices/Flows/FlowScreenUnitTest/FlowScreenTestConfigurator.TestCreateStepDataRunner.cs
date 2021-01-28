using System;
using System.Dynamic;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;


namespace EI.RP.UI.TestServices.Flows.FlowScreenUnitTest
{
	public partial class FlowScreenTestConfigurator<TFlowScreen, TFlowType>
	{

		/// <summary>
		/// Retrieves a test runner specialized in testing the screen creation
		/// </summary>
		/// <returns></returns>
		public TestCreateStepDataRunner NewTestCreateStepDataRunner() => new TestCreateStepDataRunner(Adapter);
		public class TestCreateStepDataRunner
		{
			private readonly FlowScreenWithLifecycleAdapter<TFlowScreen, TFlowType> _adapter;

			internal TestCreateStepDataRunner(FlowScreenWithLifecycleAdapter<TFlowScreen,TFlowType> adapter)
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
				private readonly FlowScreenWithLifecycleAdapter<TFlowScreen, TFlowType> _adapter;
				private readonly UiFlowScreenModel _actual;

				internal Assert(FlowScreenWithLifecycleAdapter<TFlowScreen, TFlowType> adapter,
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