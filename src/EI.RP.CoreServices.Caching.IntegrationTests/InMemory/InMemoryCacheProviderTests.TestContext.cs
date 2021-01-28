using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.CoreServices.Caching.InMemory;
using EI.RP.CoreServices.Caching.Models;
using EI.RP.CoreServices.Serialization;
using EI.RP.CoreServices.System;
using EI.RP.CoreServices.System.Async;
using EI.RP.TestServices;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using Moq.AutoMock;

namespace EI.RP.CoreServices.Caching.IntegrationTests.InMemory
{
	partial class InMemoryCacheProviderTests
	{
		public class TestContext : UnitTestContext<InMemoryCacheProvider>
		{
			class MemoryCacheOptions : IOptions<MemoryDistributedCacheOptions>
			{
				public MemoryDistributedCacheOptions Value => new MemoryDistributedCacheOptions()
				{
					ExpirationScanFrequency = TimeSpan.FromMilliseconds(100)
				};
			}

			public MemoryDistributedCache Cache { get; } =
				new MemoryDistributedCache(new MemoryCacheOptions());

			public TimeSpan Expiration
			{
				get;
				set;
			} = TimeSpan.FromSeconds(3);

			protected override InMemoryCacheProvider BuildSut(AutoMocker autoMocker)
			{
				return new InMemoryCacheProvider(BuildCacheSettings().Object, new AsyncLazy<MemoryDistributedCache>(Cache));

				Mock<ICacheSettings> BuildCacheSettings()
				{
					var mock = autoMocker
						.GetMock<ICacheSettings>();
					mock
						.SetupGet(x => x.CacheProviderType)
						.Returns(CacheProviderType.InMemory);
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
			private readonly ConcurrentHashSet<int> _pendingItems=new ConcurrentHashSet<int>();
			public ManualResetEventSlim SignalWhenNumItemsSent(int numItems)
			{
				_pendingItems.AddRange(Enumerable.Range(0,numItems));
				var signal=new ManualResetEventSlim(false);
				
				Sut.UserKeyValueAdded+= (object sender, string key,CachedItem value,DistributedCacheEntryOptions options) =>
				{
					var userKey = CacheKey<string, string>.GetUserKey(key);
					_pendingItems.Remove(int.Parse(userKey));
					if ( !_pendingItems.Any())
					{
						signal.Set();
					}

					return Task.CompletedTask;
				};
				return signal;
			}
		}
	}
}