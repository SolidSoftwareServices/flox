using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace EI.RP.CoreServices.Batching
{
	#if !FrameworkDeveloper
	[DebuggerStepThrough]
	#endif
	class BatchAggregator<TRequest> : IBatchAggregatorEnlister, IEquatable<BatchAggregator<TRequest>> where TRequest : class
	{
		private readonly BatchBase<TRequest> _batch;
		private CancellationTokenSource _cts;
		private bool _disposed;
		public Guid Id { get; } = Guid.NewGuid();
		public BatchAggregator(BatchBase<TRequest> batch, CancellationTokenSource cts)
		{
			_batch = batch;
			_cts = cts;
		}

		

		public async Task CompleteBatch()
		{
			ThrowIfDisposed();
			await _batch.FlushBatch(this);
			_cts = null;
		}

		public async Task Enlist<TRequest1>(TRequest1 request)
		{
			var r = request as TRequest;
			if (r == null)
			{
				throw new ArgumentException("Invalid request");
			}
			await _batch.Enlist(r);
		}

		private void ThrowIfDisposed()
		{
			if (_disposed) throw new ObjectDisposedException(this.GetType().FullName);
		}

		public void Dispose()
		{
			_cts?.Cancel();
			_disposed = true;
		}

		public bool Equals(BatchAggregator<TRequest> other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Id.Equals(other.Id);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((BatchAggregator<TRequest>) obj);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}