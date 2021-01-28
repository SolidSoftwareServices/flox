using System.Linq;
using System.Threading.Tasks;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DataModels.Sap.ErpUmc.Functions;
using EI.RP.DataServices.SAP.Clients.Repositories;
using NUnit.Framework;

namespace EI.RP.DataServices.SAP.Clients.IntegrationTests.Repositories
{
	//[Explicit]
	class ErpUmcTests : RepositoryTests
	{
		protected override SapRepository GetRepository()
		{
			return (SapRepository)ErpUmc;
		}


		[Test]
		public async Task ExecuteFunction()
		{
			await DoLogin();
			var accounts = await CrmUmc.NewQuery<AccountDto>().GetMany();

			var actual = await ErpUmc.ExecuteFunctionWithManyResults(new GetPremisesForAccountFunction
				{Query = {AccountID = accounts.First().AccountID}});

			Assert.IsNotNull(actual);
			Assert.IsNotEmpty(actual);

		}
	}
}