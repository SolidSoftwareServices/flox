using EI.RP.WebApp.Infrastructure.Extensions;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountEnergyServices.Tests.AccountOverview
{
    [TestFixture]
	class WhenInEnergyServicesAccountOverviewPage_LatestBillTests : WhenInAccountOverviewEnergyServicesPageTests
	{
        [Test]
        public override async Task CanSeeComponentItems()
        {
            var bill = UserConfig.EnergyServicesAccountConfigurators.First().LatestBill;
            var activity = UserConfig.EnergyServicesAccountConfigurators.First().FinancialActivitiesConfiguration.AccountActivities.FirstOrDefault();

            Assert.AreEqual(bill.Amount.ToString(), Sut.LatestBill.CurrentBalanceAmount.TextContent);
            Assert.AreEqual(bill.DueDate.ToString("dnn MMMM yyyy", true), Sut.LatestBill.PaymentDate.TextContent);

            Assert.IsNotNull(Sut.BillsAndPayments);
            Assert.IsNotNull(Sut.BillsAndPayments.BillsPaymentsTable);

            if (activity != null)
            {
                Assert.AreEqual(activity.OriginalDate.ToString("dd/MM/yyyy"), Sut.BillsAndPayments.DateColumn.TextContent);
                Assert.IsTrue(Sut.BillsAndPayments.DescriptionColumn.TextContent.Contains(activity.Description));

                var amount = "-";
                var receivedAmount = activity.ReceivedAmount.ToString().Replace("€0.00", "-");
                var dueAmount = activity.DueAmount.ToString().Replace("€0.00", "-");

                if (!string.IsNullOrWhiteSpace(receivedAmount) && receivedAmount.Trim() != "-")
                {
                    amount = $"+ {receivedAmount}";
                }
                else if (!string.IsNullOrWhiteSpace(dueAmount) && dueAmount.Trim() != "-")
                {
                    amount = dueAmount;
                }

                Assert.AreEqual(amount, Sut.BillsAndPayments.AmountColumn.TextContent);
            }
        }
    }
}