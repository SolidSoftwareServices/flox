using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Identity;
using EI.RP.CoreServices.Azure.Configuration;
using EI.RP.CoreServices.Azure.Secrets;
using EI.RP.CoreServices.Caching;
using EI.RP.CoreServices.Caching.InMemory;
using EI.RP.CoreServices.Http.Clients;
using EI.RP.CoreServices.Profiling;
using EI.RP.TestServices.Profilers;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace EI.RP.CoreServices.Azure.IntegrationTests.Secrets
{
	internal partial class AzureSecretsRepositoryTests
	{
		private class CacheOptions : IOptions<MemoryDistributedCacheOptions>
		{
			public MemoryDistributedCacheOptions Value => new MemoryDistributedCacheOptions
				{ExpirationScanFrequency = TimeSpan.FromMilliseconds(100)};
		}

		private class SecretType
		{
			public SecretType(string key, bool isPerDeployment)
			{
				IsDedicatedToDeploymentInStage = isPerDeployment;
				Key = key;
			}

			public bool IsDedicatedToDeploymentInStage { get; }
			public string Key { get; }
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
			var expectedSecrets
				= new[]
				{
					new SecretType("DataServicesSettings-SapSettings-SapUserManagementApiTokenUrl", false),

					new SecretType("DataServicesSettings-SapSettings-SapCrmUmcUrmApiTokenUrl", false),
					new SecretType("DataServicesSettings-SapSettings-SapCrmUmcApiTokenUrl", false),
					new SecretType("DataServicesSettings-SapSettings-SapErpUmcApiTokenUrl", false),
					new SecretType("DataServicesSettings-StreamServe-StreamServeCurrentApiTokenUrl", false),

					new SecretType("DataServicesSettings-EventsPublisherSettings-CmdmApiTokenUrl", false),
					new SecretType("DataServicesSettings-SwitchApiSettings-SwitchApiTokenUrl", false),
					new SecretType("DataServicesSettings-MprnApiSettings-MprnApiTokenUrl", false),

					//
					new SecretType("DataServicesSettings-SapSettings-OcpApimSubscriptionKey", true),
					new SecretType("Emails-RecipientEmail", true),
					new SecretType("Emails-AccountQueryRecipientEmail", true),
					new SecretType("EncryptionSettings-PassPhrase", true),
					new SecretType("EncryptionSettings-SaltValue", true),
					new SecretType("EncryptionSettings-InitVector", true),


					new SecretType("PaymentGatewaySettings-Account", true),
					new SecretType("PaymentGatewaySettings-MerchantId", true),
					new SecretType("PaymentGatewaySettings-Secret", true),
					new SecretType("DataServicesSettings-Redis-ConnectionString", false)
				};
			foreach (var keyVaultUrl in keyVaultUrls)
			{
				var repository = BuildRepo(keyVaultUrl);
				var settings = BuildTokenSettings().Object;
				foreach (var secret in expectedSecrets)
					if (secret.IsDedicatedToDeploymentInStage)
					{
						var key = $"PublicPortal-{secret.Key}";
						yield return new TestCaseData(repository, key, settings).SetName($"Url:{keyVaultUrl} - {key}");
						key = $"InternalPortal-{secret.Key}";
						yield return new TestCaseData(repository, key, settings).SetName($"Url:{keyVaultUrl} - {key}");
					}
					else
					{
						yield return new TestCaseData(repository, secret.Key, settings).SetName(
							$"Url:{keyVaultUrl} - {secret.Key}");
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
				var settings = BuildKeyVaultSettings().Object;
				var repository = new AzureSecretsRepository(
					settings,
					new InMemoryCacheProvider(BuildCacheSettings().Object,
						new MemoryDistributedCache(new CacheOptions())),
					new DefaultHttpClientBuilder(new ConsoleProfiler()),
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

		[Parallelizable(ParallelScope.All)]
		[TestCaseSource(nameof(CanGetTokenCases))]
		public async Task CanGetToken(AzureSecretsRepository repository, string resourceName,
			IAzureResourceTokenSettings settings)
		{
			var actual = await repository.GetAsync(resourceName, 3);
			Assert.IsFalse(string.IsNullOrWhiteSpace(actual));
		}
	}
}