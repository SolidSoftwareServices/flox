using System.Threading.Tasks;

namespace EI.RP.DataStore
{
	public interface IResidentialPortalDataSourceSettings
	{
		string ResidentialPortalDataSourceBaseUrl { get; }
		Task<string> ResidentialPortalDataSourceBearerTokenProviderUrlAsync();
	}
}