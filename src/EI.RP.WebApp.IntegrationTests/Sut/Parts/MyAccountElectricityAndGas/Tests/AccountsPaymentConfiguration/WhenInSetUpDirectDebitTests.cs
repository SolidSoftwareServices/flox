using System.Threading.Tasks;
using AngleSharp.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration.DirectDebit;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.Plan;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.AccountsPaymentConfiguration
{
	[TestFixture]
	internal class WhenInSetUpDirectDebitTests : WhenInAccountPaymentsConfigurationTests
	{
		private PlanPage _sut;

		protected override async Task TestScenarioArrangement()
		{
            async Task AUserIsInTheAccountsPage()
            {

                UserConfig = App.ConfigureUser("a@A.com", "test");
                UserConfig.AddElectricityAccount(withPaperBill: HasPaperBill);
                UserConfig.Execute();

                await ((ResidentialPortalApp) await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();

                Sut = (await App.CurrentPageAs<MyAccountElectricityAndGasPage>().ToBillingAndPayments()).CurrentPageAs<ShowPaymentsHistoryPage>();
            }

            await AUserIsInTheAccountsPage();
            _sut = (await Sut.ClickOnElement(Sut.Overview.BillAndPaymentOptionsLink)).CurrentPageAs<PlanPage>();
        }

        [Test]
		public async Task TheViewShowsTheExpectedInformation()
		{
            var actual = (await Sut.ClickOnElement(_sut.EditDirectDebitLink)).CurrentPageAs<InputDirectDebitDetailsPage>();

            Assert.AreEqual("Direct Debit Settings - Electricity", actual.EditDirectDebitHeader.TextContent);
            Assert.AreEqual("Complete Direct Debit Setup", actual.CompleteDirectDebitButton.Text());

        }
        [Test]
        public async Task CanCancelEditDirectDebitStep()
        {
            var actual = (await Sut.ClickOnElement(_sut.EditDirectDebitLink)).CurrentPageAs<InputDirectDebitDetailsPage>();
            Assert.IsTrue(actual.SureBtn.Href.Contains("/Accounts/Init"));
        }
    }
}