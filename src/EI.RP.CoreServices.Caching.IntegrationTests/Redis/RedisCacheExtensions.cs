using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.Caching.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;

namespace EI.RP.CoreServices.Caching.IntegrationTests.Redis
{
	class TestRawRedisCache:RedisCache
	{
		public Guid AppInstanceId { get; }

		public TestRawRedisCache(IOptions<RedisCacheOptions> optionsAccessor, Guid? appInstanceId = null) : base(optionsAccessor)
		{
			AppInstanceId = appInstanceId??Guid.NewGuid();
		}
		public  async Task<CachedItem> SetTestRawItem<TKey,TValue>(TKey key,TValue value,TimeSpan absoluteExpiration)
		{
			var cachedKey = new CacheKey<TKey,TValue>(key);
			var anotherValue = new CachedItem
			{
				Item = value,
				GeneratedByInstanceId = AppInstanceId
			};

			await SetAsync(cachedKey, await anotherValue.CacheSerializeAsync(),
				new DistributedCacheEntryOptions
				{
					AbsoluteExpirationRelativeToNow = absoluteExpiration
				});
			return anotherValue;

		}

		
	}
}