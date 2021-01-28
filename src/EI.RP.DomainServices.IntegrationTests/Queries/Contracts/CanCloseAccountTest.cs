using System;
using System.Collections;
using System.Threading.Tasks;
using EI.RP.DomainServices.IntegrationTests.Infrastructure;
using EI.RP.DomainServices.Queries.Contracts.CanCloseAccount;
using NUnit.Framework;

namespace EI.RP.DomainServices.IntegrationTests.Queries.Contracts
{
    [Ignore("TODO:TEST")]
    [TestFixture]
    public class CanCloseAccountTest : DomainTests
    {
        public static IEnumerable CanResolve()
        {
            yield return new TestCaseData("MR149_Elec-1Meter@preprod.esb.ie", "Test3333", "901198870", null, true);
            yield return new TestCaseData("MR503_Gas-1Meter@preprod.esb.ie", "Test3333", null, "903799787", false);
        }

        [Test, TestCaseSource(nameof(CanResolve))]
        public async Task CanCloseAccount(string userName, string password, string electricityAccountNumber, string gasAccountNumber, bool canClose)
        {
            await LoginUser(userName, password);
            var result = await DomainQueryProvider.CanCloseAccounts(electricityAccountNumber, gasAccountNumber, DateTime.Now);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.CanClose == canClose);
        }
    }
}
