using EI.RP.DataServices;

namespace EI.RP.WebApp.Infrastructure.HealthChecks.Sap
{
	class UserManagementHealthCheck : SapRepositoryHealthCheck
	{
		public UserManagementHealthCheck(ISapRepositoryOfUserManagement repository) : base(repository)
		{

		}
	}
}