using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Azure;
using Azure.Identity;
using Castle.DynamicProxy.Generators;
using EI.RP.CoreServices.Azure.Configuration;
using EI.RP.CoreServices.Azure.Secrets;
using EI.RP.CoreServices.Caching;
using EI.RP.CoreServices.Caching.InMemory;
using EI.RP.CoreServices.Caching.Models;
using EI.RP.CoreServices.Http.Clients;
using EI.RP.CoreServices.Profiling;
using EI.RP.TestServices;
using EI.RP.TestServices.Profilers;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;

namespace EI.RP.CoreServices.Azure.IntegrationTests.Secrets
{

	[Parallelizable(ParallelScope.Fixtures)]
	[TestFixture]
	partial class AzureSecretsRepositoryTests:UnitTestFixture<AzureSecretsRepositoryTests.TestContext, AzureSecretsRepository>
	{
		[Test]
		public async Task CanWriteReadAndDeleteAsync()
		{
			var key = $"IntegrationTest-{nameof(AzureSecretsRepository)}-{Context.Fixture.Create<string>()}";
			var value = Context.Fixture.Create<string>();

			Exception ex = null;
			try
			{
				await Context.Sut.SetAsync(key, value);
				Console.WriteLine($"await Context.Sut.SetAsync({key}, {value});");
				var actual = await Context.Sut.GetAsync(key, 3);
				Console.WriteLine($"await Context.Sut.GetAsync({key});");
				Assert.AreEqual(value, actual);

				Assert.IsTrue(await Context.Sut.ContainsAsync(key));
				Console.WriteLine($"await Context.Sut.ContainsAsync({key});");
			}
			catch (RequestFailedException e)
			{
				Console.WriteLine($"Error code:{e.ErrorCode} - Status:{e.Status} - {e.Message} - dump: {e}");
				ex = e;
			}
			catch (Exception e)
			{
				ex = e;
			}
			finally
			{
				if (ex == null)
				{

					await Context.Sut.RemoveAsync(key);
					Console.WriteLine($"await Context.Sut.RemoveAsync({key});");
					Assert.IsFalse(await Context.Sut.ContainsAsync(key));
					Console.WriteLine($"await Context.Sut.ContainsAsync2({key});");
				}
				if (ex != null) throw ex;
			}

			Assert.ThrowsAsync<KeyNotFoundException>(async () => await Context.Sut.GetAsync(key, 1));
		}
		[Test]
		public async Task CanExpireDataFromCache()
		{
			var key = $"IntegrationTest-{nameof(AzureSecretsRepository)}-{Context.Fixture.Create<string>()}";

			
			var cachedKey = new CacheKey<string,string>(AzureSecretsRepository.InstanceId,key);
			
			
			var value = Context.Fixture.Create<string>();
			var maxDuration = TimeSpan.FromSeconds(3);
			Context.WithMaxDuration(maxDuration);
			Exception ex = null;
			try
			{
				await Context.Sut.SetAsync(key, value);
				
				var item= await Context.Sut.GetAsync(key, 3);
				Assert.IsNotNull(item);

				var cached = await Context.Cache.GetAsync(cachedKey);
				Assert.IsNotNull(cached);

				Assert.IsTrue(await Context.Sut.ContainsAsync(key));
				
				await Task.Delay(maxDuration.Add(TimeSpan.FromSeconds(1)));
				cached = await Context.Cache.GetAsync(cachedKey);
				Assert.IsNull(cached);
				Assert.IsTrue(await Context.Sut.ContainsAsync(key));
			}
			catch (Exception e)
			{
				ex = e;
				Console.WriteLine(e.ToString());
			}
			finally
			{
				if (ex == null)
				{
					try
					{
						await Context.Sut.RemoveAsync(key);
						Assert.IsFalse(await Context.Sut.ContainsAsync(key));
					}
					catch (Exception e)
					{
						ex = e;
						Console.WriteLine(e.ToString());
					}
				}
				if (ex != null) throw ex;
			}
			Assert.ThrowsAsync<KeyNotFoundException>(async () => await Context.Sut.GetAsync(key, 1));
		}

		//[Explicit("This is a tool")]
		//[Test]
		//public async Task SetValue()
		//{
		//	await Context.Sut.SetAsync(
		//		"DataServicesSettings-Redis-ConnectionString"
		//		, "eiresportalroi-pre-rdc-01.redis.cache.windows.net:6380,password=dv4Yzso+nlgnNEScaCByJYwR1A0YcvLSU6AEq8P3BA4=,ssl=True,abortConnect=False");
		//}

		public class TestContext : UnitTestContext<AzureSecretsRepository>
		{
			private string _keyVaultUrl = 
				//"https://resportalroi-dev-vlt-01.vault.azure.net/";
				"https://resportalroi-pre-vlt-01.vault.azure.net/";
			private TimeSpan _maxDuration=TimeSpan.FromHours(1);

			public MemoryDistributedCache Cache { get; } =
				new MemoryDistributedCache(new CacheOptions());

			public TestContext()
			{
			}
			class CacheOptions : IOptions<MemoryDistributedCacheOptions>
			{
				public MemoryDistributedCacheOptions Value => new MemoryDistributedCacheOptions(){ExpirationScanFrequency = TimeSpan.FromMilliseconds(100)};
			}
			protected override AzureSecretsRepository BuildSut(AutoMocker autoMocker)
			{
				Console.WriteLine($"creating repository...");
				var repository = new AzureSecretsRepository(
					BuildKeyVaultSettings().Object,
					new InMemoryCacheProvider(BuildCacheSettings().Object, Cache),
					new DefaultHttpClientBuilder(new ConsoleProfiler(0)),
					new ClientSecretCredential(
						"fb01cb1d-bba8-4c1a-94ef-defd79c59a09",
						"fffaa3d9-f300-4ba4-83cf-ce2b896dbfa9",
						"KimPQ_-H0yOAcwVx_P-R2hS9Wnn4xrG6-Y"),new NoProfiler());
				Console.WriteLine($"repository created!");
				return repository;

				Mock<IAzureKeyVaultSettings> BuildKeyVaultSettings()
				{
					var mock = autoMocker
						.GetMock<IAzureKeyVaultSettings>();
					mock
						.SetupGet(x => x.KeyVaultUrl)
						.Returns(_keyVaultUrl);
					mock
						.SetupGet(x => x.KeyVaultCacheDuration)
						.Returns(_maxDuration);
					return mock;
				}
				Mock<ICacheSettings> BuildCacheSettings()
				{
					var mock = autoMocker
						.GetMock<ICacheSettings>();
					mock
						.SetupGet(x => x.CacheProviderType)
						.Returns(CacheProviderType.InMemory);
					mock
						.SetupGet(x => x.ExpireCacheItemsWhenNotUsedFor)
						.Returns(TimeSpan.FromSeconds(10));
					mock
						.SetupGet(x => x.IsCachePreLoaderEnabled)
						.Returns(false);
					mock
						.SetupGet(x => x.IsCacheEnabled)
						.Returns(true);
					return mock;
				}
			}

			public TestContext WithMaxDuration(TimeSpan maxDuration)
			{
				this._maxDuration = maxDuration;
				return this;
			}
		}
	}
}