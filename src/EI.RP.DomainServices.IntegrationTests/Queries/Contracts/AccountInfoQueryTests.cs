using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.IntegrationTests.Infrastructure;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using NUnit.Framework;

namespace EI.RP.DomainServices.IntegrationTests.Queries.Contracts
{
	[Explicit("TODO")]
	[TestFixture]
	public class AccountInfoQueryTests:DomainTests
	{
		

		public static IEnumerable QueryDoesNotFailCases()
		{
			//yield return new TestCaseData("hop.mark@test.com", "Test1234").Returns(2);
			yield return new TestCaseData("HHLT2years2@test.ie", "Test3333").Returns(1);
			yield return new TestCaseData("HHLT2years1@test.ie", "Test3333").Returns(1);

		}
		[Test,TestCaseSource(nameof(QueryDoesNotFailCases))]
		public async Task<int> QueryReturnsCorrectNumberOfAccounts(string userName,string password)
		{
			await LoginUser(userName, password);
			var result= await DomainQueryProvider
				.FetchAsync<AccountInfoQuery, AccountInfo>(
					new AccountInfoQuery());

			Assert.IsNotNull(result);
			return result.Count();
		}

		public static IEnumerable CanGetAccountByMprnGprnCases()
		{
			yield return new TestCaseData("hop.mark@test.com", "Test1234", "1169504");
			yield return new TestCaseData("hop.mark@test.com", "Test1234", "10302392445");
		}

		[Test, TestCaseSource(nameof(CanGetAccountByMprnGprnCases))]
		public async Task CanGetAccountByMprnGprn(string userName, string password,string mprnGprn)
		{
			await LoginUser(userName, password);
			var result = await DomainQueryProvider.GetAccountInfoByPrn((ElectricityPointReferenceNumber)mprnGprn);

			Assert.IsNotNull(result);
		}

		[Test]
		public async Task QueryReturnsCorrectEqualiserData()
		{
			await LoginUser("elecequal2@esb.ie", "Test3333");
			var result = await DomainQueryProvider.FetchAsync<AccountInfoQuery, AccountInfo>(new AccountInfoQuery());


			Assert.IsNotNull(result);
		}

		[Test]
		public async Task CanQueryDuelFuelAccounts_WhenElectricityOnly()
		{
			await LoginUser("ElecDD20@esb.ie", "Test3333");
			var result = await DomainQueryProvider.FetchAsync<AccountInfoQuery, AccountInfo>(new AccountInfoQuery
			{
				RetrieveDuelFuelSisterAccounts = true,
				AccountNumber = "900679044"
			});


			Assert.AreEqual(1,result.Count());
			Assert.AreEqual(ClientAccountType.Electricity,result.Single().ClientAccountType);
		}

		[Test]
		public async Task CanQueryDuelFuelAccounts_WhenGasOnly()
		{
			await LoginUser("GasDD20@esb.ie", "Test3333");
			var result = await DomainQueryProvider.FetchAsync<AccountInfoQuery, AccountInfo>(new AccountInfoQuery
			{
				RetrieveDuelFuelSisterAccounts = true,
				AccountNumber = "950418947"
			});


			Assert.AreEqual(1, result.Count());
			Assert.AreEqual(ClientAccountType.Gas, result.Single().ClientAccountType);
		}

		[Test]
		public async Task CanQueryDuelFuelAccounts_FromGas()
		{
			await LoginUser("DFDD20@esb.ie", "Test3333");
			var result = await DomainQueryProvider.FetchAsync<AccountInfoQuery, AccountInfo>(new AccountInfoQuery
			{
				RetrieveDuelFuelSisterAccounts = true,
				AccountNumber = "903768718"
			});


			Assert.AreEqual(2, result.Count());
			Assert.AreEqual(ClientAccountType.Gas, result.First().ClientAccountType);
			Assert.AreEqual(ClientAccountType.Electricity, result.Last().ClientAccountType);
		}

		[Test]
		public async Task CanQueryDuelFuelAccounts_FromElectricity()
		{
			await LoginUser("DFDD20@esb.ie", "Test3333");
			var result = await DomainQueryProvider.FetchAsync<AccountInfoQuery, AccountInfo>(new AccountInfoQuery
			{
				RetrieveDuelFuelSisterAccounts = true,
				AccountNumber = "900778500"
			});


			Assert.AreEqual(2, result.Count());
			Assert.AreEqual(ClientAccountType.Electricity, result.First().ClientAccountType);
			Assert.AreEqual(ClientAccountType.Gas, result.Last().ClientAccountType);
		}
	}
}