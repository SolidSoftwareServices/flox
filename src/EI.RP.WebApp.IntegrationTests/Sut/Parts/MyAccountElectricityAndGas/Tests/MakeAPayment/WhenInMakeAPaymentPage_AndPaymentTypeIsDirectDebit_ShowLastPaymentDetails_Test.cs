using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.Infrastructure.Extensions;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MakeAPayment;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.MakeAPayment
{
    [TestFixture]
    class WhenInMakeAPaymentPage_AndPaymentTypeIsDirectDebit_ShowLastPaymentDetails_Test : WhenInMakeAPaymentPageTests<MakeAPaymentPage>
    {
        public override PaymentMethodType PaymentType { get; } = PaymentMethodType.DirectDebit;
        [Test]
        public async Task CanSeeComponentItems()
        {
            var accountBillingActivities = UserConfig.ElectricityAndGasAccountConfigurators.Single().FinancialActivitiesConfiguration.AccountActivities;
            var accountBillingActivity = accountBillingActivities.First();
            var billingInfo = UserConfig.ElectricityAndGasAccountConfigurators.Single().BillingInfo;
            Assert.AreEqual(billingInfo.CurrentBalanceAmount, (EuroMoney)Sut.ShowPaymentDetails.AmountDue);
            Assert.IsTrue(Sut.ShowPaymentDetails.BillIssueDate.Contains(accountBillingActivity.OriginalDate.ToString("dnn MMMM yyyy", useExtendedSpecifiers: true)));
            Assert.IsTrue(Sut.ShowPaymentDetails.PaymentDueDate.Contains(accountBillingActivity.DueDate.ToString("dnn MMMM yyyy", useExtendedSpecifiers: true)));
            Assert.IsTrue(Sut.ShowPaymentDetails.NextBillDate.Contains(billingInfo.NextBillDate.ToString("dnn MMMM yyyy", useExtendedSpecifiers: true)));
            Assert.AreEqual("Pay a different amount", Sut.ShowPaymentDetails.ShowPayDifferentAmountButton);
			Assert.IsNull(Sut.HPPPostResponse, "hpp post response field breaks the 'message' field of the response from elavon");
        }
    }
}