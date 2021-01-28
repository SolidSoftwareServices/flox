using System;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;

namespace EI.RP.WebApp.AcceptanceTests.Infrastructure.NUnitExtensions
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	public class RetryOnErrorAttribute : PropertyAttribute, IWrapSetUpTearDown
	{
		//this is not a good practice so don't abuse of it, its still better than using the pipeline for that
		private readonly int _numberOfAttempts;

		public RetryOnErrorAttribute () : this(TestSettings.Default.NumberOfAttemptsOnDriverFailure)
		{
		}
		public RetryOnErrorAttribute (int numberOfAttempts) : base(numberOfAttempts)
		{
			_numberOfAttempts = numberOfAttempts;
		}


		public TestCommand Wrap(TestCommand command)
		{
			return new RetryOnErrorCommand(command, _numberOfAttempts);
		}

		private class RetryOnErrorCommand : DelegatingTestCommand
		{
			private  int _pendingAttempts;

			private static readonly ResultState[] FailureStates =
			{
				ResultState.Error, ResultState.Failure, ResultState.SetUpError, ResultState.SetUpFailure,
				ResultState.TearDownError, ResultState.ChildFailure
			};

			public RetryOnErrorCommand(TestCommand nextInChain, int pendingAttempts)
				: base(nextInChain)
			{
				_pendingAttempts = pendingAttempts;
			}

			
			public override TestResult Execute(TestExecutionContext context)
			{
				//only retry non-app errors otherwise we would be hiding the defect

				var totalAttempts = _pendingAttempts;
				while (_pendingAttempts-- > 0)
				{
					var txt = $"{context.CurrentTest.Name} - Starting Attempt #{totalAttempts-_pendingAttempts} of {totalAttempts}";
					
					var contextOutWriter = context.OutWriter;
					contextOutWriter.WriteOutputLine(txt);

					context.CurrentResult = innerCommand.Execute(context);
					
					//only repeat if the web driver crashed
					var resultState = context.CurrentResult.ResultState;
					txt =
						$"{context.CurrentTest.Name} - Completed Attempt #{totalAttempts - _pendingAttempts} of {totalAttempts} with result of {resultState}. {context.CurrentResult.Message} ";
					TestContext.Progress.WriteOutputLine(txt);
					contextOutWriter.WriteLine(txt);
					if (FailureStates.All(x => x != resultState))
					{
						break;
					}
				}

				//returns the last result
				return context.CurrentResult;
			}

			
		}

		
	}
}