using System;
using System.Threading;
using System.Threading.Tasks;

namespace EI.RP.CoreServices.Caching
{
	public class NoCacheProvider : IInternalCacheProvider, ICacheAccountPreLoaderRequester, IMemoryCacheProvider
	{
		public async Task<TValue> GetOrAddAsync<TKey, TValue>(TKey key, Func<Task<TValue>> providerFunc,
			string keyContext = null, TimeSpan? maxDurationFromNow = null, CancellationToken cancellationToken = default(CancellationToken)) where TValue : class
		{
			return await providerFunc();
		}

		public Task<TValue> GetOrAddAsync<TKey, TValue>(TKey key, TValue value, string keyContext = null,
			TimeSpan? maxDurationFromNow = null, CancellationToken cancellationToken = default(CancellationToken)) where TValue : class
		{
			return Task.FromResult(value);
		}

		public Task ClearAsync(string keyContext, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.CompletedTask;
		}

		public CacheProviderType Type { get; } = CacheProviderType.NoCache;
		public Task InvalidateAsync<TKey>(string keyContext, CancellationToken cancellationToken = default(CancellationToken),
			params TKey[] keys)
		{
			return Task.CompletedTask;
		}

		public Task InvalidateAsync<TKey>(CancellationToken cancellationToken = default(CancellationToken), params TKey[] keys)
		{
			return InvalidateAsync((string)null,default(CancellationToken), keys);
		}

		public Task<bool> ContainsKeyAsync<TKey, TValue>(TKey key, string keyContext = null,
			CancellationToken cancellationToken = default(CancellationToken)) where TValue : class
		{
			return Task.FromResult(false);
		}

		public Task CheckHealth(CancellationToken cancellationToken = default(CancellationToken)) => Task.CompletedTask;

		public bool IsReadyToUse() => true;
		

		public Task SubmitRequestAsync(string forUserName)
		{
			return Task.CompletedTask;
		}
	}
}