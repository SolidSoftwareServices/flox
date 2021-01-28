using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.CoreServices.Batching;
using EI.RP.CoreServices.Profiling;
using EI.RP.CoreServices.System;
using NLog;
using Simple.OData.Client;

namespace EI.RP.CoreServices.OData.Client.Infrastructure.Batches
{
	internal class ODataRepositoryBatch : BatchBase<Func<IODataClient, Task>>
	{
		private const int MaxRequestsPerBatch = 30;
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		private readonly ManualResetEventAsync _canAddItemsSignal = new ManualResetEventAsync(true);
		private readonly Lazy<ODataClient> _client;
		private readonly IProfiler _profiler;
		private readonly SemaphoreSlim _syncSem = new SemaphoreSlim(1, 1);

		private ConcurrentBag<Func<IODataClient, Task>> _requests = new ConcurrentBag<Func<IODataClient, Task>>();

		public ODataRepositoryBatch(string batchId, Lazy<ODataClient> client, double enlistTimeoutMilliseconds,
			IProfiler profiler) : base(batchId, enlistTimeoutMilliseconds)
		{
			_client = client;
			_profiler = profiler;
		}

		protected override async Task OnEnlist(Func<IODataClient, Task> request)
		{
			await _canAddItemsSignal.WaitAsync();
			_requests.Add(request);

			await _ExecuteRequest();
		}

		protected override async Task OnExecute(CancellationToken cancellationToken)
		{
			await _ExecuteRequest(true, cancellationToken);
		}

		private async Task _ExecuteRequest(bool force = false,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			if (force || MaxRequestsPerBatch <= _requests.Count)
			{
				await _canAddItemsSignal.WaitAsync(cancellationToken);
				_canAddItemsSignal.Reset();

				var pendingItems = new List<Func<IODataClient, Task>>(_requests);
				_requests = new ConcurrentBag<Func<IODataClient, Task>>();

				await _syncSem.WaitAsync(cancellationToken);
				_canAddItemsSignal.Set();

				try
				{
					var numOperations = pendingItems.Count;
					if (numOperations > 0)
					{
						var tasks=new List<Task>();
						while (pendingItems.Any())
						{
							var batch = pendingItems.Take(MaxRequestsPerBatch).ToArray();

							pendingItems.RemoveAll(batch.Contains);
							
							var batchRequest = new ODataBatch(_client.Value);

							foreach (var batchRequestItem in batch)
							{
								batchRequest += batchRequestItem;
							}

							
							tasks.Add(_DoExecute(batch.Length, batchRequest, !force && MaxRequestsPerBatch <= numOperations ));
						}

						await Task.WhenAll(tasks);
					}
				}
				catch (Exception ex)
				{
					Logger.Error(() => ex.ToString());
					throw;
				}
				finally
				{
					_syncSem.Release();
				}
			}

			async Task _DoExecute(int batchLength, ODataBatch batchRequest, bool thresholdReached)
			{
				var stepId = $"{nameof(ODataRepositoryBatch)} batch of {batchLength} requests";
				using (_profiler.RecordStep(stepId))
				{
					//if there is a batch then it gets executed otherwise complete the task
					var requestTask = batchRequest?.ExecuteAsync(cancellationToken) ?? Task.CompletedTask;

					await requestTask;
				}

				Logger.Debug(() =>
					$"{stepId} - {(thresholdReached ? $" AutoFlushed by threshold of max batch requests({MaxRequestsPerBatch})" : string.Empty)}");
			}
		}
	}
}