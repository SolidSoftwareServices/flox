using System;
using System.Threading.Tasks;
using S3.UiFlows.Core.Flows;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;

using NUnit.Framework;
using String = System.String;

namespace S3.UI.TestServices.Flows.FlowScreenUnitTest
{
	public partial class FlowScreenTestConfigurator<TFlowScreen>
	{
		/// <summary>
		/// Retrieves a test runner specialized in testing events on the ScreenNameId
		/// </summary>
		/// <param name="initialDataBuilder">define the instance valu3e before running the event. I can be modified after using also GivenTheStepDataIs</param>
		/// <returns></returns>
		public EventTestRunner<TStepData> NewEventTestRunner<TStepData>(Func<TStepData> initialDataBuilder) where TStepData:UiFlowScreenModel => new EventTestRunner<TStepData>(Adapter, initialDataBuilder());
		public EventTestRunner<TStepData> NewEventTestRunner<TStepData>(TStepData stepData) where TStepData : UiFlowScreenModel => NewEventTestRunner<TStepData>(()=>stepData);
		public class EventTestRunner<TStepData> where TStepData:UiFlowScreenModel
		{
			private readonly FlowScreenWithLifecycleAdapter<TFlowScreen> _adapter;

			internal EventTestRunner(FlowScreenWithLifecycleAdapter<TFlowScreen> adapter,TStepData initialData)
			{
				_adapter = adapter;
				GivenTheStepDataIs(initialData);

			}

			public EventTestRunner<TStepData> GivenTheStepDataIs(TStepData stepData)
			{
				_adapter.SetStepData( stepData, _adapter.GetStep());
				return this;
			}
			public EventTestRunner<TStepData> WithExistingStepData<TOtherStepData>(ScreenName step, TOtherStepData stepData) where TOtherStepData : UiFlowScreenModel
			{
				Assert.AreNotEqual(typeof(TStepData),typeof(TOtherStepData));
				_adapter.SetStepData(stepData, step);
				return this;
			}
			public AssertScreenStep WhenEvent(ScreenEvent eventToTrigger)
			{
			
				var validationResult = _adapter.RunValidation(eventToTrigger,out string errorMessage);

				var stepResult = validationResult==null || validationResult.Value
					? _adapter.ExecuteEvent(eventToTrigger)
					: _adapter.ExecuteErrorEvent(errorMessage);
				return new AssertScreenStep(_adapter,validationResult,errorMessage,stepResult);
			}

			public class AssertScreenStep
			{
				private readonly FlowScreenWithLifecycleAdapter<TFlowScreen> _adapter;
				private readonly bool? _validationResult;
				private readonly string _validationErrorMessage;

				internal AssertScreenStep(FlowScreenWithLifecycleAdapter<TFlowScreen> adapter,
					bool? validationResult, string validationErrorMessage, ScreenName stepResult)
				{
					_adapter = adapter;
					_validationResult = validationResult;
					_validationErrorMessage = validationErrorMessage;
					ResultStep = stepResult;
				}

				public ScreenName ResultStep { get; }

				public AssertScreenStep ThenTheResultStepIs(ScreenName expected)
				{
					return ThenTheResultStepIs(actual => Assert.AreEqual(expected, actual));
				}
				public AssertScreenStep ThenTheResultStepIs(Action<ScreenName> asserter)
				{
					asserter(ResultStep);
					return this;
				}

				public AssertScreenStep ThenTheValidationFailed(Action<string> assertValidationResult)
				{
					ThrowIfValidationDidNotRun();
					Assert.IsFalse(_validationResult.Value);
					assertValidationResult(_validationErrorMessage);
					return this;
				}

				private void ThrowIfValidationDidNotRun()
				{
					if(!_validationResult.HasValue) Assert.Fail($"The validation did not run. Possibly due to exclusion of the event. See {nameof(UiFlowScreenModel)}.{nameof(UiFlowScreenModel.DontValidateEvents)}");
				}

				public AssertScreenStep ThenTheValidationPassed()
				{
					ThrowIfValidationDidNotRun();
					Assert.IsTrue(_validationResult.Value);
					return this;
				}
				public AssertScreenStep ThenTheValidationFailed( string expectedErrorMessage)
				{
					return ThenTheValidationFailed(( b) =>
					{
						Assert.AreEqual(expectedErrorMessage, b);
					});
				}

				public AssertScreenStep ThenTheValidationResultIs(bool expectedResult,string expectedErrorMessage)
				{
					return ThenTheValidationResultIs((a,b) =>
					{
						Assert.AreEqual(expectedResult,a);
						Assert.AreEqual(expectedErrorMessage, b);
					});
				}

				public AssertScreenStep ThenTheValidationResultIs(Action<bool,string> assertValidationResult)
				{
					ThrowIfValidationDidNotRun();
					assertValidationResult(_validationResult.Value, _validationErrorMessage);
					return this;
				}

				public AssertScreenStep ThenTheValidationWasNotExecuted()
				{
					Assert.IsFalse(_validationResult.HasValue);
					return this;
				}



				public AssertScreenStep ThenTheStepDataAfterIs<TScreenData>(Action<TScreenData> assertAction) where TScreenData : UiFlowScreenModel
				{
					
					assertAction((TScreenData)_adapter.GetCurrentStepData<UiFlowScreenModel>().GetAwaiter().GetResult());
					return this;
				}
				
				public AssertScreenStep ThenTheStepDataAfterIsNull()
				{
					return ThenTheStepDataAfterIs<UiFlowScreenModel>(Assert.IsNull);
				}

				
			}


		
		}
	}
}