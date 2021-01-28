using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Identity;
using EI.RP.CoreServices.Azure.Configuration;
using EI.RP.CoreServices.Azure.Infrastructure.Credentials;
using EI.RP.CoreServices.Azure.Infrastructure.Credentials.Strategies;
using EI.RP.CoreServices.Azure.Infrastructure.Http;
using EI.RP.CoreServices.Azure.Secrets;
using EI.RP.CoreServices.Azure.Tokens;
using EI.RP.CoreServices.Caching;
using EI.RP.CoreServices.Caching.InMemory;
using EI.RP.CoreServices.Http.Clients;
using EI.RP.CoreServices.Profiling;
using EI.RP.CoreServices.System;
using EI.RP.TestServices.Profilers;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace EI.RP.CoreServices.Azure.IntegrationTests.Tokens
{
	[Parallelizable(ParallelScope.All)]
	[TestFixture]
	class AzureResourceTokenProviderTests
	{
		class CacheOptions : IOptions<MemoryDistributedCacheOptions>
		{
			public MemoryDistributedCacheOptions Value => new MemoryDistributedCacheOptions()
				{ExpirationScanFrequency = TimeSpan.FromMilliseconds(100)};
		}

		public static IEnumerable<TestCaseData> CanGetTokenCases()
		{
			var keyVaultUrls = new[]
			{
				"https://resportalroi-dev-vlt-01.vault.azure.net/",
				// These test the same as the next"https://rsportal-dev-demo-vlt-01.vault.azure.net/",
				"https://resportalroi-tst-vlt-01.vault.azure.net/",
				"https://resportalroi-pre-vlt-01.vault.azure.net/"
			};
			var secretNames = new[]
			{
				"DataServicesSettings-SapSettings-SapUserManagementApiTokenUrl",

				"DataServicesSettings-SapSettings-SapCrmUmcUrmApiTokenUrl",
				"DataServicesSettings-SapSettings-SapCrmUmcApiTokenUrl",
				"DataServicesSettings-SapSettings-SapErpUmcApiTokenUrl",
				"DataServicesSettings-StreamServe-StreamServeCurrentApiTokenUrl",
				"DataServicesSettings-EventsPublisherSettings-CmdmApiTokenUrl",
				"DataServicesSettings-SwitchApiSettings-SwitchApiTokenUrl",
				"DataServicesSettings-MprnApiSettings-MprnApiTokenUrl",
			};
			foreach (var keyVaultUrl in keyVaultUrls)
			{
				var repository = BuildRepo(keyVaultUrl);
				var settings = BuildTokenSettings().Object;
				foreach (var secretName in secretNames)
				{
					var endpoint = repository.GetAsync(secretName, 3).GetAwaiter().GetResult();
					yield return new TestCaseData(endpoint, settings).SetName(
						$"Url:{endpoint} <- {keyVaultUrl} - {secretName}");
				}
			}

			Mock<IAzureResourceTokenSettings> BuildTokenSettings()
			{
				var mock = new Mock<IAzureResourceTokenSettings>();
				mock
					.SetupGet(x => x.BearerTokenCacheDuration)
					.Returns(TimeSpan.FromSeconds(10));

				return mock;
			}

			AzureSecretsRepository BuildRepo(string url)
			{
				IAzureKeyVaultSettings settings = BuildKeyVaultSettings().Object;
				var repository = new AzureSecretsRepository(
					settings,
					new InMemoryCacheProvider(BuildCacheSettings().Object,
						new MemoryDistributedCache(new CacheOptions())),
					new DefaultHttpClientBuilder(new ConsoleProfiler(0)),
					new ClientSecretCredential(
						"fb01cb1d-bba8-4c1a-94ef-defd79c59a09",
						"fffaa3d9-f300-4ba4-83cf-ce2b896dbfa9",
						"KimPQ_-H0yOAcwVx_P-R2hS9Wnn4xrG6-Y"), new NoProfiler());

				return repository;

				Mock<IAzureKeyVaultSettings> BuildKeyVaultSettings()
				{
					var mock = new Mock<IAzureKeyVaultSettings>();
					mock
						.SetupGet(x => x.KeyVaultUrl)
						.Returns(url);
					mock
						.SetupGet(x => x.KeyVaultCacheDuration)
						.Returns(TimeSpan.FromHours(1));
					return mock;
				}

				Mock<ICacheSettings> BuildCacheSettings()
				{
					var mock = new Mock<ICacheSettings>();
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
		}

		[TestCaseSource(nameof(CanGetTokenCases))]
		public async Task CanGetToken(string resourceName, IAzureResourceTokenSettings settings)
		{
			var mock = new Mock<IAzureCredentialSettings>();
			mock.SetupGet(x => x.CredentialType).Returns(CredentialType.ServicePrincipal);
			mock.SetupGet(x => x.Tenant).Returns("fb01cb1d-bba8-4c1a-94ef-defd79c59a09");
			mock.SetupGet(x => x.ClientId).Returns("fffaa3d9-f300-4ba4-83cf-ce2b896dbfa9");
			mock.SetupGet(x => x.ClientSecret).Returns("KimPQ_-H0yOAcwVx_P-R2hS9Wnn4xrG6-Y");


			var azureCredentialSettings = mock.Object;
			var credentialsProvider = new AzureCredentialsProvider(azureCredentialSettings,new ServicePrincipalProvider(azureCredentialSettings).ToOneItemArray());
			var azureSettingsMock = new Mock<IAzureGeneralSettings>();
				azureSettingsMock.SetupGet(x=>x.IsAzureEnabled)
					.Returns(true);

				var sut = new AzureResourceTokenProvider(new NoCacheProvider(), settings, credentialsProvider,
					azureSettingsMock.Object, new AzureHttpClientBuilder(	new DefaultHttpClientBuilder(new ConsoleProfiler(0))));
			var actual =
				await sut.GetToken(resourceName);
			Assert.IsFalse(string.IsNullOrWhiteSpace(actual));
		}


	}
}
