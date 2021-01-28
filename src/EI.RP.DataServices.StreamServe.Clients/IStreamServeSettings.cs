
using System.Threading.Tasks;

namespace EI.RP.DataServices.StreamServe.Clients
{
	public interface IStreamServeSettings
	{
		string StreamServeUrl { get; }
		Task<string> StreamServeLiveBearerTokenProviderUrlAsync();
	
	}
}