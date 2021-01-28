using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.CoreServices.Caching.InMemory;
using EI.RP.CoreServices.Caching.Models;
using EI.RP.CoreServices.System.Async;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using NLog;
using StackExchange.Redis;


namespace EI.RP.CoreServices.Caching.Redis
{
	partial class RedisRemoteCacheFacade:IRedisCacheFacade,IDisposable
	 {
		private static readonly ILogger Logger = LogManager.GetLogger("CacheLogger");
		private readonly AsyncLazy<RedisCache> _redisCache;
		private readonly IRedisServerProvider _redisServerProvider;
		private ISubscriber _redisEventsSubscriber;
		private List<IDisposable> _disposables=new List<IDisposable>();
		private AsyncLazy<RedisCacheOptions> Options { get; }
		
		public RedisRemoteCacheFacade(AsyncLazy<RedisCacheOptions>options,AsyncLazy<RedisCache> redisCache,IRedisServerProvider redisServerProvider)
		{
			_redisCache = redisCache;
			_redisServerProvider = redisServerProvider;
			Options = options;
			
			
		}

		public event CacheKeyHandler RemoteKeyValueSetReceivedFromRedis;
		public event CacheKeyHandler RemoteKeyDeletedReceivedFromRedis;
		public event CacheKeyHandler SetRemoteKeyValueWasSentToRedis;
		public event CacheKeyHandler DeleteRemoteKeyWasSentToRedis;
		public async Task Synchronize(CancellationToken cancellationToken = default(CancellationToken))
		{
			await Task.WhenAll(InitiatePublisher(), InitiateSubscriber());
			
			var keys= await GetKeys(true, cancellationToken);
			await (RemoteKeyValueSetReceivedFromRedis?.Invoke(this, keys.ToArray())??Task.CompletedTask);
			
		}
		public async Task ClearCache(CancellationToken cancellationToken = default(CancellationToken))
		{
			var keys= (await GetKeys(true,cancellationToken));
			await Task.WhenAll(keys.Select(key => RemoveAsync(key, cancellationToken)));
		}

		public async Task<IEnumerable<string>> GetKeys(bool userKey=true, CancellationToken cancellationToken=default(CancellationToken))
		{
			var server=await _redisServerProvider.GetServerAsync(cancellationToken);
			var instanceName = await GetInstanceName();
			var keys = server.Keys(pattern: $"{instanceName}*{CacheKeyScope.EnvironmentKeyPart()}*{CacheKeyScope.VersionKeyPart}*")
				.Select(x=>(string)x);
			if (userKey)
			{
				keys = keys.Select(x => x?.Replace(instanceName, string.Empty));
			}
			return keys;
		}

		private async Task<string> GetInstanceName()
		{
			var optionsValue = await Options.Value;
			return optionsValue.InstanceName;
		}


		

		public async Task<bool> ContainsKey<TKey>(TKey key, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (key == null) throw new ArgumentNullException(nameof(key));
			//TODO: optimizable by direct querying
			var strkey = key.ToString();
			return (await GetKeys(cancellationToken:cancellationToken)).Any(x => x == strkey);
		}

		public async Task<byte[]> GetAsync(string key, CancellationToken cancellationToken = default(CancellationToken))
		{
			var cache = await _redisCache.Value;
			return await cache.GetAsync(key, cancellationToken);
		}

		public async Task ResetSubscriptions()
		{
			
			await (_redisEventsSubscriber?.UnsubscribeAllAsync()??Task.CompletedTask);
			_redisEventsSubscriber = null;
			foreach (var disposable in _disposables)
			{
				try
				{
					disposable.Dispose();
				}
				catch (Exception ex)
				{
					Logger.Warn(()=>ex.ToString());
				}
			}
			_disposables.Clear();
		}


		public void Dispose()
		{
			ResetSubscriptions().GetAwaiter().GetResult();
		}
		
	 }
	
}