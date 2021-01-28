using System.Net.Http;
using EI.RP.CoreServices.Http.Clients;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace EI.RP.CoreServices.Azure.Infrastructure.Http
{
	class AzureHttpClientBuilder: IHttpClientFactory
	{
		private readonly IHttpClientBuilder _httpClientBuilder;


		public AzureHttpClientBuilder(IHttpClientBuilder httpClientBuilder)
		{
			_httpClientBuilder = httpClientBuilder;
		}


		public HttpClient GetHttpClient()
		{
			return _httpClientBuilder.Build();
		}
	}
}
