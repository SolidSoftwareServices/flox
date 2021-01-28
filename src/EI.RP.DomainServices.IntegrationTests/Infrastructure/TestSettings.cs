using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using EI.RP.CoreServices.Azure.Configuration;
using EI.RP.CoreServices.Caching;
using EI.RP.CoreServices.Encryption;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.Platform;
using EI.RP.CoreServices.Secrets;
using EI.RP.CoreServices.System;
using EI.RP.CoreServices.System.Async;
using EI.RP.CoreServices.System.DependencyInjection;
using EI.RP.DataServices.SAP.Clients.Config;
using EI.RP.DataServices.StreamServe.Clients;
using EI.RP.DataStore;
using EI.RP.DomainServices.Commands.Platform.SendEmail;
using EI.RP.DomainServices.Infrastructure.Settings;
using EI.RP.DomainServices.Validation;
using EI.RP.SwitchApi.Clients;
using Microsoft.Extensions.Configuration;

namespace EI.RP.DomainServices.IntegrationTests.Infrastructure
{
	class TestSettings : ISapSettings, IEmailSettings,
		ISwitchApiSettings,
		IEncryptionSettings,
		IResidentialPortalDataSourceSettings,
		IPdfOverlayRepositorySettings,
		IStreamServeSettings,
		IAppCookieSettings,
		IPlatformSettings,
		ILegacyStreamServeSettings,
		IAzureResourceTokenSettings,
		IAzureKeyVaultSettings,
		IAzureCredentialSettings,
		IReservedIbanSettings,
		IDomainSettings
	{
		private readonly IConfigurationRoot _configuration;
		public TestSettings(string environmentName, IServiceProvider services)
		{
			var objAppSettingsFileFolder = Path.GetFullPath("../../../../Ei.Rp.WebApp");
			_configuration = new ConfigurationBuilder()
				.SetBasePath(objAppSettingsFileFolder)
				.AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{environmentName}.json", optional: false,
					reloadOnChange: true)
				.Build();

			_secretsRepository = new AsyncLazy<ISecretsRepository>(async() => services.Resolve<ISecretsRepository>());

		}
		public bool IsAzureEnabled => GetConfigurationValue<bool>("CloudSettings:Azure:Enabled");
		public CredentialType CredentialType => GetConfigurationValue("CloudSettings:Azure:Credentials:CredentialType").ToEnum<CredentialType>();

		public string Tenant => GetConfigurationValue("CloudSettings:Azure:Credentials:Tenant");

		public string ClientId => GetConfigurationValue("CloudSettings:Azure:Credentials:ClientId");

		public string ClientSecret => GetConfigurationValue("CloudSettings:Azure:Credentials:Secret");

		public string KeyVaultUrl => GetConfigurationValue("CloudSettings:Azure:KeyVault:KeyVaultUrl");

		public TimeSpan KeyVaultCacheDuration =>GetConfigurationValue<TimeSpan>("CloudSettings:Azure:KeyVault:KeyVaultCacheDuration");

		public TimeSpan BearerTokenCacheDuration =>GetConfigurationValue<TimeSpan>("CloudSettings:Azure:Apis:BearerTokenCacheDuration");

		public async Task<string> ApiMSubscriptionKeyAsync() => await GetSecretAsync("DataServicesSettings-SapSettings-OcpApimSubscriptionKey");

		public IEnumerable<string> AppCookieNames => throw new NotImplementedException();

		public bool UseMockEventsPublisher
		{
			get
			{
				var value = GetConfigurationValue("DataServicesSettings:EventsPublisherSettings:UseMock");
				if (!bool.TryParse(value, out var result)) return false;

				return result;
			}
		}

		public async Task<string> ResidentialOnlineEmailRecipientAsync() => await GetSecretAsync("Emails-RecipientEmail");
		public async Task<string> AccountQueryCcEmailAsync() => await GetSecretAsync("Emails-AccountQueryRecipientEmail");


		public async  Task<string> EncryptionPassPhraseAsync()
		{
			return await GetSecretAsync("EncryptionSettings-PassPhrase");
		}

		public async Task<string> EncryptionSaltValueAsync()
		{
			return await GetSecretAsync("EncryptionSettings-SaltValue");
		}

		public async Task<string> EncryptionInitVectorAsync()
		{
			return await GetSecretAsync("EncryptionSettings-InitVector");
		}

		public async Task<string> SapErpUmcBearerTokenProviderUrlAsync() =>
			await GetSecretAsync("DataServicesSettings-SapSettings-SapErpUmcApiTokenUrl", false);

		public async Task<string> SapCrmUmcUrmBearerTokenProviderUrlAsync() =>
			await GetSecretAsync("DataServicesSettings-SapSettings-SapCrmUmcUrmApiTokenUrl", false);

		public async Task<string> SapUserManagementBearerTokenProviderUrlAsync()=>
			await GetSecretAsync("DataServicesSettings-SapSettings-SapUserManagementApiTokenUrl", false);

		public async Task<string> SapCrmUmcBearerTokenProviderUrlAsync()=>
			await GetSecretAsync("DataServicesSettings-SapSettings-SapCrmUmcApiTokenUrl", false);
		public string EvenLogApiUrlPrefix =>GetConfigurationValue("DataServicesSettings:EventsPublisherSettings:BaseUrl");

		public TimeSpan CheckForPendingEventsInterval { get; } = TimeSpan.FromSeconds(5);
		public async Task<string> PaymentGatewayAccountAsync() => await GetSecretAsync("PaymentGatewaySettings-Account");
		public async Task<string> PaymentGatewayMerchantIdAsync() => await GetSecretAsync("PaymentGatewaySettings-MerchantId");
		public bool PaymentGatewayAutoSettle =>GetConfigurationValue<bool>("PaymentGatewaySettings:AutoSettle");
		public string PaymentGatewayRequestUrl =>GetConfigurationValue("PaymentGatewaySettings:RequestUrl");
		public async Task<string> PaymentGatewaySecretAsync() => await GetSecretAsync("PaymentGatewaySettings-Secret");

		public async Task<string> EventsBearerTokenProviderUrlAsync() => await GetSecretAsync("DataServicesSettings-EventsPublisherSettings-CmdmApiTokenUrl", false);

		public string PaymentResultsProviderUrl =>
			GetConfigurationValue("DataServicesSettings:PaymentResultsProviderSettings:BaseUrl");

		public string PdfOverlayImagesBaseUrl => GetConfigurationValue("PdfGeneration:ImagesBaseUrl");

		public bool IsInternalDeployment { get; set; }

		public bool IsCacheEnabled =>GetConfigurationValue<bool>("Platform:Caching:IsDomainCacheEnabled");

		public bool IsCachePreLoaderEnabled =>
			IsCacheEnabled &&GetConfigurationValue<bool>("Platform:Caching:IsCachePreLoaderEnabled");

		public CacheProviderType CacheProviderType =>
			GetConfigurationValue("Platform:Caching:CacheType").ToEnum<CacheProviderType>();

		public TimeSpan ExpireCacheItemsWhenNotUsedFor =>
			GetConfigurationValue<TimeSpan>("Platform:Caching:ExpireCacheItemsWhenNotUsedFor");

		public bool HealthChecksEnabled =>GetConfigurationValue<bool>("Platform:Health:ChecksEnabled");

		public bool ProfileInDetail => GetConfigurationValue<bool>("Platform:Profiler:ProfileInDetail");

		public bool ShowDevelopmentTools =>  GetConfigurationValue<bool>("Platform:DevelopmentTools:Enabled");

		public bool RequireCookiesPolicyCompliance => 
			GetConfigurationValue<bool>("UiSettings:UiFeatures:RequireCookiesPolicyCompliance");

		public string TagManagerUrl =>
			GetConfigurationValue("UiSettings:UiFeatures:GoogleSettings:TagManagerUrl");
		public virtual bool IsTagManagerEnabled =>
			GetConfigurationValue<bool>("UiSettings:UiFeatures:GoogleSettings:IsGoogleTagManagerEnabled");

		public IEnumerable<string> ReservedIban =>
			_configuration.GetSection("ReservedIban:ElectricIrelandIban").Value.Split(",");

		public string ResidentialPortalDataSourceBaseUrl =>
			GetConfigurationValue("DataServicesSettings:ResidentialPortalDataSourceSettings:BaseUrl");

		public async Task<string> ResidentialPortalDataSourceBearerTokenProviderUrlAsync ()=> await EventsBearerTokenProviderUrlAsync();

		public string SapBaseUrl => GetConfigurationValue("DataServicesSettings:SapSettings:BaseUrl");

		public async Task<string> SwitchApiBearerTokenProviderUrlAsync()=>await GetSecretAsync("DataServicesSettings-SwitchApiSettings-SwitchApiTokenUrl", false);

		public string ErpUtilitiesUmcEndpoint =>
			GetConfigurationValue("DataServicesSettings:SapSettings:ErpUtilitiesUmcEndpoint");

		public string UserManagementEndpoint =>
			GetConfigurationValue("DataServicesSettings:SapSettings:UserManagementEndpoint");

		public string CrmUtilitiesUmcUrmEndPoint =>
			GetConfigurationValue("DataServicesSettings:SapSettings:CrmUtilitiesUmcUrmEndPoint");

		public string CrmUtilitiesUmcEndPoint =>
			GetConfigurationValue("DataServicesSettings:SapSettings:CrmUtilitiesUmcEndPoint");

		public double BatchEnlistTimeoutMilliseconds =>
			GetConfigurationValue<double>("DataServicesSettings:SapSettings:BatchEnlistTimeoutMilliseconds");


		public TimeSpan RequestTimeout =>
			TimeSpan.FromSeconds(
				GetConfigurationValue<double>("DataServicesSettings:SapSettings:RequestTimeoutSeconds"));

	

		public string StreamServeUrl => GetConfigurationValue("DataServicesSettings:StreamServe:BaseUrl");

		public string SwitchApiEndPoint => GetConfigurationValue("DataServicesSettings:SwitchApiSettings:BaseUrl");
		
		public bool IsPromotionsEnabled =>GetConfigurationValue<bool>("UiSettings:UiFeatures:Promotions:Enabled");

		public bool IsCompetitionEnabled =>GetConfigurationValue<bool>("UiSettings:UiFeatures:Competitions:Enabled");
		public string CompetitionName => GetConfigurationValue("UiSettings:UiFeatures:Competitions:Name");
		public string CompetitionHeading => GetConfigurationValue("UiSettings:UiFeatures:Competitions:Heading");
		public string CompetitionDescription => GetConfigurationValue("UiSettings:UiFeatures:Competitions:Description");
		public string CompetitionDescription1 => GetConfigurationValue("UiSettings:UiFeatures:Competitions:Description1");
		public string CompetitionDescription2 => GetConfigurationValue("UiSettings:UiFeatures:Competitions:Description2");
		public string CompetitionDescription3 => GetConfigurationValue("UiSettings:UiFeatures:Competitions:Description3");
		public string CompetitionQuestion => GetConfigurationValue("UiSettings:UiFeatures:Competitions:Question");
		public string CompetitionAnswerA => GetConfigurationValue("UiSettings:UiFeatures:Competitions:AnswerA");
		public string CompetitionAnswerB => GetConfigurationValue("UiSettings:UiFeatures:Competitions:AnswerB");
		public string CompetitionAnswerC => GetConfigurationValue("UiSettings:UiFeatures:Competitions:AnswerC");
		public string CompetitionTermAndConditionsUrl =>
			GetConfigurationValue("UiSettings:UiFeatures:Competitions:TermAndConditionsUrl");
		public string CompetitionAlreadyEnteredMessage => GetConfigurationValue("UiSettings:UiFeatures:Competitions:AlreadyEnteredMessage");
		public string CompetitionImageDesktop => 
			GetConfigurationValue("UiSettings:UiFeatures:Competitions:Images:Desktop");
		public string CompetitionImageMobile => 
			GetConfigurationValue("UiSettings:UiFeatures:Competitions:Images:Mobile");
		public string CompetitionImageHeader => 
			GetConfigurationValue("UiSettings:UiFeatures:Competitions:Images:Header");

		public string PromotionHeading => GetConfigurationValue("UiSettings:UiFeatures:Promotions:Heading");
		public string PromotionDescription1 => GetConfigurationValue("UiSettings:UiFeatures:Promotions:Description1");
		public string PromotionDescription2 => GetConfigurationValue("UiSettings:UiFeatures:Promotions:Description2");
		public string PromotionLinkText => GetConfigurationValue("UiSettings:UiFeatures:Promotions:LinkText");
		public string PromotionLinkURL => GetConfigurationValue("UiSettings:UiFeatures:Promotions:LinkURL");
		public string PromotionPageTitle => GetConfigurationValue("UiSettings:UiFeatures:Promotions:PageTitle");
		public string PromotionDescription3 => GetConfigurationValue("UiSettings:UiFeatures:Promotions:Description3");
		public string PromotionDescription4 => GetConfigurationValue("UiSettings:UiFeatures:Promotions:Description4");
		public string PromotionTermsConditionsLinkText =>
			GetConfigurationValue("UiSettings:UiFeatures:Promotions:TermsConditionsLinkText");
		public string PromotionTermsConditionsLinkURL =>
			GetConfigurationValue("UiSettings:UiFeatures:Promotions:TermsConditionsLinkURL");
		public string PromotionImageDesktop => 
			GetConfigurationValue("UiSettings:UiFeatures:Promotions:Images:Desktop");
		public string PromotionImageMobile => 
			GetConfigurationValue("UiSettings:UiFeatures:Promotions:Images:Mobile");
		public string PromotionImageHeader => 
			GetConfigurationValue("UiSettings:UiFeatures:Promotions:Images:Header");

		public bool IsSmartActivationEnabled => GetConfigurationValue<bool>("UiSettings:UiFeatures:IsSmartActivationEnabled");
		public bool IsFullRequestLoggingEnabled => _configuration.GetValue<bool>("Platform:RequestResponseLogSettings:IsFullRequestLoggingEnabled");
		public bool IsFullResponseLoggingEnabled => _configuration.GetValue<bool>("Platform:RequestResponseLogSettings:IsFullResponseLoggingEnabled");

		public string LogsRoot => GetConfigurationValue("UiSettings:UiFeatures:LogsViewer:Path");
		public bool LogViewerEnabled =>GetConfigurationValue<bool>("UiSettings:UiFeatures:LogsViewer:Enabled");
		public bool FlowDebuggerIsEnabled =>GetConfigurationValue<bool>("UiSettings:UiFeatures:FlowDebuggerIsEnabled");
		public string ShopBaseUrl => GetConfigurationValue("UiSettings:ShopBaseUrl");
		public string StoreBaseUrl => GetConfigurationValue("UiSettings:StoreBaseUrl");
		public string ElectricIrelandBaseUrl => GetConfigurationValue("UiSettings:ElectricIrelandBaseUrl");

		public async Task<string> StreamServeLiveBearerTokenProviderUrlAsync() => await GetSecretAsync("DataServicesSettings-StreamServe-StreamServeCurrentApiTokenUrl", false);

		public bool EncryptUrls =>
			GetConfigurationValue<bool>("Platform:Encryption:EncryptUrls");

	
		

		public string StreamServeLive => GetConfigurationValue("DataServicesSettings:StreamServe:BaseUrl");

		public string StreamServeArchive =>GetConfigurationValue("DataServicesSettings:StreamServe:ArchiveBaseUrl");

		public string StreamServeUserName => GetConfigurationValue("DataServicesSettings:StreamServe:UserName");
		public Task<string> RedisConnectionString()
		{
			throw new NotImplementedException();
		}

		public string StreamServePassword =>  GetConfigurationValue("DataServicesSettings:StreamServe:Password");
		public Task<string> CacheServiceUserNameAsync() => GetSecretAsync("Platform-Caching-AOT-User", false);

		public Task<string> CacheServicePasswordAsync()=> GetSecretAsync("Platform-Caching-AOT-Password", false);

		private async Task<string> GetSecretAsync(string key, bool isPrefixed = true)
		{
			string result=null;
			if (IsAzureEnabled)
			{
				var keyForEnvironment = isPrefixed ? ResolveSecretKeyForEnvironment(key) : key;
				var secretsRepositoryValue = await _secretsRepository.Value;
				result = await secretsRepositoryValue.GetAsync(keyForEnvironment, 3);
			}
			else
			{
				result = GetConfigurationValue(key.Replace('-', ':'));
			}

			
			return result;
		}
		private string ResolveSecretKeyForEnvironment(string keyWithoutDeploymentType)
		{
			var prefix = !IsInternalDeployment ? "PublicPortal" : "InternalPortal";

			return $"{prefix}-{keyWithoutDeploymentType}";
		}
		private static readonly object LockConfigReload=new object();
		private readonly AsyncLazy<ISecretsRepository> _secretsRepository;

		private string GetConfigurationValue(string key)
		{
			return GetConfigurationValue<string>(key);
		}
		private TValue GetConfigurationValue<TValue>(string key)
		{
			var flag = _configuration.GetValue<bool?>("CloudSettings:Azure:Enabled");
			if (flag == null)
			{
				lock (LockConfigReload)
				{
					flag = _configuration.GetValue<bool?>("CloudSettings:Azure:Enabled");
					if (flag == null )
					{
						_configuration.Reload();
					}
				}
			}

			return _configuration.GetValue<TValue>(key);
		}
	}

}