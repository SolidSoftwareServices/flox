using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.CoreServices.Caching.Redis;
using EI.RP.TestServices;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using AutoFixture;
using EI.RP.CoreServices.Caching.Models;
using EI.RP.CoreServices.Resiliency;
using EI.RP.CoreServices.System;
using Microsoft.Extensions.Caching.Distributed;

namespace EI.RP.CoreServices.Caching.IntegrationTests.Redis
{
	
	[TestFixture]
	internal partial class
		RedisCacheProviderTests : UnitTestFixture<RedisCacheProviderTests.TestContext, RedisCacheProvider>
	{
		[TestCase(1)]
		[TestCase(2)]
		[TestCase(100)]
		public async Task CanAddItems(int numItems)
		{
			var sut = Context.Sut;

			var signal = Context.SignalWhenNumItemsSent(nameof(CanAddItems),numItems);

			for (var i = 0; i < numItems; i++)
			{
				var result = await sut.GetOrAddAsync(Context.GetKey(nameof(CanAddItems),i), async () => await Task.FromResult(i.ToString()));
				Assert.AreEqual(i.ToString(), result);
			}

			if (!signal.Wait(TimeSpan.FromSeconds(60)))
			{
				Assert.Fail("items not sent");
			}
			await Task.WhenAll(Context.AssertItemsAreInRedis(nameof(CanAddItems),numItems,i=> i.ToString()));

		}


		[TestCase(1)]
		[TestCase(10)]
		public async Task CanAddItemsConcurrently(int numItems)
		{
			var tasks = new List<Task>();
			var mre = new ManualResetEvent(false);
			Context.Expiration = TimeSpan.FromSeconds(60);
			var sut = Context.Sut;

			for (var i = 0; i < numItems; i++)
			{
				var i1 = Context.GetKey(nameof(CanAddItemsConcurrently), i);
				var value = i;
				for (var tId = 0; tId < 4; tId++)
					tasks.Add(Task.Factory.StartNew(async () =>
						{
							mre.WaitOne();
							var result1 = sut.GetOrAddAsync(i1, async () => await Task.FromResult(value.ToString()));
							var result2 = sut.GetOrAddAsync(i1, async () => await Task.FromResult(value.ToString()));
							var result3 = sut.GetOrAddAsync(i1, async () => await Task.FromResult(value.ToString()));
							Assert.IsNotNull(await result1);
							Assert.IsNotNull(await result2);
							Assert.IsNotNull(await result3);
						}
					));
			}

			var signal = Context.SignalWhenNumItemsSent(nameof(CanAddItemsConcurrently),numItems);

			mre.Set();
			await Task.WhenAll(tasks);
			if (!signal.Wait(TimeSpan.FromSeconds(30)))
			{
				Assert.Fail("items not sent");
			}
			var assertItemsAreInRedis = Context.AssertItemsAreInRedis(nameof(CanAddItemsConcurrently),numItems, i => i.ToString());
			for (var i = 0; i < numItems; i++)
				Assert.IsTrue(await sut.ContainsKeyAsync<string, string>(Context.GetKey(nameof(CanAddItemsConcurrently), i)), $"it does not contain {i}");

			await Task.WhenAll(assertItemsAreInRedis);
		}



		[Test]
		public async Task ClearCacheRemovesOnlyTestingItemsAndDoesNotCollideWithOtherCaches()
		{
			using (var anotherCache = GetAnotherClient())
			{
				var anotherKey = Context.Fixture.Create<string>();
				var anotherValue = Context.Fixture.Create<string>();
				await anotherCache.SetAsync(anotherKey, anotherValue.ToByteArray(),
					new DistributedCacheEntryOptions
					{
						AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5)
					});
				var theKey = Context.GetKey(nameof(ClearCacheRemovesOnlyTestingItemsAndDoesNotCollideWithOtherCaches),0);
				var sut = Context.Sut;
				var signal = Context.SignalWhenNumItemsSent(nameof(ClearCacheRemovesOnlyTestingItemsAndDoesNotCollideWithOtherCaches),1);

				await sut.GetOrAddAsync(theKey, Context.Fixture.Create<string>());
				if (!signal.Wait(TimeSpan.FromSeconds(30)))
				{
					Assert.Fail("items not sent");
				}
				await Context.AssertItemIsInRedis<string, string>(theKey);

				//assert it does not collide
				Assert.IsNull(await anotherCache.GetAsync(theKey));
				await sut.ClearAsync();
				var actual = await anotherCache.GetAsync(anotherKey);
				Assert.IsNotNull(actual);
				Assert.AreEqual(anotherValue, actual.ToObject<string>());
			}
			RedisCache GetAnotherClient()
			{
				var anotherInstanceName = $"{nameof(ClearCacheRemovesOnlyTestingItemsAndDoesNotCollideWithOtherCaches)}_{Guid.NewGuid()}";
				var cache = new RedisCache(new OptionsWrapper<RedisCacheOptions>(new RedisCacheOptions
				{
					Configuration = Context.ConnectionString,
					InstanceName = anotherInstanceName
				}));

				return cache;
			}
		}


		[Test]
		public async Task CanExpireItems()
		{
			Context.Expiration = TimeSpan.FromSeconds(3);
			var sut = Context.Sut;

			var numItems = 5;
			var signal = Context.SignalWhenNumItemsSent(nameof(CanExpireItems),numItems);
			for (var i = 0; i < numItems; i++)
			{
				await sut.GetOrAddAsync(Context.GetKey(nameof(CanExpireItems),i), async () => await Task.FromResult(i.ToString()));
			}
			if (!signal.Wait(TimeSpan.FromSeconds(30)))
			{
				Assert.Fail("items not sent");
			}
			for (var i = 0; i < numItems; i++)
			{
				var key = Context.GetKey(nameof(CanExpireItems),i);
				var redisTask = Context.AssertItemIsInRedis(key, i.ToString());
				Assert.IsTrue(await sut.ContainsKeyAsync<string, string>(key));
				await redisTask;
			}
			await Task.Delay(Context.Expiration);

			for (var i = 0; i < numItems; i++)
			{
				var key = Context.GetKey(nameof(CanExpireItems),i);
				var redisTask = Context.AssertItemNotIsInRedis<string, string>(key);
				Assert.IsFalse(await sut.ContainsKeyAsync<string, string>(key));
				await redisTask;
			}
		}

		[Test]
		public async Task OtherAppsUsingSameCacheWontInterfere()
		{
			Context.Expiration = TimeSpan.FromSeconds(300);
			var sut = Context.Sut;
			//add one
			var anotherApp = Context.GetAnotherClientOfSameCacheButDifferentApp();
			var anotherOfSameApp = Context.GetAnotherClientOfTheSameCacheApp();

			TimeSpan absoluteExpiration = TimeSpan.FromSeconds(10);
			var expectedKey = $"key-from-same-app-but-different-instance{Guid.NewGuid()}";
			var expectedItem = await anotherOfSameApp.SetTestRawItem(expectedKey, Context.Fixture.Create<string>(), absoluteExpiration);

			var unExpectedKey = $"key-from-different-app-and-different-instance{Guid.NewGuid()}";
			var unExpectedItem = await anotherApp.SetTestRawItem(unExpectedKey, Context.Fixture.Create<string>(), absoluteExpiration);

			//allow update
			await Task.Delay(TimeSpan.FromSeconds(3));

			Assert.IsTrue(await sut.ContainsKeyAsync<string, string>(expectedKey));
			Assert.IsFalse(await sut.ContainsKeyAsync<string, string>(unExpectedKey));

		}

		[Test]
		public async Task AddingAndRemovingItemUpdatesRedis()
		{
			var sut = Context.Sut;

			var numItems = 5;
			var signal = Context.SignalWhenNumItemsSent(nameof(AddingAndRemovingItemUpdatesRedis),numItems);
			for (var i = 0; i < numItems; i++)
			{
				var result = await sut.GetOrAddAsync(Context.GetKey(nameof(AddingAndRemovingItemUpdatesRedis),i), async () => await Task.FromResult(i.ToString()));
				Assert.AreEqual(i.ToString(), result);
			}
			if (!signal.Wait(TimeSpan.FromSeconds(60)))
			{
				Assert.Fail("items not sent");
			}
			for (var i = 0; i < numItems; i++)
			{
				await Context.AssertItemIsInRedis(Context.GetKey(nameof(AddingAndRemovingItemUpdatesRedis),i), i.ToString());
			}
			signal = Context.SignalWhenNumItemsRemoved(numItems);
			for (var i = 0; i < numItems; i++)
			{
				await sut.InvalidateAsync(default(CancellationToken), i);
			}
			if (!signal.Wait(TimeSpan.FromSeconds(60)))
			{
				Assert.Fail("items not invalidated");
			}
			for (var i = 0; i < numItems; i++)
			{
				await Context.AssertItemNotIsInRedis<string, string>(Context.GetKey(nameof(AddingAndRemovingItemUpdatesRedis),i));
			}
		}

		[TestCase(2)]
		public async Task AddingItemsLocallyThenUpdating_ItSyncsRemotely(int numCases)
		{
			Context.Expiration = TimeSpan.FromSeconds(300);
			var sut = Context.Sut;
			var anotherClient = Context.GetAnotherClientOfTheSameCacheApp();
			await Task.WhenAll(Enumerable.Range(1, numCases).Select(_ =>
				ExecuteSingle(sut, anotherClient)));

			async Task ExecuteSingle(RedisCacheProvider testSut, TestRawRedisCache rawClient)
			{
				var key = Context.Fixture.Create<string>();
				var value = Context.Fixture.Create<string>();
				var signal = Context.SignalWhenItemsSent(keys => keys.Any(k => k.Contains(key)));
				await testSut.GetOrAddAsync(key, value);

				if (!signal.Wait(TimeSpan.FromSeconds(60)))
				{
					Assert.Fail("items not sent");
				}

				var cacheKey = new CacheKey<string, string>(key);
				var entry = await rawClient.GetAsync(cacheKey);
				Assert.IsNotNull(entry);
				var actual = await entry.CacheDeserializeAsync();
				Assert.AreEqual(value, actual.ItemAs<string>());
			}
		}
		[TestCase(1)]
		[TestCase(3)]
		public async Task RemoteOperations_SyncsLocally(int numCases)
		{
			Context.Expiration = TimeSpan.FromSeconds(300);
			var sut = Context.Sut;
			//this will ensure the initialization
			await sut.InvalidateAsync(null, CancellationToken.None, "no-valid-key");
			//add one
			var anotherCache = Context.GetAnotherClientOfTheSameCacheApp();

			await Task.WhenAll(Enumerable.Range(1, numCases)
				.Select(async (_, idx) => await AddTheItems(anotherCache, sut, GetItemId(idx)))
				.Select(async (userKey, idx) => await UpdateTheItems(await userKey, anotherCache, sut, GetItemId(idx)))
				.Select(async (userKey, idx) => await ExpireTheItems(await userKey, anotherCache, GetItemId(idx))));
			async Task<string> ExpireTheItems(string userKey, TestRawRedisCache otherCache, int itemId)
			{
				using (var signal = Context.SignalWhenItemsRemovedRemotely(keys => keys.Any(k => k.Contains(userKey))))
				{
					var anotherValue = await otherCache.SetTestRawItem(userKey,
						$"TestExpiration value item# {itemId}", TimeSpan.FromSeconds(2));
					//wait for expiration
					if (!signal.Wait(TimeSpan.FromSeconds(30)))
					{
						Assert.Fail($"Could not receive a notification of the expired value on time:{DateTime.UtcNow} item# {itemId}");
					}

					Assert.IsNull(
						await otherCache.GetAsync(new CacheKey<string, string>(userKey)),
						$"item# {itemId}");

					Assert.IsFalse(await ResilientOperations.Default.RetryIfNeeded(
						async () => await Context.InMemoryCacheProvider.ContainsKeyAsync<string, string>(userKey)
						? throw new Exception($"{DateTime.UtcNow} - retry because it still contains key {userKey}")
						: false,
						maxAttempts: 5, waitBetweenAttempts: TimeSpan.FromSeconds(1)), $"item# {itemId}");

					return userKey;
				}
			}
			async Task<string> UpdateTheItems(string userKey, TestRawRedisCache otherCache,
				RedisCacheProvider sutCacheProvider, int itemId)
			{
				using (var signal = Context.SignalWhenItemsSetRemotely(keys => keys.Any(k => k.Contains(userKey))))
				{
					var anotherValue = await otherCache.SetTestRawItem(userKey,
						$"Updated value item# {itemId}", TimeSpan.FromSeconds(200));

					if (!signal.Wait(TimeSpan.FromSeconds(30)))
					{
						Assert.Fail($"Could not receive a notification of the updated value on time:{DateTime.UtcNow} item# {itemId}");
					}

					var expectedValue = anotherValue.ItemAs<string>();
					Assert.IsTrue(await ResilientOperations.Default.RetryIfNeeded(
						async () => expectedValue == await sutCacheProvider.GetOrAddAsync(userKey, "wrong value")
							? true
							: throw new Exception("retry"),
						maxAttempts: 5, waitBetweenAttempts: TimeSpan.FromSeconds(1)), $"item# {itemId}");
				}

				return userKey;
			}

			async Task<string> AddTheItems(TestRawRedisCache otherCache,
				RedisCacheProvider sutCacheProvider, int itemId)
			{
				var userKey = itemId.ToString();

				using (var signal = Context.SignalWhenItemsSetRemotely(keys => keys.Any(k => k.Contains(userKey))))
				{
					var anotherValue = await otherCache.SetTestRawItem(userKey,
						$"First value item# {itemId}", TimeSpan.FromSeconds(120));


					if (!signal.Wait(TimeSpan.FromSeconds(30)))
					{
						Assert.Fail($"Could not receive a notification of the added value on time:{DateTime.UtcNow} item# {itemId}");
					}


					Assert.IsTrue(await ResilientOperations.Default.RetryIfNeeded(
						async () =>
						{
							var containsKeyAsync =
								await Context.InMemoryCacheProvider.ContainsKeyAsync<string, string>(userKey);
							if (!containsKeyAsync) throw new Exception("retry");
							return containsKeyAsync;
						},
						maxAttempts: 5, waitBetweenAttempts: TimeSpan.FromSeconds(1)), $"item# {itemId}");
					Assert.IsTrue(await sutCacheProvider.ContainsKeyAsync<string, string>(userKey), $"item# {itemId}");

					var itemFromMemory = await sutCacheProvider.GetOrAddAsync(userKey, "wrong value");
					Assert.AreEqual(anotherValue.Item.ToString(), itemFromMemory, $"item# {itemId}");
				}

				return userKey;
			}

			int GetItemId(int idx)
			{
				return numCases * 100 + idx;
			}
		}





	}
}