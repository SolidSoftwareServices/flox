using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using EI.RP.CoreServices.System;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration.DirectDebit;
using NUnit.Framework;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.Plan;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.AccountsPaymentConfiguration
{
	[TestFixture]
    internal class WhenInEditDirectDebitTests : WhenInAccountPaymentsConfigurationTests
    {
	    private PlanPage _sut;
		protected override async Task TestScenarioArrangement()
        {
            await base.TestScenarioArrangement();
            _sut = (await Sut.ClickOnElement(Sut.Overview.BillAndPaymentOptionsLink)).CurrentPageAs<PlanPage>();
        }

        [Test]
        public async Task TheViewShowsTheExpectedInformation()
        {
            var incomingBankAccount = UserConfig.ElectricityAndGasAccountConfigurators.Single().Model.IncomingBankAccount;
            var actual = (await _sut.ClickOnElement(_sut.EditDirectDebitLink)).CurrentPageAs<InputDirectDebitDetailsPage>();

            Assert.AreEqual("Direct Debit Settings - Electricity", actual.EditDirectDebitHeader.TextContent);
            Assert.AreEqual(incomingBankAccount.IBAN.Mask('*', incomingBankAccount.IBAN.Length - 4), actual.Iban.Attributes["value"].Value);
            Assert.AreEqual(incomingBankAccount.NameInBankAccount, actual.CustomerName.Attributes["value"].Value);
            Assert.IsTrue(actual.SaveMoneyReminder.Text().Contains("By setting up Direct Debit you will save 5% on your bills."));
            Assert.IsTrue(actual.PrivacyNotice.TextContent.Contains("Electric Ireland requires the below information to allow you to pay your Bills by Direct Debit"));
        }
        [Test]
        public async Task CanCancelEditDirectDebitStep()
        {
			var actual = (await _sut.ClickOnElement(_sut.EditDirectDebitLink)).CurrentPageAs<InputDirectDebitDetailsPage>();
			(await Sut.ClickOnElement(actual.SureBtn)).CurrentPageAs<AccountSelectionPage>();
        }

        [Test]
        public async Task WhenEmptyShowsError()
        {
            var pageEdit = (await _sut.ClickOnElement(_sut.EditDirectDebitLink)).CurrentPageAs<InputDirectDebitDetailsPage>();
			pageEdit.Iban.Value = string.Empty;
            var errorPage = (await Sut.ClickOnElement(pageEdit.UpdateDetailsButton)).CurrentPageAs<InputDirectDebitDetailsPage>();
            Assert.AreEqual("Please enter a valid IBAN", errorPage.IbanError.TextContent);
        }

		[Test]
		public async Task WhenElectricIrelandIbanUsedShowsError()
		{
			var pageEdit = (await _sut.ClickOnElement(_sut.EditDirectDebitLink)).CurrentPageAs<InputDirectDebitDetailsPage>();
			pageEdit.Iban.Value = "IE36AIBK93208681900087"; 
			var errorPage = (await Sut.ClickOnElement(pageEdit.UpdateDetailsButton)).CurrentPageAs<InputDirectDebitDetailsPage>();
			Assert.AreEqual("Please enter a valid IBAN", errorPage.IbanError.TextContent);
		}
	}
}