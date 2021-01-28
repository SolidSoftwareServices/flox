using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Commands.Banking.DirectDebit.SetUpDirectDebit;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration.DirectDebit;
using NUnit.Framework;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.Plan;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.AccountsPaymentConfiguration
{
	[TestFixture]
    internal class WhenInConfirmationTests : WhenInAccountPaymentsConfigurationTests
    {
	    private PlanPage _sut;
		protected override async Task TestScenarioArrangement()
        {
            await base.TestScenarioArrangement();
            _sut = (await Sut.ClickOnElement(Sut.Overview.BillAndPaymentOptionsLink)).CurrentPageAs<PlanPage>();
        }

        [Test]
        public async Task CanNavigateToConfirmationPage()
        {
            var pageEdit = (await _sut.ClickOnElement(_sut.EditDirectDebitLink)).CurrentPageAs<InputDirectDebitDetailsPage>();
            var incomingBankAccount = UserConfig.ElectricityAndGasAccountConfigurators.Single().Model.IncomingBankAccount;
            pageEdit = SetRequiredValue(pageEdit);
            var accountInfo = UserConfig.ElectricityAndGasAccountConfigurators.Single().Model;
            var setUpDirectDebitDomainCommand = new SetUpDirectDebitDomainCommand(
                pageEdit.AccountNumber.TextContent,
                pageEdit.CustomerName.Value,
                incomingBankAccount.IBAN,
                pageEdit.Iban.Value, 
                accountInfo.Partner,
                ClientAccountType.Electricity, 
                PaymentMethodType.DirectDebit);

            App.DomainFacade.CommandDispatcher.ExpectCommandAndSuccess(setUpDirectDebitDomainCommand);

            var directDebitPageConfirmationPage = (await pageEdit.ClickOnElement(pageEdit.CompleteDirectDebitButton))
                .CurrentPageAs<DirectDebitPageConfirmation>();

            App.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(setUpDirectDebitDomainCommand);
            Assert.AreEqual("Thank you, your direct debit setup has been set up.", directDebitPageConfirmationPage.SectionHeader.TextContent);
        }

		[Test]
		public async Task NavigateToConfirmationPageFails_WhenElectricIrelandIbanUsed()
		{
			var pageEdit = (await _sut.ClickOnElement(_sut.EditDirectDebitLink)).CurrentPageAs<InputDirectDebitDetailsPage>();
			var incomingBankAccount = UserConfig.ElectricityAndGasAccountConfigurators.Single().Model.IncomingBankAccount;
			pageEdit = SetRequiredValue(pageEdit);
			pageEdit.Iban.Value = "IE36AIBK93208681900087";
			var accountInfo = UserConfig.ElectricityAndGasAccountConfigurators.Single().Model;
			var setUpDirectDebitDomainCommand = new SetUpDirectDebitDomainCommand(
				pageEdit.AccountNumber.TextContent,
				pageEdit.CustomerName.Value,
				incomingBankAccount.IBAN,
				pageEdit.Iban.Value,
				accountInfo.Partner,
				ClientAccountType.Electricity,
				PaymentMethodType.DirectDebit);

			App.DomainFacade.CommandDispatcher.ExpectCommandAndThrow(setUpDirectDebitDomainCommand, new DomainException(DomainError.GeneralValidation));
			await pageEdit.ClickOnElement(pageEdit.CompleteDirectDebitButton);
			App.DomainFacade.CommandDispatcher.AssertCommandWasNotExecuted(setUpDirectDebitDomainCommand);
		}

		[Test]
        public async Task CanNavigateBackToPlanPage()
        {
            var pageEdit = (await _sut.ClickOnElement(_sut.EditDirectDebitLink)).CurrentPageAs<InputDirectDebitDetailsPage>();
			pageEdit = SetRequiredValue(pageEdit);
            var directDebitPageConfirmationPage = (await pageEdit.ClickOnElement(pageEdit.CompleteDirectDebitButton))
                .CurrentPageAs<DirectDebitPageConfirmation>();
            (await Sut.ClickOnElement(directDebitPageConfirmationPage.BackToBillAndPaymentOptionsLink)).CurrentPageAs<PlanPage>();
        }
        [Test]
        public async Task CanNavigateBackToAccounts()
        {
            var pageEdit = (await _sut.ClickOnElement(_sut.EditDirectDebitLink)).CurrentPageAs<InputDirectDebitDetailsPage>();
			pageEdit = SetRequiredValue(pageEdit);
            var directDebitPageConfirmationPage = (await pageEdit.ClickOnElement(pageEdit.CompleteDirectDebitButton))
                .CurrentPageAs<DirectDebitPageConfirmation>();
            (await Sut.ClickOnElement(directDebitPageConfirmationPage.BackToAccountsLink)).CurrentPageAs<AccountSelectionPage>();

        }

        private InputDirectDebitDetailsPage SetRequiredValue(InputDirectDebitDetailsPage currentDetailsPage)
        {
            currentDetailsPage.Iban.Value = "IE65AIBK93104715784037";
            currentDetailsPage.CustomerName.Value = "Account Name";
            currentDetailsPage.ConfirmTerms.IsChecked = true;
            return currentDetailsPage;
        }
    }
}
