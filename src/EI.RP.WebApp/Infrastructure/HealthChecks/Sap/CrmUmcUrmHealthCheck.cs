using EI.RP.DataServices;

namespace EI.RP.WebApp.Infrastructure.HealthChecks.Sap
{
	class CrmUmcUrmHealthCheck : SapRepositoryHealthCheck
	{
		public CrmUmcUrmHealthCheck(ISapRepositoryOfCrmUmcUrm repository) : base(repository)
		{

		}
	}
}