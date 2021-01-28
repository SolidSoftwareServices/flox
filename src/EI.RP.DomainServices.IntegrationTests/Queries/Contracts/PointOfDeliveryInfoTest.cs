using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.Metering;
using EI.RP.DomainServices.IntegrationTests.Infrastructure;
using EI.RP.DomainServices.Queries.Contracts.PointOfDelivery;
using NUnit.Framework;

namespace EI.RP.DomainServices.IntegrationTests.Queries.Contracts
{
    [TestFixture]
    public class PointOfDeliveryInfoTest : DomainTests
    {
        public static IEnumerable QueryDoesNotFailCases()
        {
            yield return new TestCaseData("hop.mark@test.com", "Test1234");
        }

        [Explicit]
        [Test, TestCaseSource(nameof(QueryDoesNotFailCases))]
        public async Task CanPointOfDeliveryQuery(string userName, string password)
        {
            await LoginUser(userName, password);
            var result = (await DomainQueryProvider
                .FetchAsync<PointOfDeliveryQuery, PointOfDeliveryInfo>(
                    new PointOfDeliveryQuery{ Prn = (ElectricityPointReferenceNumber)"12345"})).FirstOrDefault();
            Assert.IsNotNull(result);
        }
      
    }
}
