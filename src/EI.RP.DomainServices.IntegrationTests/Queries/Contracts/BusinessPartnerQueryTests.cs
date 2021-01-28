using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.Contracts;
using EI.RP.DomainServices.IntegrationTests.Infrastructure;
using EI.RP.DomainServices.Queries.Contracts.BusinessPartners;
using NUnit.Framework;

namespace EI.RP.DomainServices.IntegrationTests.Queries.Contracts
{
	[Ignore("TODO:TEST")]
	[TestFixture]
	public class BusinessPartnerQueryTests : DomainTests
	{
		private bool _wasInternal;

		[OneTimeSetUp]
		public void OnOneTimeSetUp()
		{
			_wasInternal = AssemblySetUp.IsInternalDeployment;
			AssemblySetUp.IsInternalDeployment = true;
		}

		[OneTimeTearDown]
		public void OnOneTimeTearDown()
		{
			AssemblySetUp.IsInternalDeployment = _wasInternal;
		}

		public static IEnumerable QueryDoesNotFailCases()
		{
			yield return new TestCaseData("furlong_d", "Init1234", "7005466453", "SKY ROAD,CLIFDEN");
		}

		[Test, TestCaseSource(nameof(QueryDoesNotFailCases))]
		public async Task QueryReturnsCorrectNumberOfAccounts(string userName, string password, string partnerNum,
			string expectedDescription)
		{
			await LoginUser(userName, password);

			var result = await DomainQueryProvider
				.FetchAsync<BusinessPartnerQuery, BusinessPartner>(
					new BusinessPartnerQuery
					{
						PartnerNum = partnerNum
					}
			);

			Assert.IsNotNull(result);
			Assert.IsNotEmpty(result);
			Assert.AreEqual(1,result.Count());
			var actual = result.Single();
			Assert.AreEqual(partnerNum,actual.NumPartner);
			Assert.AreEqual(expectedDescription, actual.Description);
		}
	}
}