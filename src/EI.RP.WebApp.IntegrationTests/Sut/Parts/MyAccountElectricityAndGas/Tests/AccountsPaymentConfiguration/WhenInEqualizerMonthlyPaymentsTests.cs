using Ei.Rp.DomainModels.MappingValues;
using EI.RP.CoreServices.System;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration.EqualizerMonthlyPayments;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.Plan;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.AccountsPaymentConfiguration
{
	[TestFixture]
	internal class WhenInEqualizerMonthlyPaymentsTests : MyAccountCommonTests<EqualizerMonthlyPaymentsPage>
    {
        private static readonly DateTime InitialDate = DateTime.MinValue;
        private static readonly DateTime DateTimeBefore24Th = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 23);
        private static readonly DateTime DateTimeAfter24Th = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 25);
        private static readonly DateTime DateTimeToday = DateTime.Today.AddDays(10);

        protected override async Task TestScenarioArrangement()
		{
            UserConfig = App.ConfigureUser("a@A.com", "test");
            UserConfig.AddElectricityAccount(
                paymentType: PaymentMethodType.Manual, withEqualizerSetupDates: new[] { DateTimeBefore24Th, DateTimeAfter24Th, DateTimeToday, InitialDate })
                .WithInvoices(3, DateTime.Today.AddMonths(-12), DateTime.Today);

            UserConfig.Execute();

            await ((ResidentialPortalApp) await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();

            var page = await App.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMeterReading();
            var billingAndPaymentsOverviewPage = (await page.CurrentPageAs<MyAccountElectricityAndGasPage>().ToPlanPage()).CurrentPageAs<PlanPage>();

            await billingAndPaymentsOverviewPage.ClickOnElement(billingAndPaymentsOverviewPage.EqualiserLink);
            Sut = App.CurrentPageAs<EqualizerMonthlyPaymentsPage>();
        }

        [Test]
        public async Task TheViewShowsTheExpectedInformation()
        {
            var equalizerPaymentDetails = UserConfig.ElectricityAndGasAccountConfigurators.Single().EqualizerPaymentSetupData[DateTimeToday];
            var invoices = UserConfig.ElectricityAndGasAccountConfigurators.Single().FinancialActivitiesConfiguration.AccountActivities
                .OrderBy(x => x.Amount.Amount).ToArray();
            var lowestInvoice = invoices.FirstOrDefault();
            var highestInvoice = invoices.LastOrDefault();

            Assert.IsTrue(Sut.EqualMonthlyPaymentAmount.TextContent.Contains(equalizerPaymentDetails.Amount.ToString()));
            Assert.IsTrue(Sut.HighestBill.TextContent.Contains(highestInvoice?.Amount.ToString()));
            Assert.IsTrue(Sut.HighestInvoiceDate.TextContent.Contains(highestInvoice?.OriginalDate.ToDisplayDate(false)));
            Assert.IsTrue(Sut.LowestBill.TextContent.Contains(lowestInvoice?.Amount.ToString()));
            Assert.IsTrue(Sut.LowestInvoiceDate.TextContent.Contains(lowestInvoice?.OriginalDate.ToDisplayDate(false)));
        }
    }
}