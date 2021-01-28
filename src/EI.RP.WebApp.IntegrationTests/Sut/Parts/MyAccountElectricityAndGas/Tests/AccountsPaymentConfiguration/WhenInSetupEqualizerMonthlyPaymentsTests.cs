using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.Mvc.Core.System;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration.EqualizerMonthlyPayments;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.Plan;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.AccountsPaymentConfiguration
{
	[TestFixture]
	internal class WhenInSetupEqualizerMonthlyPaymentsTests : MyAccountCommonTests<SetupEqualizerMonthlyPaymentsPage>
    {
        private static readonly DateTime DateTimeBefore24Th = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 23);
        private static readonly DateTime DateTimeAfter24Th = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 25);
        private static readonly DateTime DateTimeToday = DateTime.Today.AddDays(10).FirstDayOfNextMonth(28);

        protected override async Task TestScenarioArrangement()
        {
            
            UserConfig = App.ConfigureUser("a@A.com", "test");
            UserConfig.AddElectricityAccount(
                paymentType: PaymentMethodType.Manual, withEqualizerSetupDates: new []{DateTimeBefore24Th, DateTimeAfter24Th, DateTimeToday});
            UserConfig.Execute();

            await ((ResidentialPortalApp) await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();

            var billingAndPaymentsOverviewPage = (await App.CurrentPageAs<MyAccountElectricityAndGasPage>().ToBillingAndPayments()).CurrentPageAs<ShowPaymentsHistoryPage>();
            var planPage = (await billingAndPaymentsOverviewPage.ClickOnElement(billingAndPaymentsOverviewPage.Overview.BillAndPaymentOptionsLink)).CurrentPageAs<PlanPage>();

            var equalizerMonthlyPaymentsPage = (await planPage.ClickOnElement(planPage.EqualiserLink)).CurrentPageAs<EqualizerMonthlyPaymentsPage>();
            Sut = (await equalizerMonthlyPaymentsPage.ClickOnElement(equalizerMonthlyPaymentsPage.SetupEqualMonthlyPaymentsBtn))
                .CurrentPageAs<SetupEqualizerMonthlyPaymentsPage>();
        }

        [Test]
        public async Task TheViewShowsTheExpectedInformation()
        {
            var equalizerDetails =
                UserConfig.ElectricityAndGasAccountConfigurators.Single().EqualizerPaymentSetupData[DateTimeToday];
            var accountInfo = UserConfig.ElectricityAndGasAccountConfigurators.Single().Model;
           
            Assert.AreEqual(equalizerDetails.Amount.ToString(), Sut.EqualMonthlyPaymentAmount.TextContent);
            var firstDueDate = DateTime.Today.AddDays(10).FirstDayOfNextMonth(28);
            
            Assert.AreEqual(firstDueDate.ToShortDateString(), Sut.DatePickerInputElement.Value);
        }

        public static IEnumerable ItRefreshesTheAmountWhenANewDateIsSelected_Cases()
        {
            yield return new TestCaseData(DateTimeBefore24Th).SetName(
                $"ItRefreshesTheAmountWhenANewDateIsSelected - before 24th");
            yield return new TestCaseData(DateTimeAfter24Th).SetName(
                $"ItRefreshesTheAmountWhenANewDateIsSelected - after 24th");
            yield return new TestCaseData(DateTimeToday).SetName(
                $"ItRefreshesTheAmountWhenANewDateIsSelected - Today");
        }

        [Test]
        [TestCaseSource(nameof(ItRefreshesTheAmountWhenANewDateIsSelected_Cases))]
        public async Task ItRefreshesTheAmountWhenANewDateIsSelected(DateTime selectDate)
        {
            
            Sut.DatePickerInputElement.Value = selectDate.ToShortDateString();
            Sut = (await Sut.ExecuteEventOnInputChanged(Sut.DatePickerInputElement)).CurrentPageAs<SetupEqualizerMonthlyPaymentsPage>();
            var equalizerPaymentSetupData = UserConfig.ElectricityAndGasAccountConfigurators.Single().EqualizerPaymentSetupData;
            var expected =
                equalizerPaymentSetupData[selectDate].Amount.ToString();
            var actual = Sut.EqualMonthlyPaymentAmount.TextContent;
            Assert.AreEqual(expected,actual);

        }
    }
}