using System;
using System.Threading.Tasks;

namespace EI.RP.DataServices.SAP.Clients.Config
{
	public interface ISapSettings
	{
		string SapBaseUrl { get; }

		string ErpUtilitiesUmcEndpoint { get; }
		string UserManagementEndpoint { get; }
		string CrmUtilitiesUmcUrmEndPoint { get; }
		string CrmUtilitiesUmcEndPoint { get; }

		double BatchEnlistTimeoutMilliseconds { get; }
		TimeSpan RequestTimeout { get; }
		Task<string> SapErpUmcBearerTokenProviderUrlAsync();
		Task<string> SapCrmUmcUrmBearerTokenProviderUrlAsync ();
		Task<string> SapUserManagementBearerTokenProviderUrlAsync();
		Task<string> SapCrmUmcBearerTokenProviderUrlAsync();
	}
}