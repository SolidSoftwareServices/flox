using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace EI.RP.CoreServices.Authx
{
	public interface IBearerTokenProvider
	{
		Task AppendHeaders(HttpRequestHeaders headers, string bearerTokenResource,
			CancellationToken cancellationToken = default(CancellationToken));
	}
	
}