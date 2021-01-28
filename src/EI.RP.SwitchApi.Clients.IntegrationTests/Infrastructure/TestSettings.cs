using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using EI.RP.CoreServices.Azure.Configuration;
using EI.RP.CoreServices.Caching;
using EI.RP.CoreServices.Secrets;
using EI.RP.CoreServices.System;
using EI.RP.DataStore;
using Microsoft.Extensions.Configuration;

namespace EI.RP.SwitchApi.Clients.IntegrationTests.Infrastructure
{
	class TestSettings : ISwitchApiSettings
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

		public TimeSpan ExpireCacheItemsWhenNotUsedFor =>
			TimeSpan.Parse(Configuration["Platform:Caching:ExpireCacheItemsWhenNotUsedFor"]);

		public bool HealthChecksEnabled => bool.Parse(Configuration["Platform:Health:ChecksEnabled"]);

		public IEnumerable<string> ReservedIban =>
			Configuration.GetSection("ReservedIban:ElectricIrelandIban").Value.Split(",");

		public string ResidentialPortalDataSourceBaseUrl =>
			Configuration["DataServicesSettings:ResidentialPortalDataSourceSettings:BaseUrl"];

		

		public string SapBaseUrl => Configuration["DataServicesSettings:SapSettings:BaseUrl"];

		public string ErpUtilitiesUmcEndpoint =>
			Configuration["DataServicesSettings:SapSettings:ErpUtilitiesUmcEndpoint"];

		public string UserManagementEndpoint =>
			Configuration["DataServicesSettings:SapSettings:UserManagementEndpoint"];

		public string CrmUtilitiesUmcUrmEndPoint =>
			Configuration["DataServicesSettings:SapSettings:CrmUtilitiesUmcUrmEndPoint"];

		public string CrmUtilitiesUmcEndPoint =>
			Configuration["DataServicesSettings:SapSettings:CrmUtilitiesUmcEndPoint"];

		public double BatchEnlistTimeoutMilliseconds =>
			double.Parse(Configuration["DataServicesSettings:SapSettings:BatchEnlistTimeoutMilliseconds"]);


		public TimeSpan RequestTimeout =>
			TimeSpan.FromSeconds(
				double.Parse(Configuration["DataServicesSettings:SapSettings:RequestTimeoutSeconds"]));

		public string StreamServeLive => Configuration["DataServicesSettings:StreamServe:BaseUrl"];
		public string StreamServeArchive => Configuration["DataServicesSettings:StreamServe:ArchiveBaseUrl"];

		public string SwitchApiEndPoint => Configuration["DataServicesSettings:SwitchApiSettings:BaseUrl"];
		public async Task<string> SwitchApiBearerTokenProviderUrlAsync()=> await GetSecretAsync("DataServicesSettings-SwitchApiSettings-SwitchApiTokenUrl");
		public bool IsPromotionsEnabled => bool.Parse(Configuration["UiSettings:UiFeatures:Promotions:Enabled"]);

		public bool IsCompetitionEnabled => bool.Parse(Configuration["UiSettings:UiFeatures:Competitions:Enabled"]);
		public string CompetitionName => Configuration["UiSettings:UiFeatures:Competitions:Name"];
		public string CompetitionHeading => Configuration["UiSettings:UiFeatures:Competitions:Heading"];
		public string CompetitionDescription => Configuration["UiSettings:UiFeatures:Competitions:Description"];
		public string CompetitionDescription1 => Configuration["UiSettings:UiFeatures:Competitions:Description1"];
		public string CompetitionDescription2 => Configuration["UiSettings:UiFeatures:Competitions:Description2"];
		public string CompetitionDescription3 => Configuration["UiSettings:UiFeatures:Competitions:Description3"];
		public string CompetitionQuestion => Configuration["UiSettings:UiFeatures:Competitions:Question"];
		public string CompetitionAnswerA => Configuration["UiSettings:UiFeatures:Competitions:AnswerA"];
		public string CompetitionAnswerB => Configuration["UiSettings:UiFeatures:Competitions:AnswerB"];
		public string CompetitionAnswerC => Configuration["UiSettings:UiFeatures:Competitions:AnswerC"];

		public string CompetitionTermAndConditionsUrl =>
			Configuration["UiSettings:UiFeatures:Competitions:TermAndConditionsUrl"];

		public string CompetitionAlreadyEnteredMessage => Configuration["UiSettings:UiFeatures:Competitions:AlreadyEnteredMessage"];

		public string PromotionHeading => Configuration["UiSettings:UiFeatures:Promotions:Heading"];
		public string PromotionDescription1 => Configuration["UiSettings:UiFeatures:Promotions:Description1"];
		public string PromotionDescription2 => Configuration["UiSettings:UiFeatures:Promotions:Description2"];
		public string PromotionLinkText => Configuration["UiSettings:UiFeatures:Promotions:LinkText"];
		public string PromotionLinkURL => Configuration["UiSettings:UiFeatures:Promotions:LinkURL"];
		public string PromotionPageTitle => Configuration["UiSettings:UiFeatures:Promotions:PageTitle"];
		public string PromotionDescription3 => Configuration["UiSettings:UiFeatures:Promotions:Description3"];
		public string PromotionDescription4 => Configuration["UiSettings:UiFeatures:Promotions:Description4"];

		public string PromotionTermsConditionsLinkText =>
			Configuration["UiSettings:UiFeatures:Promotions:TermsConditionsLinkText"];

		public string PromotionTermsConditionsLinkURL =>
			Configuration["UiSettings:UiFeatures:Promotions:TermsConditionsLinkURL"];

		public string LogsRoot => Configuration["UiSettings:UiFeatures:LogsViewer:Path"];
		public bool LogViewerEnabled => bool.Parse(Configuration["UiSettings:UiFeatures:LogsViewer:Enabled"]);
		public bool FlowDebuggerIsEnabled => bool.Parse(Configuration["UiSettings:UiFeatures:FlowDebuggerIsEnabled"]);
		public string ShopBaseUrl => Configuration["UiSettings:ShopBaseUrl"];
		public string StoreBaseUrl => Configuration["UiSettings:StoreBaseUrl"];
		public string ElectricIrelandBaseUrl => Configuration["UiSettings:ElectricIrelandBaseUrl"];

	

		public bool EncryptUrls =>
			bool.Parse(Configuration["Platform:Encryption:EncryptUrls"]);
	}
}