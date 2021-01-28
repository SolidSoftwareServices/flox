using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace EI.RP.WebApp.AcceptanceTests.Infrastructure.Selenium
{
	class PageElementAwaiter
	{
		public static readonly PageElementAwaiter Instance = new PageElementAwaiter();

		private PageElementAwaiter()
		{ }

		public void Retry(Action<int> payload, TimeSpan timeout,
			TimeSpan? waitBetweenAttempts = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			Poll((attempt) =>
			{
				payload(attempt);
				return true;
			}, timeout, waitBetweenAttempts, cancellationToken);
		}

		public async Task Retry(Func<int,Task> payload, TimeSpan timeout,
			TimeSpan? waitBetweenAttempts = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			await PollAsync(async (attempt) =>
			{
				await payload(attempt);
				return true;
			}, timeout, waitBetweenAttempts, cancellationToken);
		}

		public TResult Poll<TResult>(Func<int,TResult> payload, TimeSpan timeout,
			TimeSpan? waitBetweenAttempts = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			return PollAsync((attempt) => Task.FromResult(payload(attempt)), timeout, waitBetweenAttempts, cancellationToken)
				.GetAwaiter().GetResult();
		}

		public async Task<TResult> PollAsync<TResult>(Func<int,Task<TResult>> payload, TimeSpan timeout,
			TimeSpan? waitBetweenAttempts = null, CancellationToken cancellationToken = default(CancellationToken))
		{

			int attemptNumber = 0;

			var maxTime = DateTime.UtcNow.Add(timeout);
			bool succeeded = false;
			var result = default(TResult);

			while (!succeeded)
			{
				if (cancellationToken != default(CancellationToken) && cancellationToken.IsCancellationRequested)
				{
					throw new OperationCanceledException();
				}

				try
				{

					result = await payload(++attemptNumber);
					succeeded = true;
				}
				catch
				{
					if (DateTime.UtcNow > maxTime)
					{
						throw;
					}

					if (waitBetweenAttempts.HasValue)
					{
						await Task.Delay(waitBetweenAttempts.Value, cancellationToken);
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