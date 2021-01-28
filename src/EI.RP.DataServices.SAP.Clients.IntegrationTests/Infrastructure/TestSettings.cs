using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.Azure.Configuration;
using EI.RP.DataServices.SAP.Clients.Config;

namespace EI.RP.DataServices.SAP.Clients.IntegrationTests.Infrastructure
{
	//TODO: MAKE SETTINGS IMPLICITly brought from the app for cohesion purposes
	class TestSettings : ISapSettings, IAzureResourceTokenSettings, IAzureCredentialSettings
	{
		public string SapBaseUrl { get; } = "https://pre-api.esb.ie/";
		public string ErpUtilitiesUmcEndpoint { get; } = "internal/ei/sap/erp/v1.1/";
		public string UserManagementEndpoint { get; } = "internal/ei/sap/usermetadata/v1.1/";
		public string CrmUtilitiesUmcUrmEndPoint { get; } = "internal/ei/sap/crm_urm/v1.1/";
		public string CrmUtilitiesUmcEndPoint { get; } = "internal/ei/sap/crm_utilities/v1.1/";
		public double BatchEnlistTimeoutMilliseconds { get; } = 100D;
		public TimeSpan RequestTimeout { get; } = TimeSpan.FromSeconds(100D);

		public async Task<string> SapErpUmcBearerTokenProviderUrlAsync() => "https://ElectricitySupplyBoard.onmicrosoft.com/a40ca8f2-a3ea-4a2e-9ea5-06b40cf532b1";

		public async Task<string> SapCrmUmcUrmBearerTokenProviderUrlAsync() => "https://ElectricitySupplyBoard.onmicrosoft.com/802bf65a-4774-4c5d-bcc8-13ff7ea0bcf8";

		public async Task<string> SapUserManagementBearerTokenProviderUrlAsync() => "https://ElectricitySupplyBoard.onmicrosoft.com/f15a5d62-5327-4bc7-a0f5-98e08dbfc60c";

		public async Task<string> SapCrmUmcBearerTokenProviderUrlAsync() => "https://ElectricitySupplyBoard.onmicrosoft.com/a6c3885e-7ee1-46de-8d33-c0f25ebc6675";

		public TimeSpan BearerTokenCacheDuration => TimeSpan.FromSeconds(180);
		public Task<string> ApiMSubscriptionKeyAsync()
		{
			return Task.FromResult("973df8880e3b4f4dbff9618195ef8052");
		}

		public bool IsAzureEnabled => true;

		public CredentialType CredentialType => CredentialType.ServicePrincipal;

		public string Tenant => "fb01cb1d-bba8-4c1a-94ef-defd79c59a09";

		public string ClientId => "fffaa3d9-f300-4ba4-83cf-ce2b896dbfa9";

		public string ClientSecret => "KimPQ_-H0yOAcwVx_P-R2hS9Wnn4xrG6-Y";
	}
}