using EI.RP.DataServices.SAP.Clients.Repositories;
using NUnit.Framework;

namespace EI.RP.DataServices.SAP.Clients.IntegrationTests.Repositories
{
	//[Explicit]
	class UserManagementTests : RepositoryTests
	{
		protected override SapRepository GetRepository()
		{
			return (SapRepository)UserManagement;
		}
	}
}