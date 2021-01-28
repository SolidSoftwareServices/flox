using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.Batching;
using EI.RP.CoreServices.OData.Client.Infrastructure.Batches;
using EI.RP.CoreServices.Ports.OData;

namespace EI.RP.CoreServices.OData.Client
{
	
	public abstract partial class ODataRepository
	{
		private Lazy<ODataRepositoryBatch> _batch;

		private async Task ExecuteAutoBatch(Func<IBatchAggregator, Task> payload)
		{
			var p =(Func<IBatchAggregator, Task<bool>>) (async b =>
			{
				await payload(b);
				return true;
			});
			await ExecuteAutoBatch(p);
		}
		private async Task<TResult> ExecuteAutoBatch<TResult>(Func<IBatchAggregator, Task<TResult>> payloadWithResult)
		{
			using (var token = StartNewBatchAggregator())
			{
				var task = payloadWithResult(token);
				
				await token.CompleteBatch();
				return await task;
			}
		}

		public IBatchAggregator StartNewBatchAggregator()
		{
			
			IBatchAggregator token;
			try
			{
				token = Resolve();
			}
			catch (InvalidOperationException)
			{
				token = Resolve();
			}

			return token;

			IBatchAggregator Resolve()
			{
				if (_batch.Value.Status != BatchStatus.Opened)
				{
					CreateNewBatch();
				}

				return _batch.Value.NewBatchAggregatorToken();
			}
		}

		private void CreateNewBatch()
		{
			_batch = new Lazy<ODataRepositoryBatch>(() => new ODataRepositoryBatch(this.EndpointUrl,_client,this.BatchEnlistTimeoutMilliseconds,Profiler));
		}

	}
}