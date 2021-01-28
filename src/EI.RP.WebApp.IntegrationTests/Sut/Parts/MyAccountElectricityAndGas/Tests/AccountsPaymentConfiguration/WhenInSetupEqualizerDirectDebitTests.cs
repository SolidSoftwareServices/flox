using System;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Commands.Banking.DirectDebit.SetUpDirectDebit;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration.DirectDebit;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration.EqualizerMonthlyPayments;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.Plan;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.AccountsPaymentConfiguration
{
	[TestFixture]
	internal class WhenInSetupEqualizerDirectDebitTests : MyAccountCommonTests<SetupEqualizerDirectDebitPage>
    {
        private static readonly DateTime DateTimeBefore24Th = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 23);
        private static readonly DateTime DateTimeAfter24Th = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 25);
        private static readonly DateTime DateTimeToday = DateTime.Today.AddDays(10).FirstDayOfNextMonth(28);

		protected override async Task TestScenarioArrangement()
        {

            UserConfig = App.ConfigureUser("a@A.com", "test");
            UserConfig.AddElectricityAccount(
                paymentType: PaymentMethodType.Manual, withEqualizerSetupDates: new[] { DateTimeBefore24Th, DateTimeAfter24Th, DateTimeToday });
            UserConfig.Execute();

            await ((ResidentialPortalApp) await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();

            var billingAndPaymentsOverviewPage = (await App.CurrentPageAs<MyAccountElectricityAndGasPage>().ToBillingAndPayments()).CurrentPageAs<ShowPaymentsHistoryPage>();
            var planPage = (await billingAndPaymentsOverviewPage.ClickOnElement(billingAndPaymentsOverviewPage.Overview.BillAndPaymentOptionsLink)).CurrentPageAs<PlanPage>();

            var equalizerMonthlyPaymentsPage = (await planPage.ClickOnElement(planPage.EqualiserLink)).CurrentPageAs<EqualizerMonthlyPaymentsPage>();
            var setupEqualizerMonthlyPaymentsPage = (await equalizerMonthlyPaymentsPage.ClickOnElement(equalizerMonthlyPaymentsPage.SetupEqualMonthlyPaymentsBtn)).CurrentPageAs<SetupEqualizerMonthlyPaymentsPage>();
            Sut = (await setupEqualizerMonthlyPaymentsPage.ClickOnElement(setupEqualizerMonthlyPaymentsPage.SetDirectDebitButton)).CurrentPageAs<SetupEqualizerDirectDebitPage>();
        }

        [Test]
        public async Task CanNavigateToConfirmationPage()
        {
            var accountInfo = UserConfig.ElectricityAndGasAccountConfigurators.Single().Model;
            var incomingBankAccount = UserConfig.ElectricityAndGasAccountConfigurators.Single().Model.IncomingBankAccount;
            Sut = SetRequiredValue(Sut);

            var setUpDirectDebitDomainCommand = new SetUpDirectDebitDomainCommand(Sut.AccountNumber.TextContent,
                Sut.CustomerName.Value, incomingBankAccount.IBAN, Sut.Iban.Value, accountInfo.Partner, ClientAccountType.Electricity, PaymentMethodType.Equalizer);
            App.DomainFacade.CommandDispatcher.ExpectCommandAndSuccess(setUpDirectDebitDomainCommand);
            var directDebitPageConfirmationPage = (await Sut.ClickOnElement(Sut.CompleteDirectDebitButton))
                .CurrentPageAs<EqualizerDirectDebitPageConfirmation>();
            App.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(setUpDirectDebitDomainCommand);
            (await directDebitPageConfirmationPage.ClickOnElement(directDebitPageConfirmationPage.BackToAccountsLink)).CurrentPageAs<AccountSelectionPage>();

        }

		[Test]
		public async Task NavigateToConfirmationPageFails_WhenElectricIrelandIbanUsed()
		{
			var accountInfo = UserConfig.ElectricityAndGasAccountConfigurators.Single().Model;
			var incomingBankAccount = UserConfig.ElectricityAndGasAccountConfigurators.Single().Model.IncomingBankAccount;
			Sut = SetRequiredValue(Sut);
			Sut.Iban.Value = "IE15AIBK93208681900756"; 

			var setUpDirectDebitDomainCommand = new SetUpDirectDebitDomainCommand(Sut.AccountNumber.TextContent,
				Sut.CustomerName.Value, incomingBankAccount.IBAN, Sut.Iban.Value, accountInfo.Partner, ClientAccountType.Electricity, PaymentMethodType.Equalizer);
			App.DomainFacade.CommandDispatcher.ExpectCommandAndThrow(setUpDirectDebitDomainCommand, new DomainException(DomainError.GeneralValidation));
			await Sut.ClickOnElement(Sut.CompleteDirectDebitButton);
			App.DomainFacade.CommandDispatcher.AssertCommandWasNotExecuted(setUpDirectDebitDomainCommand);
		}

		[Test]
        public async Task CanNavigateBackToPlanPage()
        {
            Sut = SetRequiredValue(Sut);

            var directDebitPageConfirmationPage = (await Sut.ClickOnElement(Sut.CompleteDirectDebitButton))
                .CurrentPageAs<EqualizerDirectDebitPageConfirmation>();

            (await directDebitPageConfirmationPage.ClickOnElement(directDebitPageConfirmationPage.BackToEqualizerDirectDebitEditLink)).CurrentPageAs<PlanPage>();
        }
        [Test]
        public async Task CanNavigateBackToAccounts()
        {
            Sut = SetRequiredValue(Sut);

            var directDebitPageConfirmationPage = (await Sut.ClickOnElement(Sut.CompleteDirectDebitButton))
                .CurrentPageAs<EqualizerDirectDebitPageConfirmation>();
            (await Sut.ClickOnElement(directDebitPageConfirmationPage.BackToAccountsLink)).CurrentPageAs<AccountSelectionPage>();

        }
        private SetupEqualizerDirectDebitPage SetRequiredValue(SetupEqualizerDirectDebitPage currentPage)
        {
            currentPage.Iban.Value = "IE65AIBK93104715784037";
            currentPage.CustomerName.Value = "Account Name";
            currentPage.CheckBox.IsChecked = true;
            return currentPage;
        }
    }
}