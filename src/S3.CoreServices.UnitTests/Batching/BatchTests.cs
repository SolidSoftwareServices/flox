using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using S3.CoreServices.Batching;
using NUnit.Framework;

namespace S3.CoreServices.UnitTests.Batching
{
	[Parallelizable(ParallelScope.Fixtures)]
	[TestFixture]
	public class BatchTests
	{
		
		private  TargetBatch _sut;

		[SetUp]
		public void OnSetup()
		{
			_sut=new TargetBatch();
		}


		[Test]
		public async Task CanEnlistItem()
		{
			var value = Guid.NewGuid().ToString();
			using (var aggregator = _sut.NewBatchAggregatorToken())
			{
				 _sut.Enlist(value);
				Thread.Sleep(10);
				Assert.IsEmpty(_sut.ExecutedRequests);
				await aggregator.CompleteBatch();
				Assert.IsTrue(_sut.ExecutedRequests.Single() == value);

			}
		}


		[Test]
		public async Task CanCompleteWhenNoItems()
		{
			using (var aggregator = _sut.NewBatchAggregatorToken())
			{
				await aggregator.CompleteBatch();
				Assert.IsEmpty(_sut.ExecutedRequests);

			}
		}

		[Test]
		public async Task WhenNotCompleted()
		{
			var value = Guid.NewGuid().ToString();
			using (var aggregator = _sut.NewBatchAggregatorToken())
			{
				 _sut.Enlist(value);
				Thread.Sleep(10);
			}
			Assert.IsEmpty(_sut.ExecutedRequests);
		}

		
		[Test]
		public async Task CannotCompleteAfterDispose()
		{
			var value = Guid.NewGuid().ToString();
			var aggregator = _sut.NewBatchAggregatorToken();

			_sut.Enlist(value);
			aggregator.Dispose();
			Assert.ThrowsAsync<ObjectDisposedException>(async () => await aggregator.CompleteBatch());
		}

		[Test]
		public async Task CanAggregateFromSeveralAggregators()
		{
			using (var aggregator = _sut.NewBatchAggregatorToken())
			{
				for (var i = 0; i < 1000; i++)
				{
					 _sut.Enlist(i.ToString());
				}
				Assert.IsEmpty(_sut.ExecutedRequests);
				await aggregator.CompleteBatch();
				_sut.EnListedRequests.Clear();
				
				Assert.IsTrue(Enumerable.Range(0,999).All(x=> _sut.ExecutedRequests.Count(y=>y==x.ToString())==1));
			}
		}
		
		[Test]
		public async Task CannotReuseCompletedAggregator()
		{
			using (var aggregator = _sut.NewBatchAggregatorToken())
			{
				var v1 = Guid.NewGuid().ToString();
				 _sut.Enlist(v1);
				
				await aggregator.CompleteBatch();
				var v2 = Guid.NewGuid().ToString();
				Assert.ThrowsAsync<BatchWasClosedException>(async ()=> await _sut.Enlist(v2));
			}
		}

	
		
		private class TargetBatch : BatchBase<string>
		{
			public TimeSpan? ExecutionPayloadTime = null;

			public HashSet<string> EnListedRequests { get; } = new HashSet<string>();
			public HashSet<string> ExecutedRequests { get; } = new HashSet<string>();

			protected override Task OnEnlist(string request)
			{
				this.EnListedRequests.Add(request);

				return Task.CompletedTask;
			}

			public int ExecutionCount;
			

			protected override async Task OnExecute(CancellationToken cancellationToken)
			{
				foreach (var request in EnListedRequests)
				{
					ExecutedRequests.Add(request);
				}
				Interlocked.Increment(ref ExecutionCount);
				if (ExecutionPayloadTime.HasValue)
				{
					await Task.Delay(ExecutionPayloadTime.Value);
				}
			}


			public TargetBatch() : base("TestBatch")
			{
			}

			public void SetEnlistAwait(TimeSpan timeout)
			{
				EnlistAwaitTimeout = timeout;
			}
		}

	}
}