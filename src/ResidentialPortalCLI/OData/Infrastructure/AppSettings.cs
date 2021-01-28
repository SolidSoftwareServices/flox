using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.Azure.Configuration;
using EI.RP.CoreServices.Caching;
using EI.RP.CoreServices.Platform;
using EI.RP.CoreServices.Secrets;
using EI.RP.CoreServices.System;
using EI.RP.CoreServices.System.DependencyInjection;
using EI.RP.DataServices.SAP.Clients.Config;
using Microsoft.Extensions.Configuration;

namespace ResidentialPortalCLI.OData.Infrastructure
{
	class AppSettings : ISapSettings, 
		IAzureKeyVaultSettings,
		IAzureResourceTokenSettings, 
		ICacheSettings,
		IAzureCredentialSettings,
		IPlatformSettings
	{
		private readonly IConfiguration _configuration;
		private readonly Lazy<ISecretsRepository> _secretsRepository;

		public AppSettings(IConfiguration configuration, IServiceProvider services)
		{
			_configuration = configuration;
			_secretsRepository = new Lazy<ISecretsRepository>(() => services.Resolve<ISecretsRepository>());
		}
		public string SapBaseUrl => _configuration["DataServicesSettings:SapSettings:BaseUrl"];

		public string ErpUtilitiesUmcEndpoint =>
			_configuration["DataServicesSettings:SapSettings:ErpUtilitiesUmcEndpoint"];

		public string UserManagementEndpoint =>
			_configuration["DataServicesSettings:SapSettings:UserManagementEndpoint"];

		public string CrmUtilitiesUmcUrmEndPoint =>
			_configuration["DataServicesSettings:SapSettings:CrmUtilitiesUmcUrmEndPoint"];

		public string CrmUtilitiesUmcEndPoint =>
			_configuration["DataServicesSettings:SapSettings:CrmUtilitiesUmcEndPoint"];

		public double BatchEnlistTimeoutMilliseconds =>
			double.Parse(_configuration["DataServicesSettings:SapSettings:BatchEnlistTimeoutMilliseconds"]);

		public TimeSpan RequestTimeout =>
			TimeSpan.FromSeconds(double.Parse(_configuration["DataServicesSettings:SapSettings:RequestTimeoutSeconds"]));

		public async Task<string> SapErpUmcBearerTokenProviderUrlAsync() =>
			await GetSecretAsync("DataServicesSettings-SapSettings-SapErpUmcApiTokenUrl", false);

		public async Task<string> SapCrmUmcUrmBearerTokenProviderUrlAsync() =>
			await GetSecretAsync("DataServicesSettings-SapSettings-SapCrmUmcUrmApiTokenUrl", false);

		public async Task<string> SapUserManagementBearerTokenProviderUrlAsync()=>
			await GetSecretAsync("DataServicesSettings-SapSettings-SapUserManagementApiTokenUrl", false);

		public async Task<string> SapCrmUmcBearerTokenProviderUrlAsync()=>
			await GetSecretAsync("DataServicesSettings-SapSettings-SapCrmUmcApiTokenUrl", false);
		
		
		private async Task<string> GetSecretAsync(string key, bool isPrefixed = true)
		{
			string result;
				var keyForEnvironment = isPrefixed ? ResolveSecretKeyForEnvironment(key) : key;
				result =await  _secretsRepository.Value.GetAsync(keyForEnvironment, 3);

			return result;

			 string ResolveSecretKeyForEnvironment(string keyWithoutDeploymentType)
			{

				return $"PublicPortal-{keyWithoutDeploymentType}";
			}
		}

		public bool IsAzureEnabled =>  true;

		public string KeyVaultUrl => _configuration["CloudSettings:Azure:KeyVault:KeyVaultUrl"];

		public TimeSpan KeyVaultCacheDuration => TimeSpan.Parse(_configuration["CloudSettings:Azure:KeyVault:KeyVaultCacheDuration"]);

		public TimeSpan BearerTokenCacheDuration => TimeSpan.Parse(_configuration["CloudSettings:Azure:Apis:BearerTokenCacheDuration"]);
		public async Task<string> ApiMSubscriptionKeyAsync()
		{
			return await GetSecretAsync("DataServicesSettings-SapSettings-OcpApimSubscriptionKey");
		}


		public bool IsCacheEnabled => false;

		public bool IsCachePreLoaderEnabled => throw new NotImplementedException();

		public CacheProviderType CacheProviderType => CacheProviderType.InMemory;

		public TimeSpan ExpireCacheItemsWhenNotUsedFor => throw new NotImplementedException();
		public Task<string> RedisConnectionString()
		{
			throw new NotImplementedException();
		}

		public CredentialType CredentialType => _configuration["CloudSettings:Azure:Credentials:CredentialType"].ToEnum<CredentialType>();

		public string Tenant => _configuration["CloudSettings:Azure:Credentials:Tenant"];

		public string ClientId => _configuration["CloudSettings:Azure:Credentials:ClientId"];

		public string ClientSecret => _configuration["CloudSettings:Azure:Credentials:Secret"];

		public bool IsInternalDeployment => throw new NotImplementedException();

		public bool HealthChecksEnabled => throw new NotImplementedException();

		public bool ProfileInDetail => false;

		public bool ShowDevelopmentTools => throw new NotImplementedException();

		public bool IsSmartActivationEnabled => throw new NotImplementedException();
		public bool IsFullRequestLoggingEnabled => false;
		public bool IsFullResponseLoggingEnabled => false;

		public bool RequireCookiesPolicyCompliance => throw new NotImplementedException();
	}
}