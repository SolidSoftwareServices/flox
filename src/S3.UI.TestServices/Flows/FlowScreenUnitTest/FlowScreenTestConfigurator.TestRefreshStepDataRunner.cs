using System;
using System.Collections.Generic;
using System.Dynamic;
using S3.UiFlows.Core.Flows;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;


namespace S3.UI.TestServices.Flows.FlowScreenUnitTest
{
	public partial class FlowScreenTestConfigurator<TFlowScreen, TFlowType>
	{

		/// <summary>
		/// Retrieves a test runner specialized in testing the screen refresh
		/// </summary>
		/// <returns></returns>
		public TestRefreshStepDataRunner NewTestRefreshStepDataRunner() => new TestRefreshStepDataRunner(Adapter);
		public class TestRefreshStepDataRunner
		{
			private readonly FlowScreenWithLifecycleAdapter<TFlowScreen, TFlowType> _adapter;

			internal TestRefreshStepDataRunner(FlowScreenWithLifecycleAdapter<TFlowScreen,TFlowType> adapter)
			{
				_adapter = adapter;
			}
			public TestRefreshStepDataRunner GivenTheCurrentStepDataBeforeRefreshIs<TStepData>( TStepData stepData) where TStepData : UiFlowScreenModel
			{
				return WithExistingStepData(_adapter.GetStep(),stepData);
			}
			public TestRefreshStepDataRunner WithExistingStepData<TStepData>(ScreenName step, TStepData stepData) where TStepData : UiFlowScreenModel
			{
				_adapter.SetStepData(stepData,step);
				return this;
			}

			public Assert WhenRefreshed(IDictionary<string, object> withRouteParameters=null)
			{
				UiFlowScreenModel actual = _adapter.RefreshStepData(withRouteParameters);
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