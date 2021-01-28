using System.Net.Http;
using System.Threading.Tasks;

namespace EI.RP.CoreServices.Http.Clients
{
	public interface IApiResultStatusHandler
	{
		Task EnsureSuccessfulResponse(HttpResponseMessage response, bool isOData=true, bool clearSessionOnAuthenticationError=true);
	}
}