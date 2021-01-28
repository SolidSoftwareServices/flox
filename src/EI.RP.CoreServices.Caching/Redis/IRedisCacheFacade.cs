using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.CoreServices.Caching.InMemory;
using Microsoft.Extensions.Caching.Distributed;

namespace EI.RP.CoreServices.Caching.Redis
{
	internal interface IRedisCacheFacade
	{
		event CacheKeyHandler RemoteKeyValueSetReceivedFromRedis;
		event CacheKeyHandler RemoteKeyDeletedReceivedFromRedis;
		Task Synchronize(CancellationToken cancellationToken = default(CancellationToken));
		Task<bool> ContainsKey<TKey>(TKey key, CancellationToken cancellationToken = default(CancellationToken));
		Task<byte[]> GetAsync(string key, CancellationToken cancellationToken = default(CancellationToken));
		Task RemoveAsync(string key, CancellationToken cancellationToken = default(CancellationToken));
		Task SetAsync(string key, byte[] bytes, DistributedCacheEntryOptions options);
		Task ResetSubscriptions();
	}
}