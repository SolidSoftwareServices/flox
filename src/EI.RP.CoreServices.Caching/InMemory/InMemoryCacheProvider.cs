using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.CoreServices.Caching.Models;
using EI.RP.CoreServices.System;
using EI.RP.CoreServices.System.Async;
using EI.RP.CoreServices.System.FastReflection;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using NLog;

namespace EI.RP.CoreServices.Caching.InMemory
{
	sealed class InMemoryCacheProvider:IMemoryCacheProvider, IInternalCacheProvider
	{
		private static readonly ILogger Logger = LogManager.GetLogger("CacheLogger");
		

		public  CacheProviderType Type { get; } = CacheProviderType.InMemory;


		private readonly bool _isCacheEnabled;
		private readonly TimeSpan _slidingExpiration;

		public InMemoryCacheProvider(ICacheSettings settings, MemoryDistributedCache cache) : this(settings, new AsyncLazy<MemoryDistributedCache>(cache))
		{ }
		public InMemoryCacheProvider(ICacheSettings settings, AsyncLazy<MemoryDistributedCache> cache)
		{
			Cache = cache;
			_isCacheEnabled = settings.IsCacheEnabled;
			_slidingExpiration = settings.ExpireCacheItemsWhenNotUsedFor;
		}
		public  AsyncLazy<MemoryDistributedCache> Cache { get; }

		public async Task InvalidateAsync<TKey>(string keyContext, CancellationToken cancellationToken = default(CancellationToken),
			params TKey[] keys)
		{
			if (!_isCacheEnabled) return;

			var keysToRemove =
				(await Task.WhenAll(keys.Select(x =>
					{
						var strKey = x.ToString();

						var keyStr = strKey.IsCacheKey()?strKey :(new CacheKey<TKey,object>(x).ToString());
						return CacheKey<TKey, object>.GetUserKey(keyStr);
					})
					.Select(userKey => GetKeys(k => k.Contains(userKey))))).SelectMany(x => x).ToArray();
			
			await RemoveKeys(keysToRemove);
		}

		public async Task InvalidateAsync<TKey>(CancellationToken cancellationToken = default(CancellationToken), params TKey[] keys)
		{
			await InvalidateAsync((string)null,default(CancellationToken), keys);
		}

		private static readonly ConcurrentHashSet<string> KeysBeingAdded=new ConcurrentHashSet<string>();
		public async Task<TValue> GetOrAddAsync<TKey, TValue>(TKey key, Func<Task<TValue>> providerFunc,
			string keyContext = null, TimeSpan? maxDurationFromNow = null, CancellationToken cancellationToken = default(CancellationToken)) where TValue : class
		{
			ThrowIfMaxDurationIsInvalid(maxDurationFromNow);
			if (!_isCacheEnabled)
			{
				return await providerFunc();
			}

			var cache = (await Cache.Value);

			var keyValue = new CacheKey<TKey,TValue>(keyContext,key);
			var distributedCache = (await Cache.Value);
			var cacheHit = await distributedCache.GetAsync(keyValue,cancellationToken);
			CachedItem value=null;
			
			if (cacheHit == null)
			{
				value = await Add();
			}
			else
			{
				value = await GetCachedItem(cacheHit);
			}

			if (value == null)
			{
				value = await Add();
			}

			return (TValue)value.Item;

			async Task<CachedItem> Add()
			{
				var keyBeingAddedKey = $"{Thread.CurrentThread.ManagedThreadId}_{keyValue}";
				bool creator = KeysBeingAdded.AddIfNotExists(keyBeingAddedKey);

				while (!creator && KeysBeingAdded.Contains(keyBeingAddedKey))
				{
					await Task.Yield();
				}

				if (creator)
				{
					KeysBeingAdded.Add(keyBeingAddedKey);
					try
					{
						value = new CachedItem
						{
							CreatedTimeUtc = DateTime.UtcNow,
							Item = await providerFunc()
						};

						await SetValueAsync(keyValue, value, maxDurationFromNow,cancellationToken);
						
					}
					finally
					{
						KeysBeingAdded.Remove(keyBeingAddedKey);
					}

					LogCacheMissed<TKey>(keyValue);
				}
				else
				{
					int attempts=0;
					do
					{
						await Task.Yield();
						
						cacheHit = await cache.GetAsync(keyValue);
					} while (cacheHit == null && ++attempts<5);
					value = await GetCachedItem(cacheHit);
					LogCacheHit<TKey>(keyValue);
				}

				return value;
			}

			async Task<CachedItem> GetCachedItem(byte[] bytes)
			{
				if (bytes == null) return null;
				try
				{
					value = await bytes.CacheDeserializeAsync();
				}
				catch (NullReferenceException ex)
				{
					Logger.Error(() => $"{ex}");
					throw;
				}

				LogCacheHit<TKey>(keyValue);
				return value;
			}
		}

		private static void ThrowIfMaxDurationIsInvalid(TimeSpan? maxDurationFromNow)
		{
			if (maxDurationFromNow.HasValue && maxDurationFromNow.Value > TimeSpan.FromDays(1))
			{
				throw new ArgumentOutOfRangeException(nameof(maxDurationFromNow), maxDurationFromNow.Value,
					$"The max cache duration allowed is 1 day");
			}
		}

		public async Task SetValueAsync(string keyValue, CachedItem value, TimeSpan? maxDurationFromNow = null,
			CancellationToken cancellationToken=default(CancellationToken)) 
		{
			Logger.Trace(()=>$"{GetType().FullName} - SetValueAsync key:{keyValue} START");
			var options = new DistributedCacheEntryOptions
			{
				SlidingExpiration = _slidingExpiration,
				AbsoluteExpirationRelativeToNow = maxDurationFromNow
			};
			var cache = await Cache.Value;
			await cache.SetAsync(keyValue, await value.CacheSerializeAsync(), options,cancellationToken);
			await OnKeyValueSet(keyValue, value, options);
			Logger.Trace(()=>$"{GetType().FullName} - SetValueAsync key:{keyValue} COMPLETED");
		}

		

		public async Task<CachedItem> GetAsync<TKey,TValue>(TKey key,string keyContext=null)
		{
			//TODO: refactor access to cached item to all use this method
			var keyValue = new CacheKey<TKey,TValue>(keyContext,key);
			var distributedCache = (await Cache.Value);
			var cacheHit = await distributedCache.GetAsync(keyValue);
			return cacheHit?.ToObject<CachedItem>();
		}
		public Task<TValue> GetOrAddAsync<TKey, TValue>(TKey key, TValue value, string keyContext = null,
			TimeSpan? maxDurationFromNow = null, CancellationToken cancellationToken = default(CancellationToken)) where TValue : class
		{
			return GetOrAddAsync(key, () => Task.FromResult(value), keyContext, maxDurationFromNow);
		}

		private static void LogCacheHit<TKey>(string keyValue)
		{
			Logger.Trace(() => $"{typeof(TKey).FullName} -CACHE HIT key: {keyValue} ");
		}

		private static void LogCacheMissed<TKey>(string keyValue)
		{
			Logger.Debug(() => $"{typeof(TKey).FullName} - CACHE MISSED key : {keyValue}");
		}

		
	

		public async Task ClearAsync(string keyContext=null, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!_isCacheEnabled) return;
			
			var prefix = new CacheKeyScope(keyContext);

			var keys = await GetKeys(x => x.StartsWith(prefix));
			await RemoveKeys(keys);

			Logger.Debug(() => $"Cache cleared");

			
		}

		private async Task RemoveKeys(IEnumerable<string> keys)
		{
			Logger.Trace(()=>$"{GetType().FullName} - RemoveKeys.Count:{keys.Count()}");
			var cache = (await Cache.Value);
			var tasks = new List<Task>();
			foreach (var key in keys)
			{
				Logger.Debug(()=>$"{GetType().FullName} - Removing key:{key}");
				var removeAsync = cache.RemoveAsync(key).ContinueWith(t =>
				{
					Logger.Debug(()=>$"{GetType().FullName} - Removed key:{key}");
					return OnKeyRemoved(key.ToString());
				});
				tasks.Add(removeAsync);
			}

			await Task.WhenAll(tasks.ToArray());
		}

		
		public async Task<bool> ContainsKeyAsync<TKey, TValue>(TKey key, string keyContext = null,
			CancellationToken cancellationToken = default(CancellationToken)) where TValue : class
		{
			if (key == null) throw new ArgumentNullException(nameof(key));
			var strKey = new CacheKey<TKey,TValue>(keyContext,key);
			var cacheValue = await Cache.Value;

			var contains = await cacheValue.GetAsync(strKey, cancellationToken) != null;
			

			return contains;
		}



		public async Task CheckHealth(CancellationToken cancellationToken = default(CancellationToken))
		{
			var key = Guid.NewGuid().ToString();

			try
			{
				var value = await GetOrAddAsync(key, "test", maxDurationFromNow: TimeSpan.FromMinutes(5));
				if (value != "test")
				{
					throw new InvalidProgramException("Could not retrieve the expected value from the cache");
				}
			}
			finally
			{

				await InvalidateAsync(cancellationToken, key);
			}
		}

		public bool IsReadyToUse() => true;

		private static readonly FieldInfo GetMemCacheField= typeof(MemoryDistributedCache).GetField("_memCache", BindingFlags.Instance | BindingFlags.NonPublic);
		private async Task<string[]> GetKeys(Func<string, bool> predicate = null)
		{
			
			var cache = await this.Cache;
			var memCache = GetMemCacheField.GetValue(cache);

			var allKeys = ((IDictionary) memCache.GetPropertyValueFastFromPropertyPath("EntriesCollection")).Keys.Cast<string>().ToArray();
			return allKeys.Where(x => predicate == null || predicate(x)).ToArray();
		}


		public event CacheKeyValueAddedHandler UserKeyValueAdded;

		private async Task OnKeyValueSet(string key, CachedItem value,
			DistributedCacheEntryOptions options)
		{
			await (UserKeyValueAdded?.Invoke(this, key, value, options) ?? Task.CompletedTask);

		}

		public event CacheKeyHandler UserKeyValueDeleted;

		private  async Task OnKeyRemoved(string key)
		{
			await (UserKeyValueDeleted?.Invoke(this, key.ToOneItemArray()) ?? Task.CompletedTask);
		}
		

		
	}
}
