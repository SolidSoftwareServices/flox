using System;
using System.Dynamic;
using S3.CoreServices.System;
using S3.UiFlows.Core.Flows;
using S3.UiFlows.Core.Flows.Initialization;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;

using NUnit.Framework;

namespace S3.UI.TestServices.Flows.FlowInitializerUnitTest
{
	public partial class FlowInitializerTestConfigurator<TInitializer, TFlowType>
	{
		/// <summary>
		/// Retrieves a test runner specialized in the initialization of the flow
		/// </summary>
		/// <returns></returns>
		public TestInitializationRunner NewInitializationRunner() => new TestInitializationRunner(Adapter);
		public class TestInitializationRunner
		{
			private readonly FlowInitializerWithLifecycleAdapter<TInitializer, TFlowType> _adapter;

			internal TestInitializationRunner(FlowInitializerWithLifecycleAdapter<TInitializer,TFlowType> adapter)
			{
				_adapter = adapter;
			}
			/// <summary>
			///specifies the flow input for a given scenario
			/// </summary>
			/// <typeparam name="TInput"></typeparam>
			/// <param name="input"></param>
			/// <returns></returns>
			public TestInitializationRunner WithInput<TInput>(TInput input) where TInput : IFlowInputContract
			{
				return WithInput(input.ToExpandoObject());
			}
			/// <summary>
			///specifies the flow input for a given scenario
			/// </summary
			public TestInitializationRunner WithInput(ExpandoObject input)
			{
				this.Input = input;
				return this;
				
			}

			public ExpandoObject Input { get; private set; }


			public FlowInitializationAssert Run()
			{
				return new FlowInitializationAssert(_adapter, _adapter.Initialize(Input).GetAwaiter().GetResult());
			}
			public class FlowInitializationAssert
			{
				private readonly FlowInitializerWithLifecycleAdapter<TInitializer, TFlowType> _adapter;
				private readonly ScreenEvent _result;

				internal FlowInitializationAssert(FlowInitializerWithLifecycleAdapter<TInitializer, TFlowType> adapter,
					ScreenEvent result)
				{
					_adapter = adapter;
					_result = result;
				}
				/// <summary>
				/// Asserts the step data after the initialisation was executed
				/// </summary>
				/// <typeparam name="TStepData"></typeparam>
				/// <param name="asserter"></param>
				/// <returns></returns>
				public FlowInitializationAssert AssertStepData<TStepData>(Action<TStepData> asserter)
					where TStepData : UiFlowScreenModel
				{
					asserter(_adapter.GetStepData<TStepData>().GetAwaiter().GetResult());
					return this;
				}
				/// <summary>
				/// // Asserts the event triggered the initialisation was executed
				/// </summary>
				/// <param name="asserter"></param>
				/// <returns></returns>
				public FlowInitializationAssert AssertTriggeredEventIs(Action<ScreenEvent> asserter)
				{
					asserter(_result);
					return this;
				}
				/// <summary>
				/// // Asserts the event triggered the initialisation was executed
				/// </summary>
				public FlowInitializationAssert AssertTriggeredEventIs(ScreenEvent expected)
				{
					
					return AssertTriggeredEventIs(actual=>Assert.AreEqual(expected,actual));
				}
			}
		}
	}
}