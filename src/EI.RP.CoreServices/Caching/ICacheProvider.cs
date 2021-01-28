using System;
using System.Threading;
using System.Threading.Tasks;

namespace EI.RP.CoreServices.Caching
{
	public interface ICacheProvider
	{
		/// <param name="key"></param>
		/// <param name="providerFunc"></param>
		/// <param name="keyContext">contextualizes the key provided, so there can be duplicates in different contexts</param>
		/// <param name="maxDurationFromNow"></param>
		/// <param name="cancellationToken"></param>
		Task<TValue> GetOrAddAsync<TKey, TValue>(TKey key, Func<Task<TValue>> providerFunc, string keyContext = null, TimeSpan? maxDurationFromNow = null,CancellationToken cancellationToken = default(CancellationToken)) where TValue : class;
		Task<TValue> GetOrAddAsync<TKey, TValue>(TKey key, TValue value, string keyContext = null, TimeSpan? maxDurationFromNow = null,CancellationToken cancellationToken = default(CancellationToken)) where TValue : class;
		Task ClearAsync(string keyContext=null,CancellationToken cancellationToken = default(CancellationToken));
		CacheProviderType Type { get; }
		Task InvalidateAsync<TKey>(string keyContext,CancellationToken cancellationToken = default(CancellationToken),params TKey[] keys);
		Task InvalidateAsync<TKey>(CancellationToken cancellationToken = default(CancellationToken),params TKey[] keys);


		Task<bool> ContainsKeyAsync<TKey,TValue>(TKey key, string keyContext = null, CancellationToken cancellationToken = default(CancellationToken)) where TValue:class;
		Task CheckHealth(CancellationToken cancellationToken=default(CancellationToken));
		bool IsReadyToUse();
	}
}