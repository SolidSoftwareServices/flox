using EI.RP.CoreServices.Http.Clients;

namespace EI.RP.DataServices.SAP.Clients.Repositories.ErpUmc
{
	internal class SapRepositoryOfErpUmc : SapRepository, ISapRepositoryOfErpUmc
	{
		public SapRepositoryOfErpUmc(SapRepositoryOfErpUmcOptions options,IHttpClientBuilder httpClientBuilder) : base(options,httpClientBuilder)
		{
		}

		protected override string GetTemporalName()
		{
			return "ErpUmc";
		}
	}
}


