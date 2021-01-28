using EI.RP.DataServices;

namespace EI.RP.WebApp.Infrastructure.HealthChecks.Sap
{
	class ErpUmcHealthCheck : SapRepositoryHealthCheck
	{
		public ErpUmcHealthCheck(ISapRepositoryOfErpUmc repository) : base(repository)
		{

		}
	}
}