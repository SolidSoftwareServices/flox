using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using NLog;

namespace EI.RP.CoreServices.Batching
{
	public enum BatchStatus
	{
		/// <summary>
		/// it admits requests
		/// </summary>
		Opened=1,
		/// <summary>
		/// It is being executed
		/// </summary>
		/// <remarks>it does not admit requests</remarks>
		Executing,
		/// <summary>
		/// It ewas executed
		/// </summary>
		/// <remarks>it does not admit requests</remarks>
		Executed
	}
#if !FrameworkDeveloper
	[DebuggerStepThrough]
#endif
	public abstract class BatchBase<TRequest> where TRequest : class
	{
		public string Id { get; }
		/// <summary>
		/// it will await for other bacth aggregators to enlist new requests
		/// </summary>
		public TimeSpan EnlistAwaitTimeout { get; protected set; }
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		private readonly HashSet<IBatchAggregator> _currentBatchAggregators =
			new HashSet<IBatchAggregator>();

		private readonly ManualResetEventAsync _resetEvent = new ManualResetEventAsync(false);
		private readonly object _syncLock = new object();
		private readonly CancellationTokenSource _cts = new CancellationTokenSource();
		private  int _count = 0;
		public BatchStatus Status { get; private set; } = BatchStatus.Opened;

		protected BatchBase(string id,double enlistAwaitTimeoutMilliseconds=0)
		{
			Id = id;
			EnlistAwaitTimeout = TimeSpan.FromMilliseconds(enlistAwaitTimeoutMilliseconds);
		}
		public int Count => _count;
		private Guid _lastOperation=Guid.NewGuid();
		internal async Task Enlist(TRequest request)
		{
			ThrowIfNoMoreRequestAllowed();
			await OnEnlist(request);
			Interlocked.Increment(ref _count);
			_lastOperation = Guid.NewGuid();
			await _resetEvent.WaitAsync(_cts.Token);
		}

	
		protected abstract Task OnEnlist(TRequest request);


		public IBatchAggregator NewBatchAggregatorToken()
		{
			var token = new BatchAggregator<TRequest>(this, _cts);
			lock (_syncLock)
			{
				ThrowIfNoMoreRequestAllowed();
				_currentBatchAggregators.Add(token);
			}
			_lastOperation = Guid.NewGuid();
			return token;
		}

	
		protected abstract Task OnExecute(CancellationToken cancellationToken);

		internal async Task FlushBatch(IBatchAggregator aggregator)
		{
			await AwaitEnlistersUntilNoMoreEnlist();

			bool executeBatch;
			Task awaitForTask;

			lock (_syncLock)
			{
				ThrowIfNoMoreRequestAllowed();
				ThrowIfInvalidToken(aggregator);

				var token = aggregator;
				_currentBatchAggregators.Remove(token);
				executeBatch = !_currentBatchAggregators.Any();
				if (executeBatch)
				{
					Status = BatchStatus.Executing;
					awaitForTask = OnExecute(_cts.Token);
					
				}
				else
					awaitForTask = _resetEvent.WaitAsync(_cts.Token);
			}
			await awaitForTask;
			if (executeBatch)
			{
				
				Status = BatchStatus.Executed;
				
				Logger.Trace($"FlushBatch Batch {Id} of {Count} requests");
				_count = 0;
				_resetEvent.Set();
				
			}

			async Task AwaitEnlistersUntilNoMoreEnlist()
			{
				var current = _lastOperation;
				//by doing this we are letting other asynchronous tasks to enlist before executing the batch
				await Task.Delay(EnlistAwaitTimeout);

				while (current != _lastOperation)
				{
					current = _lastOperation;
					await Task.Delay(EnlistAwaitTimeout);
				}
			}
		}

		private void ThrowIfInvalidToken(IBatchAggregator aggregator)
		{
			if (!_currentBatchAggregators.Contains(aggregator))
				throw new InvalidOperationException("The aggregator is not part of the current batch");
		}
		private void ThrowIfNoMoreRequestAllowed()
		{
			if (Status!=BatchStatus.Opened)
				throw new BatchWasClosedException($"The batch status is not opened. Status:{Status}");
		}

		public void Dispose()
		{
			_cts.Cancel(false);
			_cts.Dispose();
			
		}
	}

	public class BatchWasClosedException:Exception
	{
		public BatchWasClosedException()
		{
		}

		protected BatchWasClosedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public BatchWasClosedException(string message) : base(message)
		{
		}

		public BatchWasClosedException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}