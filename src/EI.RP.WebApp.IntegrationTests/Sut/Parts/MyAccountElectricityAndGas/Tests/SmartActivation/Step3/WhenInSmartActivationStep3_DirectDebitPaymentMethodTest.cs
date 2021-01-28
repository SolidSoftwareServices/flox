using AutoFixture;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.CoreServices.System;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SmartActivation;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System.Linq;
using System.Threading.Tasks;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.SmartActivation.Step3
{
	[TestFixture]
    class WhenInSmartActivationStep3_DirectDebitPaymentMethodTest : WhenInSmartActivationStep3Test
    {
	    protected override PaymentMethodType PaymentMethod => PaymentMethodType.DirectDebit;

		[Test]
		public override async Task CanSeeComponents()
		{
			Assert.IsNotNull(Sut.Stepper);
			Assert.AreEqual("Choose a payment option", Sut.PaymentHeading?.TextContent);
			Assert.IsNotNull(Sut.ExistingDirectDebitElement?.ExistingDirectDebitOption);
			Assert.IsNull(Sut.AlternativePayerElement?.MainElement);

			var user = UserConfig.Accounts.Single(x => x.IsElectricityAccount());
			Assert.AreEqual(user.IncomingBankAccount.NameInBankAccount,Sut.ExistingDirectDebitElement?.NameOnBank.TextContent);
			Assert.AreEqual(user.IncomingBankAccount.IBAN.Mask('*', user.IncomingBankAccount.IBAN.Length - 4), Sut.ExistingDirectDebitElement?.ExistingIban.TextContent);

			Assert.AreEqual("I confirm that all of the details are correct and that I have the authority to make this submission *", Sut.ExistingDirectDebitElement?.ConfirmUseExistingDebitLabel?.TextContent);
			
			Assert.IsNotNull(Sut.ExistingDirectDebitElement?.ConfirmUseExistingDebitError);
			Assert.IsNotNull(Sut.ExistingDirectDebitElement?.UseExistingDirectDebitButton);

			Assert.IsNotNull(Sut.SetupDirectDebitElementForManualPayment?.SetupNewDirectDebitOption);
			Assert.IsNotNull(Sut.SetupDirectDebitElementForManualPayment?.SetupNewDirectDebitLink);

			Assert.IsNotNull(Sut.SetupNewDirectDebitPopupElement?.Popup);
			Assert.IsNotNull(Sut.SetupNewDirectDebitPopupElement?.NameInput);
			Assert.IsNotNull(Sut.SetupNewDirectDebitPopupElement?.IbanInput);

			Assert.AreEqual(string.Empty, Sut.SetupNewDirectDebitPopupElement.NameInputError?.TextContent);
			Assert.AreEqual(string.Empty, Sut.SetupNewDirectDebitPopupElement.IbanError?.TextContent);

			Assert.IsNotNull(Sut.SetupNewDirectDebitPopupElement?.AddDebitCardButton);
		}

		[Test]
		public async Task CanSeeError()
		{
			var page = (await Sut.ClickOnElement(Sut.ExistingDirectDebitElement.UseExistingDirectDebitButton))
				.CurrentPageAs<Step3PaymentDetailsPage>();
			Assert.AreEqual("Please confirm that you are authorised to provide Electric Ireland with this information", page.ExistingDirectDebitElement?.ConfirmUseExistingDebitError.TextContent);
		}

		[Test]
		public async Task CanSelectExistingPaymentMethod()
		{
			Sut.ExistingDirectDebitElement.ConfirmUseExistingDebit.IsChecked = true;
			var page = (await Sut.ClickOnElement(Sut.ExistingDirectDebitElement.UseExistingDirectDebitButton))
				.CurrentPageAs<Step4BillingFrequencyPage>();
		}

		[Test]
		public async Task SetupNewDirectDebitPopupSeeError()
		{
			Sut.SetupNewDirectDebitPopupElement.NameInput.Value = App.Fixture.Create<string>();
			Sut.SetupNewDirectDebitPopupElement.IbanInput.Value = App.Fixture.Create<string>();
			var page = (await Sut.ClickOnElement(Sut.SetupNewDirectDebitPopupElement.AddDebitCardButton))
				.CurrentPageAs<Step3PaymentDetailsPage>();
			Assert.AreEqual("Please enter a Bank Account name", page.SetupNewDirectDebitPopupElement.NameInputError?.TextContent);
			Assert.AreEqual("Please enter a valid IBAN", page.SetupNewDirectDebitPopupElement.IbanError?.TextContent);
		}

		[Test]
		public async Task CanSetupNewDirectDebit()
		{
			Sut.SetupNewDirectDebitPopupElement.NameInput.Value = "Name Surname";
			Sut.SetupNewDirectDebitPopupElement.IbanInput.Value = "IE62AIBK93104777372010";
			(await Sut.ClickOnElement(Sut.SetupNewDirectDebitPopupElement.AddDebitCardButton))
				.CurrentPageAs<Step4BillingFrequencyPage>();
		}

		[Test]
		public async Task CanAddDirectDebitAfterError()
		{
			Sut.SetupNewDirectDebitPopupElement.NameInput.Value = string.Empty;
			Sut.SetupNewDirectDebitPopupElement.IbanInput.Value = string.Empty;
			var page = (await Sut.ClickOnElement(Sut.SetupNewDirectDebitPopupElement.AddDebitCardButton))
				.CurrentPageAs<Step3PaymentDetailsPage>();
			page.SetupNewDirectDebitPopupElement.NameInput.Value = "Name Surename";
			page.SetupNewDirectDebitPopupElement.IbanInput.Value = "IE62AIBK93104777372010";
			(await page.ClickOnElement(page.SetupNewDirectDebitPopupElement.AddDebitCardButton))
				.CurrentPageAs<Step4BillingFrequencyPage>();
		}
	}
}
