using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.ReadOnlyCollections;
using EI.RP.DomainServices.IntegrationTests.Infrastructure;
using EI.RP.DomainServices.Queries.Geography.Country;
using NUnit.Framework;

namespace EI.RP.DomainServices.IntegrationTests.Queries.Geography
{
    [Ignore("TODO:TEST")]
    [TestFixture]
    public class CountryQueryTests : DomainTests
    {
        public static IEnumerable CanGetInstallationQuery()
        {
            yield return new TestCaseData("DFDD8@esb.ie", "Test3333");
        }

        [Test, TestCaseSource(nameof(CanGetInstallationQuery))]
        public async Task CanInstallationQuery(string userName, string password)
        {
            await LoginUser(userName, password);
            var result = (await DomainQueryProvider
                .FetchAsync<CountryDetailsQuery, CountryDetails>(
                    new CountryDetailsQuery { })).ToArray();


            Assert.IsNotNull(result);
        }
      
    }
}
