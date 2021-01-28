using System.Net.Http.Headers;
using EI.RP.CoreServices.Http.Clients;

namespace EI.RP.DataServices.SAP.Clients.Repositories.CrmUmcUrm
{
	internal class SapRepositoryOfCrmUmcUrm : SapRepository, ISapRepositoryOfCrmUmcUrm
	{
		public SapRepositoryOfCrmUmcUrm(SapRepositoryOfCrmUmcUrmOptions options, IHttpClientBuilder httpClientBuilder) : base(options,httpClientBuilder)
		{
		}
		protected override void ResolveCsrfHeaders(HttpRequestHeaders headers, string csrf)
		{
			const string xRequestedWith = "X-Requested-With";
			headers.Remove(xRequestedWith);
			headers.Add(xRequestedWith, "XMLHttpRequest");
			
			
		}
	
	}
}