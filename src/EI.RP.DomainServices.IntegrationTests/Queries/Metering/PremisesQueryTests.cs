using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.Metering;
using EI.RP.DomainServices.IntegrationTests.Infrastructure;
using EI.RP.DomainServices.Queries.Metering.Premises;
using NUnit.Framework;

namespace EI.RP.DomainServices.IntegrationTests.Queries.Metering
{
    [Explicit("TODO:TEST")]
    [TestFixture]
    public class PremisesQueryTests : DomainTests
    {
        public static IEnumerable CanGetQuery()
        {
            yield return new TestCaseData("DFDD8@esb.ie", "Test3333");
        }

        [Test, TestCaseSource(nameof(CanGetQuery))]
        public async Task CanQuery(string userName, string password)
        {
            await LoginUser(userName, password);
            var result = (await DomainQueryProvider
                .FetchAsync<PremisesQuery, Premise>(
                    new PremisesQuery { PremiseId = "0061057405"})).FirstOrDefault();


            Assert.IsNotNull(result);
        }
      
    }
}
