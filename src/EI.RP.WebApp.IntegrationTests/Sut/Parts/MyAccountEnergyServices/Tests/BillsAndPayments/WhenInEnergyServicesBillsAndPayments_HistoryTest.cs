using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountEnergyServices.Tests.BillsAndPayments
{
    [TestFixture]
    class WhenInEnergyServicesBillsAndPayments_HistoryTest : WhenInEnergyServiceBillsAndPaymentsPageTest
    {
        [Test]
        [Ignore("Feature removed")]
        public override async Task CanSeeComponentItems()
        {
            var activity = UserConfig.EnergyServicesAccountConfigurators.First().FinancialActivitiesConfiguration.AccountActivities.FirstOrDefault();

            Assert.IsNotNull(Sut.BillsAndPayments.BillsPaymentsTable);

            if (activity != null)
            {
                var amount = activity.ReceivedAmount != null && activity.ReceivedAmount.ToString() != "€0.00"
                    ? $"+ {activity.ReceivedAmount.ToString()}"
                    : activity.DueAmount != null && activity.DueAmount.ToString() != "€0.00"
                        ? activity.DueAmount.ToString()
                        : "-";

                Assert.AreEqual(activity.OriginalDate.ToString("dd/MM/yyyy"), Sut.BillsAndPayments.DateColumnValue.TextContent);
                Assert.AreEqual(activity.Description, Sut.BillsAndPayments.DescriptionColumnValue.TextContent);
                Assert.AreEqual(amount, Sut.BillsAndPayments.AmountColumnValue.TextContent);
            }
        }

    }
}
