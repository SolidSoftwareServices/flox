using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.CoreServices.Caching.InMemory;
using EI.RP.CoreServices.Caching.Models;
using EI.RP.CoreServices.Caching.Redis;
using EI.RP.CoreServices.Encryption;
using EI.RP.CoreServices.System.Async;
using EI.RP.Stubs.CoreServices.Encryption;
using EI.RP.TestServices;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;

namespace EI.RP.CoreServices.Caching.IntegrationTests.Redis
{
	partial class RedisCacheProviderTests
	{
	
		public class TestContext : UnitTestContext<RedisCacheProvider>
		{
			private RedisCache _redisCache;
			private RedisServerProvider _redisServerProvider;
			
			public TestContext()
			{
			}

			private bool _initialised = false;
			private TestContext EnsureInitialised()
			{
				if (!_initialised)
				{
					var options = new RedisCacheOptions
					{
						Configuration = ConnectionString,
						InstanceName = InstanceName
					};
					AutoMocker.Use(options);
					AutoMocker.Use(new AsyncLazy<RedisCacheOptions>(options));
					_redisServerProvider = new RedisServerProvider(AutoMocker.Get<RedisCacheOptions>());
					AutoMocker.Use((IRedisServerProvider) _redisServerProvider);
					
					

					_initialised = true;
				}

				return this;
			}
			private readonly string testRunId = Guid.NewGuid().ToString();
			public string GetKey(string testName,int idx)
			{
				return $"{GetKeyPrefix(testName)}{idx}";
			}

			public string GetKeyPrefix(string testName)
			{
				return $"{testRunId}{testName}_";
			}
			public override void Reset()
			{
				_redisCache?.Dispose();
				_redisCache = null;

				_redisServerProvider?.Dispose();
				_redisServerProvider = null;

				_initialised = false;

				base.Reset();
			}

			public string ConnectionString { get; } =
				"EIResPortalROI-dev-rdc-01.redis.cache.windows.net:6380,password=E1IaG+2TcEPwapIiSrdNfhfB20ksLR1QzAnmCxkJWK0=,ssl=True,abortConnect=False";
				//"eiresportalroi-pre-rdc-01.redis.cache.windows.net:6380,password=dv4Yzso+nlgnNEScaCByJYwR1A0YcvLSU6AEq8P3BA4=,ssl=True,abortConnect=False";
			public string InstanceName { get; }= $"RedisRemoteSubscriberTests_{Guid.NewGuid()}";

			public RedisCache GetCache()
			{
				EnsureInitialised();
				return _redisCache ?? (_redisCache = GetRedisCache());

				RedisCache GetRedisCache()
				{
					var cache = new RedisCache(new OptionsWrapper<RedisCacheOptions>(AutoMocker.Get<RedisCacheOptions>()));
					AutoMocker.Use(cache);
					AutoMocker.Use(new AsyncLazy<RedisCache>(cache));

					

					return cache;
				}
			}

			

			private readonly List<RedisCache> _otherClientsCreated=new List<RedisCache>();
			public TestRawRedisCache GetAnotherClientOfTheSameCacheApp(Guid? appInstanceId=null)
			{
				var instanceName = InstanceName;
				return GetTestRawRedisCache( instanceName,appInstanceId);
			}
			
			public TestRawRedisCache GetAnotherClientOfSameCacheButDifferentApp(Guid? appInstanceId=null)
			{
				var instanceName =  $"AnotherAppUsingTheCache{Guid.NewGuid()}";
				return GetTestRawRedisCache( instanceName,appInstanceId);
				
			}
			private TestRawRedisCache GetTestRawRedisCache( string instanceName,Guid? appInstanceId)
			{
				var cache = new TestRawRedisCache(new OptionsWrapper<RedisCacheOptions>(new RedisCacheOptions
				{
					Configuration = ConnectionString,
					InstanceName = instanceName
				}), appInstanceId);
				_otherClientsCreated.Add(cache);
				return cache;
			}
			class MemoryCacheOptions : IOptions<MemoryDistributedCacheOptions>
			{
				public MemoryDistributedCacheOptions Value => new MemoryDistributedCacheOptions()
					{ExpirationScanFrequency = TimeSpan.FromMilliseconds(100)};
			}

			public MemoryDistributedCache InMemoryCache { get; } =
				new MemoryDistributedCache(new MemoryCacheOptions());

			public TimeSpan Expiration { get; set;}= TimeSpan.FromSeconds(3);

			
			protected override RedisCacheProvider BuildSut(AutoMocker autoMocker)
			{
				EnsureInitialised();
				GetCache();
				this.InMemoryCacheProvider = new InMemoryCacheProvider(BuildCacheSettings().Object, new AsyncLazy<MemoryDistributedCache>(InMemoryCache));
				autoMocker.Use(InMemoryCacheProvider);
				RedisCacheFacade=new RedisRemoteCacheFacade(AutoMocker.Get<AsyncLazy<RedisCacheOptions>>(),AutoMocker.Get<AsyncLazy<RedisCache>>(),autoMocker.Get<IRedisServerProvider>());
				autoMocker.Use((IRedisCacheFacade)RedisCacheFacade);
				autoMocker.Use((IEncryptionService)new NoEncryptionService());
				return base.BuildSut(autoMocker);
				Mock<ICacheSettings> BuildCacheSettings()
				{
					var mock = autoMocker
						.GetMock<ICacheSettings>();
					mock
						.SetupGet(x => x.CacheProviderType)
						.Returns(CacheProviderType.Redis);
					mock
						.SetupGet(x => x.ExpireCacheItemsWhenNotUsedFor)
						.Returns(Expiration);
					mock
						.SetupGet(x => x.IsCachePreLoaderEnabled)
						.Returns(false);
					mock
						.SetupGet(x => x.IsCacheEnabled)
						.Returns(true);
					return mock;
				}
			}

			public RedisRemoteCacheFacade RedisCacheFacade { get;private set; }

			public InMemoryCacheProvider InMemoryCacheProvider { get; private set; }


			public TestContext WithCacheDown()
			{
				return this;
			}
			public ManualResetEventSlim SignalWhenNumItemsSent(string testName,int numItems)
			{
				var signal=new ManualResetEventSlim(false);
				var counter = 0;
				RedisCacheFacade.SetRemoteKeyValueWasSentToRedis += (sender, keys) =>
				{
					if (keys.Any(x => x.Contains(GetKeyPrefix(testName))) && Interlocked.Increment(ref counter) == numItems)
					{
						signal.Set();
					}

					return Task.CompletedTask;
				};
				return signal;
			}
			public ManualResetEventSlim SignalWhenNumItemsRemoved(int numItems)
			{
				var signal=new ManualResetEventSlim(false);
				var counter = 0;
				RedisCacheFacade.DeleteRemoteKeyWasSentToRedis += (sender, keys) =>
				{
					if(Interlocked.Increment(ref counter)==numItems)
						signal.Set();
					return Task.CompletedTask;
				};
				return signal;
			}
			public ManualResetEventSlim SignalWhenItemsSent(Func<string[], bool> conditionEval )
			{
				var signal=new ManualResetEventSlim(false);
				
				RedisCacheFacade.SetRemoteKeyValueWasSentToRedis += (sender, keys) =>
				{
					if (conditionEval(keys))
					{
						signal.Set();
					}
					return Task.CompletedTask;
				};
				return signal;
			}
			
			public ManualResetEventSlim SignalWhenItemsSetRemotely(Func<string[], bool> conditionEval )
			{
				var signal=new ManualResetEventSlim(false);
				
				RedisCacheFacade.RemoteKeyValueSetReceivedFromRedis+= (sender, keys) =>
				{
					if (conditionEval(keys))
					{
						signal.Set();
					}
					return Task.CompletedTask;
				};
				return signal;
			}

			public ManualResetEventSlim SignalWhenItemsRemovedRemotely(Func<string[], bool> conditionEval )
			{
				var signal=new ManualResetEventSlim(false);
				
				RedisCacheFacade.RemoteKeyDeletedReceivedFromRedis+= (sender, keys) =>
				{
					if (conditionEval(keys))
					{
						signal.Set();
					}
					return Task.CompletedTask;
				};
				return signal;
			}
			protected override void Dispose(bool disposing)
			{
				
				if (_redisServerProvider != null)
				{
					_redisServerProvider.Dispose();
					_redisServerProvider = null;
				}

				if (_redisCache != null)
				{
					_redisCache.Dispose();
					_redisCache = null;
				}
				RedisCacheFacade?.Dispose();
				RedisCacheFacade = null;
				foreach (var redisCache in _otherClientsCreated)
				{
					redisCache.Dispose();
				}
			}
			public async Task AssertItemIsInRedis<TKey,TValue>(TKey key,TValue expected)
			{
				var redisCache = GetCache();

				var item = await redisCache.GetAsync(new CacheKey<TKey,TValue>(key));
				Assert.IsNotNull(item);
				var actual = (TValue) (await item.CacheDeserializeAsync())?.Item;
				Assert.AreEqual(expected, actual);
			}
			public IEnumerable<Task> AssertItemsAreInRedis<TValue>(string testName,int numItems, Func<int, TValue> func)
			{
				return Enumerable.Range(0, numItems).Select(i => AssertItemIsInRedis(GetKey(testName,i), func(i)));
			}

			public async Task AssertItemNotIsInRedis<TKey,TValue>(TKey key)
			{
				var redisCache = GetCache();

				var item = await redisCache.GetAsync(new CacheKey<TKey,TValue>(key));
				Assert.IsNull(item);
			}

			public async Task AssertItemIsInRedis<TKey,TValue>(TKey key)
			{
				var redisCache = GetCache();

				var item = await redisCache.GetAsync(new CacheKey<TKey,TValue>(key));
				Assert.IsNotNull(item);
			}


		
		}
	}
}