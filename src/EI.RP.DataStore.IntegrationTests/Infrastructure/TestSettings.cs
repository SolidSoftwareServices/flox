using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using EI.RP.CoreServices.Azure.Configuration;
using EI.RP.CoreServices.Caching;
using EI.RP.CoreServices.Secrets;
using EI.RP.CoreServices.System;
using Microsoft.Extensions.Configuration;

namespace EI.RP.DataStore.IntegrationTests.Infrastructure
{
	class TestSettings : IResidentialPortalDataSourceSettings
		,IAzureResourceTokenSettings
		,IAzureCredentialSettings
	,IAzureKeyVaultSettings
	
	{
		private readonly Func<ISecretsRepository> _secretsRepository;
		
		public TestSettings(string environment, Func<ISecretsRepository> secretsRepository)
		{
			_secretsRepository=secretsRepository;

			var rawPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
				,"../../../../EI.RP.WebApp");
			var dirPath = Path.GetFullPath(rawPath);

			Configuration = ConfigureEnvironmentSettings().Build();

			IConfigurationBuilder ConfigureEnvironmentSettings()
			{
				var appConfig = new ConfigurationBuilder()
					.SetBasePath(dirPath)
					.AddJsonFile($"appsettings.{environment}.json", true)
					.AddEnvironmentVariables();
				return appConfig;
			}
		}

		public IConfiguration Configuration { get; }

		
		private async Task<string> GetSecretAsync(string key)
		{
			var result = await _secretsRepository().GetAsync(key, 3);

			return result;
		}
		public bool IsAzureEnabled => bool.Parse(Configuration["CloudSettings:Azure:Enabled"]);
		public CredentialType CredentialType => Configuration["CloudSettings:Azure:Credentials:CredentialType"].ToEnum<CredentialType>();

		public string Tenant => Configuration["CloudSettings:Azure:Credentials:Tenant"];

		public string ClientId => Configuration["CloudSettings:Azure:Credentials:ClientId"];

		public string ClientSecret => Configuration["CloudSettings:Azure:Credentials:Secret"];

		public string KeyVaultUrl => Configuration["CloudSettings:Azure:KeyVault:KeyVaultUrl"];

		public TimeSpan KeyVaultCacheDuration => TimeSpan.Parse(Configuration["CloudSettings:Azure:KeyVault:KeyVaultCacheDuration"]);

		public TimeSpan BearerTokenCacheDuration => TimeSpan.Parse(Configuration["CloudSettings:Azure:Apis:BearerTokenCacheDuration"]);
		public async Task<string> ApiMSubscriptionKeyAsync()
		{
			return await GetSecretAsync("PublicPortal-DataServicesSettings-SapSettings-OcpApimSubscriptionKey");
		}

		public bool UseMockEventsPublisher
		{
			get
			{
				var value = Configuration["DataServicesSettings:EventsPublisherSettings:UseMock"];
				if (!bool.TryParse(value, out var result)) return false;

				return result;
			}
		}

	

		public bool IsCacheEnabled => bool.Parse(Configuration["Platform:Caching:IsDomainCacheEnabled"]);

		public bool IsCachePreLoaderEnabled =>
			IsCacheEnabled && bool.Parse(Configuration["Platform:Caching:IsCachePreLoaderEnabled"]);

		public CacheProviderType CacheProviderType =>
			Configuration["Platform:Caching:CacheType"].ToEnum<CacheProviderType>();

	

		public string ResidentialPortalDataSourceBaseUrl =>
			Configuration["DataServicesSettings:ResidentialPortalDataSourceSettings:BaseUrl"];

		public async Task<string> ResidentialPortalDataSourceBearerTokenProviderUrlAsync() =>
			await GetSecretAsync("DataServicesSettings-EventsPublisherSettings-CmdmApiTokenUrl");

	


		public TimeSpan RequestTimeout =>
			TimeSpan.FromSeconds(
				double.Parse(Configuration["DataServicesSettings:SapSettings:RequestTimeoutSeconds"]));

		
	}
}