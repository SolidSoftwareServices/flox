using System;
using System.Threading;
using System.Threading.Tasks;

namespace S3.CoreServices.Resiliency
{
	public class ResilientOperations
	{
		public static readonly ResilientOperations Default = new ResilientOperations();

		private ResilientOperations()
		{ }

		public async Task RetryIfNeeded(Func<Task> payloadAction,
			CancellationToken cancellationToken = default(CancellationToken), int maxAttempts = 2,
			TimeSpan? waitBetweenAttempts = null)
		{
			await RetryIfNeeded(async () =>
			{
				await payloadAction();
				return true;
			}, cancellationToken, maxAttempts, waitBetweenAttempts);
		}

		public async Task<TResult> RetryIfNeeded<TResult>(Func<Task<TResult>> payloadAction,
			CancellationToken cancellationToken = default(CancellationToken), int maxAttempts = 2,
			TimeSpan? waitBetweenAttempts = null)
		{
			bool succeeded = false;
			var result = default(TResult);
			while (!succeeded)
			{
				if (cancellationToken != default(CancellationToken) && cancellationToken.IsCancellationRequested)
					throw new OperationCanceledException();
				try
				{

					result = await payloadAction();
					succeeded = true;
				}
				catch
				{
					if (--maxAttempts == 0) throw;
					if (waitBetweenAttempts.HasValue)
					{
						await Task.Delay(waitBetweenAttempts.Value,cancellationToken);
					}
					else
					{
						await Task.Yield();
					}
				}
			}

			return result;
		}
	}
}
