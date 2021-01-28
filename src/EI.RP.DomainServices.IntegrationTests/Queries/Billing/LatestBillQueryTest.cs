using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.Billing;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.IntegrationTests.Infrastructure;
using EI.RP.DomainServices.Queries.Billing.LatestBill;
using NUnit.Framework;

namespace EI.RP.DomainServices.IntegrationTests.Queries.Billing
{
    [TestFixture]
    public class LatestBillQueryTest : DomainTests
    {
        public static IEnumerable CanGetLatestBillsCases_ElectricityAndGas()
        {
            yield return new TestCaseData("hop.mark@test.com", "Test1234", "902885821");
        }
       

        [Explicit]
        [Test, TestCaseSource(nameof(CanGetLatestBillsCases_ElectricityAndGas))]
        public async Task CanGetLatestBillQuery_ElectrictyAndGas(string userName, string password, string accountNumber)
        { 
            await LoginUser(userName, password);
            var result = (await DomainQueryProvider.FetchAsync<LatestBillQuery, LatestBillInfo>(
                new LatestBillQuery
                    {AccountNumber = accountNumber})).FirstOrDefault();

            Assert.IsNotNull(result);
            Assert.AreEqual((EuroMoney) 72.16, result.CurrentBalanceAmount);
            Assert.AreEqual(PaymentMethodType.DirectDebit, result.PaymentMethod);
            Assert.AreEqual((EuroMoney)77.90, result.Amount);
            Assert.AreEqual(true, result.AccountIsOpen);
            Assert.IsNull(result.AccountDescription);
        }

        public static IEnumerable CanGetLatestBillsCases_EnergyService()
        {
            yield return new TestCaseData("patoconnor14@pp.ie", "Test5555", "950493211");
        }

        [Explicit]
        [Test, TestCaseSource(nameof(CanGetLatestBillsCases_EnergyService))]
        public async Task CanGetLatestBillQuery_EnergyService(string userName, string password, string accountNumber)
        {
            await LoginUser(userName, password);
            var result = (await DomainQueryProvider.FetchAsync<LatestBillQuery, LatestBillInfo>(
                new LatestBillQuery
                    { AccountNumber = accountNumber })).FirstOrDefault();

            Assert.IsNotNull(result);

            Assert.AreEqual((EuroMoney)10.98, result.Amount);
            Assert.AreEqual(PaymentMethodType.DirectDebit, result.PaymentMethod);
            Assert.IsTrue(result.AccountDescription.Contains("Smarter Home"));
        }
    }

}
