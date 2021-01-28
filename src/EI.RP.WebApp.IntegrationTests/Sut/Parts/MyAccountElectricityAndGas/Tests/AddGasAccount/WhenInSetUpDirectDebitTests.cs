using System.Linq;
using AngleSharp.Dom;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AddGasAccount;
using NUnit.Framework;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Commands.Banking.DirectDebit.SetUpDirectDebit;
using EI.RP.DomainServices.Commands.Contracts.AddAdditionalAccount.Gas;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration.DirectDebit;
using EI.RP.CoreServices.ErrorHandling;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.AddGasAccount
{
	[TestFixture]
    internal class WhenInSetUpDirectDebitTests : WhenInCollectAccountConsumptionDetailsTest
    {
		[Test]
        public async Task WhenConfigureNewDirectDebitSetUpForGasAccount()
        {
            Sut.InputFormValues(UserConfig, 123435);
            var sut = (await Sut.ClickOnElement(Sut.SubmitButton)).CurrentPageAs<ConfirmAddressPage>();
             await sut.ClickOnElement(sut.ConfirmAddressButton);
            var sutOptionsPage = App.CurrentPageAs<ChoosePaymentOptionsPage>();
            Assert.IsNotNull(sutOptionsPage);
            Assert.IsNotNull(sutOptionsPage.SetUpNewDirectDebitButton);
            Assert.AreEqual("Set up new Direct Debit", sutOptionsPage.SetUpNewDirectDebitButton.TextContent);

            var accountInfo = UserConfig.ElectricityAndGasAccountConfigurators.Single().Model;


            var sutSetUpDirectDebit = (await sutOptionsPage.ClickOnElement(sutOptionsPage.SetUpNewDirectDebitButton))
                .CurrentPageAs<ConfigureDirectDebitPage>();


            Assert.IsNotNull(sutSetUpDirectDebit);
            Assert.AreEqual("Direct Debit Settings - Gas", sutSetUpDirectDebit.Heading.TextContent);
            Assert.AreEqual("Complete Direct Debit Setup", sutSetUpDirectDebit.CompleteDirectDebitButton().Text());

            sutSetUpDirectDebit.InputFormValues("IE65AIBK93104715784037", "Account Name");
            var setUpDirectDebitDomainCommand = new SetUpDirectDebitDomainCommand(accountInfo.AccountNumber,
                sutSetUpDirectDebit.CustomerName.Value, null, sutSetUpDirectDebit.Iban.Value, accountInfo.Partner, accountInfo.ClientAccountType, PaymentMethodType.Manual);


            var sutConfirmation =
                (await sutSetUpDirectDebit.ClickOnElement(sutSetUpDirectDebit.CompleteDirectDebitButton()))
                .CurrentPageAs<AddGasDirectDebitPageConfirmation>();

            
			Assert.IsNotNull(sutConfirmation);
            Assert.AreEqual("Gas Account Set Up & Savings Confirmation", sutConfirmation.Heading.TextContent);

            App.DomainFacade.CommandDispatcher.AssertCommandWasExecuted<AddGasAccountCommand>();
            App.DomainFacade.CommandDispatcher.AssertCommandWasNotExecuted<SetUpDirectDebitDomainCommand>();
		}

		[Test]
		public async Task WhenInNewDirectDebitSetUpWithElectricIrelandIban()
		{
			Sut.InputFormValues(UserConfig, 123435);
			var sut = (await Sut.ClickOnElement(Sut.SubmitButton)).CurrentPageAs<ConfirmAddressPage>();
			await sut.ClickOnElement(sut.ConfirmAddressButton);
			var sutOptionsPage = App.CurrentPageAs<ChoosePaymentOptionsPage>();
			var accountInfo = UserConfig.ElectricityAndGasAccountConfigurators.Single().Model;
			var sutSetUpDirectDebit = (await sutOptionsPage.ClickOnElement(sutOptionsPage.SetUpNewDirectDebitButton))
				.CurrentPageAs<ConfigureDirectDebitPage>();
			sutSetUpDirectDebit.InputFormValues("IE15AIBK93208681900756", "Account Name");

			var setUpDirectDebitDomainCommand = new SetUpDirectDebitDomainCommand(accountInfo.AccountNumber,
				sutSetUpDirectDebit.CustomerName.Value, null, sutSetUpDirectDebit.Iban.Value, accountInfo.Partner, accountInfo.ClientAccountType, PaymentMethodType.Manual);

			await sutSetUpDirectDebit.ClickOnElement(sutSetUpDirectDebit.CompleteDirectDebitButton());
			App.DomainFacade.CommandDispatcher.ExpectCommandAndThrow(setUpDirectDebitDomainCommand, new DomainException(DomainError.GeneralValidation));
		}
	}
}
