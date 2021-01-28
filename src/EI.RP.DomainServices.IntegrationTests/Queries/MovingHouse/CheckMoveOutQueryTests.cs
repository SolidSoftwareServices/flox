using NUnit.Framework;
using System.Collections;
using System.Threading.Tasks;
using EI.RP.DomainServices.IntegrationTests.Infrastructure;
using EI.RP.DomainServices.Queries.MovingHouse.CheckMoveOutRequestResults;

namespace EI.RP.DomainServices.IntegrationTests.Query
{
    [Ignore("TODO:TEST")]
    [TestFixture]
    public class CheckMoveOutQueryTests : DomainTests
    {
        public static IEnumerable CanGetCheckMoveOutQuery()
        {
            yield return new TestCaseData("gequal@esb.ie", "Test3333", "1500922456", true, true);

            yield return new TestCaseData("DFSFEXITFEE@gmail.ie", "Test3333", "1501006241", true, true);
            yield return new TestCaseData("DFSFEXITFEE@gmail.ie", "Test3333", "2012635532", true, false);
        }

        [Test, TestCaseSource(nameof(CanGetCheckMoveOutQuery))]
        public async Task CanCheckMoveOutQuery(string userName, string password, string contactId, 
                                               bool isMoveOutOk, bool hasExitFee)
        {
            await LoginUser(userName, password);
            var result = await DomainQueryProvider.CheckMoveOut(contactId);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsMoveOutOk==isMoveOutOk);
            Assert.IsTrue(result.HasExitFee==hasExitFee);
        }      
    }
}
