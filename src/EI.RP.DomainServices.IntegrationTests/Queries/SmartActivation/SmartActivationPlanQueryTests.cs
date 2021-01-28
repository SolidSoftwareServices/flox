using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.IntegrationTests.Infrastructure;
using EI.RP.DomainServices.Queries.SmartActivation.SmartActivationPlan;
using NUnit.Framework;

namespace EI.RP.DomainServices.IntegrationTests.Queries.SmartActivation
{
	[Explicit]
	public class SmartActivationPlanQueryTests : DomainTests
	{


		public static IEnumerable QueryReturnsPlanCorrectlyCases()
		{
			yield return new TestCaseData("MPRN10034763240@esb.ie", "Test3333",SmartPlanGroup.Single,"904755258").SetName("Single electricity");
			yield return new TestCaseData("MPRN10036876790@esb.ie", "Test3333",SmartPlanGroup.Dual,"904755259").SetName("Dual fuel");

		}

		[Test, TestCaseSource(nameof(QueryReturnsPlanCorrectlyCases))]
		public async Task QueryReturnsFileCorrectly(string userName, string password, SmartPlanGroup expectedPlanGroup,string accountNumber)
		{
			await LoginUser(userName, password);
			
			var result = (await DomainQueryProvider.GetSmartActivationPlans(accountNumber, onlyActive: true)).ToArray();

			Assert.IsNotNull(result);
			Assert.IsTrue(result.All(x => x.PlanType == expectedPlanGroup));
			Assert.IsTrue(result.All(x => x.IsActive));
		}
	}
}