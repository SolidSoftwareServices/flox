using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.User;
using EI.RP.DomainServices.IntegrationTests.Infrastructure;
using EI.RP.DomainServices.Queries.User.UserContact;
using NUnit.Framework;

namespace EI.RP.DomainServices.IntegrationTests.Queries.User
{
    [TestFixture]
    public class UserAccountDetailsQueryTest : DomainTests
    {
        public static IEnumerable CanGetUserContactDetails()
        {
            yield return new TestCaseData("hop.mark@test.com", "Test1234", "902885821");
            yield return new TestCaseData("duelfuel@esb.ie", "Test1234", "950442473");
        }

        [Explicit]
        [Test, TestCaseSource(nameof(CanGetUserContactDetails))]
        public async Task CanGetUserContactDetailsQuery_Test(string userName, string password, string accountNumber)
        {
            await LoginUser(userName, password);
            var result = (await DomainQueryProvider.FetchAsync<UserContactDetailsQuery, UserContactDetails>(
                new UserContactDetailsQuery
                    {AccountNumber = accountNumber})).FirstOrDefault();

            Assert.IsNotNull(result);
        }
    }
}
