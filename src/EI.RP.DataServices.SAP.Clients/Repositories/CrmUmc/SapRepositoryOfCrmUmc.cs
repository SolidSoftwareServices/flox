using EI.RP.CoreServices.Http.Clients;

namespace EI.RP.DataServices.SAP.Clients.Repositories.CrmUmc
{
	internal class SapRepositoryOfCrmUmc : SapRepository, ISapRepositoryOfCrmUmc
	{
		public SapRepositoryOfCrmUmc(SapRepositoryOfCrmUmcOptions options,IHttpClientBuilder httpClientBuilder) : base(options,httpClientBuilder)
		{
		}

		protected override string GetTemporalName()
		{
			return "CrmUmc";
		}
	}
}