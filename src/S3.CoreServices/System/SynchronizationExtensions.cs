using System;
using System.Threading;
using System.Threading.Tasks;

namespace S3.CoreServices.System
{

	public static class SynchronizationExtensions
	{
		public static async Task AsyncCriticalSection(this SemaphoreSlim src, Func<Task> payload)
		{
			await src.AsyncCriticalSection(async () =>
			{
				await payload();
				return true;
			});
		}

		public static async Task<TResult> AsyncCriticalSection<TResult>(this SemaphoreSlim src,
			Func<Task<TResult>> payload)
		{
			await src.WaitAsync();
			try
			{
				return await payload();
			}
			finally
			{
				src.Release();
			}
		}

		public static void CriticalSection(this SemaphoreSlim src, Action payload)
		{
			src.CriticalSection(() =>
			{
				payload();
				return true;
			});
		}

		public static TResult CriticalSection<TResult>(this SemaphoreSlim src, Func<TResult> payload)
		{
			src.Wait();
			try
			{
				return payload();
			}
			finally
			{
				src.Release();
			}
		}
	}


}
