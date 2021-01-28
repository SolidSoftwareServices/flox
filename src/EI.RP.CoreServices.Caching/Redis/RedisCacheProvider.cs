using System;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.CoreServices.Caching.InMemory;
using EI.RP.CoreServices.Caching.Models;
using EI.RP.CoreServices.Encryption;
using EI.RP.CoreServices.Resiliency;
using EI.RP.CoreServices.System;
using EI.RP.CoreServices.System.Async;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using NLog;
using StackExchange.Redis;

namespace EI.RP.CoreServices.Caching.Redis
{
	class RedisCacheProvider : IInternalCacheProvider
	{
		private static readonly ILogger Logger = LogManager.GetLogger("CacheLogger");
		private readonly InMemoryCacheProvider _memoryCache;
		private readonly IRedisCacheFacade _redisCacheFacade;
		private readonly AsyncLazy<RedisCacheOptions> _cacheOptions;
		private readonly IEncryptionService _encryptionService;
		public CacheProviderType Type { get; } = CacheProviderType.Redis;
		private readonly Guid _processInstanceId;

		public RedisCacheProvider(
			InMemoryCacheProvider memoryCache, IRedisCacheFacade redisCacheFacade,
			AsyncLazy<RedisCacheOptions> cacheOptions, IEncryptionService encryptionService)
		{
			_memoryCache = memoryCache;
			_redisCacheFacade = redisCacheFacade;
			_cacheOptions = cacheOptions;
			_encryptionService = encryptionService;

			redisCacheFacade.RemoteKeyDeletedReceivedFromRedis += RedisSubscriber_RemoteKeyDeleted;
			_redisCacheFacade.RemoteKeyValueSetReceivedFromRedis += RedisSubscriber_RemoteKeyValueSet;

			_memoryCache.UserKeyValueAdded += _memoryCache_UserKeyValueAdded;
			_memoryCache.UserKeyValueDeleted += _memoryCache_UserKeyValueDeleted;

			_processInstanceId = new CachedItem().GeneratedByInstanceId;
		}

		private readonly ConcurrentHashSet<string> _redisOriginated = new ConcurrentHashSet<string>();
		private readonly ConcurrentHashSet<string> _memoryOriginated = new ConcurrentHashSet<string>();

		private async Task RedisSubscriber_RemoteKeyDeleted(object sender, string[] keys)
		{
			foreach (var key in keys)
			{

				if (_synchronized && !_memoryOriginated.Remove(key))
				{
					_redisOriginated.Add(key);
					await _memoryCache.InvalidateAsync(default(CancellationToken), key);
					Logger.Debug(() => $"RedisSubscriber_RemoteKeyDeleted: Removed local key: {key} From Remote");
				}
				else
				{
					Logger.Debug(() =>
						$"RedisSubscriber_RemoteKeyDeleted: Remove of local key : {key} was originated locally");
				}
			}
		}

		private async Task RedisSubscriber_RemoteKeyValueSet(object sender, string[] keys)
		{
			try
			{
				foreach (var key in keys)
				{

					if (_synchronized && !_memoryOriginated.Remove(key))
					{
						_redisOriginated.Add(key);
						
						var bytes = await _redisCacheFacade.GetAsync(key);
						if (bytes != null)
						{
							var cachedItem = await bytes.CacheDeserializeAsync(_encryptionService);
							if (cachedItem != null && cachedItem.GeneratedByInstanceId != _processInstanceId)
							{
								await _memoryCache.SetValueAsync(key, cachedItem);
								Logger.Debug(() =>
									$"RedisSubscriber_RemoteKeyValueAdded: Add local key: {key} From Remote");
							}
						}
					}

				}
			}
			catch (Exception)
			{
				await OnRedisException();
				throw;
			}
		}

		private async Task OnRedisException()
		{
			Logger.Warn(()=>"Resetting redis interop");
			_synchronized = false;
			await _redisCacheFacade.ResetSubscriptions();
			
		}

		private async Task _memoryCache_UserKeyValueDeleted(object sender, string[] keys)
		{
			foreach (var key in keys)
			{

				if (_synchronized && !_redisOriginated.Remove(key))
				{
					_memoryOriginated.Add(key);
					try
					{
						await _redisCacheFacade.RemoveAsync(key);
					}
					catch (Exception)
					{
						await OnRedisException();
						throw;
					}
				}
			}
		}

		private async Task _memoryCache_UserKeyValueAdded(object sender, string key, CachedItem value,
			DistributedCacheEntryOptions options)
		{
			if (_synchronized &&!_redisOriginated.Remove(key))
			{
				_memoryOriginated.Add(key);
				var cachedItem = value;
				Logger.Debug(() => $"redis.SetAsync - key={key}");
				try
				{
					await _redisCacheFacade.SetAsync(key, await cachedItem.CacheSerializeAsync(_encryptionService),
						options);
				}
				catch (Exception)
				{
					await OnRedisException();
					throw;
				}
			}
		}

		public async Task<TValue> GetOrAddAsync<TKey, TValue>(TKey key, Func<Task<TValue>> providerFunc,
			string keyContext = null,
			TimeSpan? maxDurationFromNow = null, CancellationToken cancellationToken = default(CancellationToken))
			where TValue : class
		{
			await EnsureInitializationSynchronization(cancellationToken);
			return await _memoryCache.GetOrAddAsync(key, providerFunc, keyContext, maxDurationFromNow,
				cancellationToken);
		}

		private volatile bool _synchronized = false;
		private readonly SemaphoreSlim _syncSemaphoreSlim = new SemaphoreSlim(1, 1);
		private DateTime _nextCheck = DateTime.MinValue;
		private bool _connectionErrorLogged = false;
		private async Task<bool> EnsureInitializationSynchronization(CancellationToken cancellationToken)
		{

			if (!_synchronized && DateTime.UtcNow > _nextCheck)
			{
				await _syncSemaphoreSlim.AsyncCriticalSection(async () =>
				{
					if (!_synchronized && DateTime.UtcNow > _nextCheck)
					{
						try
						{
							await _redisCacheFacade.Synchronize(cancellationToken);
							_synchronized = true;
							_connectionErrorLogged = false;
						}
						catch (Exception ex)
						{
							await OnRedisException();
							
							_nextCheck = DateTime.UtcNow.AddSeconds(20);
							if (!_connectionErrorLogged)
							{
								Logger.Error(()=> $"Could not connect to Redis. You still Memory cache, but you HAVE TO fix this promptly. Reason:{ex}");
								_connectionErrorLogged = true;
							}
						}

					}
				});

			}

			return _synchronized;
		}

		public bool IsReadyToUse() => _synchronized;


		public async Task<TValue> GetOrAddAsync<TKey, TValue>(TKey key, TValue value, string keyContext = null,
			TimeSpan? maxDurationFromNow = null, CancellationToken cancellationToken = default(CancellationToken))
			where TValue : class
		{
			await EnsureInitializationSynchronization(cancellationToken);
			return await _memoryCache.GetOrAddAsync(key, value, keyContext, maxDurationFromNow, cancellationToken);
		}

		public async Task ClearAsync(string keyContext = null,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			await EnsureInitializationSynchronization(cancellationToken);
			await _memoryCache.ClearAsync(keyContext, cancellationToken);
		}

		public async Task InvalidateAsync<TKey>(string keyContext,
			CancellationToken cancellationToken = default(CancellationToken),
			params TKey[] keys)
		{
			await EnsureInitializationSynchronization(cancellationToken);
			await _memoryCache.InvalidateAsync(keyContext, cancellationToken, keys);
		}

		public async Task InvalidateAsync<TKey>(CancellationToken cancellationToken = default(CancellationToken),
			params TKey[] keys)
		{
			await EnsureInitializationSynchronization(cancellationToken);
			await _memoryCache.InvalidateAsync(cancellationToken, keys);
		}

		public async Task<bool> ContainsKeyAsync<TKey, TValue>(TKey key, string keyContext = null,
			CancellationToken cancellationToken = default(CancellationToken)) where TValue : class
		{
			var synchronized = await EnsureInitializationSynchronization(cancellationToken);

			var innerKey = new CacheKey<TKey, TValue>(keyContext, key);
			try
			{
				var redisTask = synchronized
					? _redisCacheFacade.ContainsKey(innerKey, cancellationToken)
					: Task.FromResult(false);
				return await _memoryCache.ContainsKeyAsync<TKey, TValue>(key, null, cancellationToken) ||
				       await redisTask;
			}
			catch (Exception)
			{
				await OnRedisException();
				throw;
			}
		}

		public async Task CheckHealth(CancellationToken cancellationToken = default(CancellationToken))
		{
			await CanAddAndGetValue();
			await CanSynchronize();

			async Task CanAddAndGetValue()
			{
				var key = Guid.NewGuid().ToString();

				try
				{
					var value = await GetOrAddAsync(key, "test", maxDurationFromNow: TimeSpan.FromMinutes(5),cancellationToken:cancellationToken);
					if (value != "test")
					{
						throw new InvalidOperationException("Could not retrieve the expected value from the cache");
					}
				}
				finally
				{
					await InvalidateAsync(cancellationToken, key);
				}
			}

			async Task CanSynchronize()
			{
				await EnsureInitializationSynchronization(cancellationToken);
				var testInstanceId = Guid.NewGuid();
				await ResilientOperations.Default.RetryIfNeeded(async () =>
				{
					var key = Guid.NewGuid().ToString();

					try
					{
						var options = new OptionsWrapper<RedisCacheOptions>(await _cacheOptions.Value);
						using (var c = new RedisCache(options))
						{

							await c.SetAsync(new CacheKey<string, string>(key), await new CachedItem
							{
								GeneratedByInstanceId = testInstanceId, Item = "Test"
							}.CacheSerializeAsync(_encryptionService), cancellationToken);
						}

						await Task.Delay(TimeSpan.FromSeconds(10),cancellationToken);
						var actual = await GetOrAddAsync(key, "wrong value",
							maxDurationFromNow: TimeSpan.FromMinutes(5), cancellationToken: cancellationToken);
						if (actual != "Test")
						{
							throw new InvalidOperationException(
								$"The synchronization settings are not correct. Possible cause: notify-keyspace-events => AKE. Test data: {actual} ");
						}
					}
					finally
					{
						await InvalidateAsync(cancellationToken, key);
					}
				}, cancellationToken: cancellationToken, maxAttempts: 4, waitBetweenAttempts: TimeSpan.FromSeconds(1));
			}
		}


	}
}