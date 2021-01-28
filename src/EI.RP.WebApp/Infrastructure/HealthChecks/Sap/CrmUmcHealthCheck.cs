using EI.RP.DataServices;

namespace EI.RP.WebApp.Infrastructure.HealthChecks.Sap
{
	class CrmUmcHealthCheck : SapRepositoryHealthCheck
	{
		public CrmUmcHealthCheck(ISapRepositoryOfCrmUmc repository):base(repository)
		{
			
		}
	}
}