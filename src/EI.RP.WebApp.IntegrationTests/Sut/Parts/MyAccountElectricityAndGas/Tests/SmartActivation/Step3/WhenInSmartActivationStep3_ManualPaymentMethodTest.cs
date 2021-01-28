using AutoFixture;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SmartActivation;
using NUnit.Framework;
using System.Threading.Tasks;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.SmartActivation.Step3
{
	[TestFixture]
    class WhenInSmartActivationStep3_ManualPaymentMethodTest : WhenInSmartActivationStep3Test
    {
        [Test]
        public override async Task CanSeeComponents()
        {
			Assert.IsNotNull(Sut.Stepper);
			Assert.AreEqual("Get a 5% discount when you sign up for direct debit", Sut.PaymentHeading?.TextContent);
			Assert.IsNotNull(Sut.SetupDirectDebitElementForManualPayment?.SetupNewDirectDebitOption);
			Assert.IsNull(Sut.SetupDirectDebitElementForManualPayment?.SetupNewDirectDebitLink);
			Assert.IsNull(Sut.SetupNewDirectDebitPopupElement?.Popup);
			Assert.IsNull(Sut.AlternativePayerElement?.MainElement);

			Assert.IsNotNull(Sut.SetupDirectDebitElementForManualPayment?.NameInput);
			Assert.IsNotNull(Sut.SetupDirectDebitElementForManualPayment?.IbanInput);

			Assert.AreEqual(string.Empty,Sut.SetupDirectDebitElementForManualPayment?.NameInputError?.TextContent);
			Assert.AreEqual(string.Empty, Sut.SetupDirectDebitElementForManualPayment?.IbanError?.TextContent);

			Assert.IsNotNull(Sut.SetupDirectDebitElementForManualPayment?.AddDebitCardButton);
			Assert.IsNotNull(Sut.SetupDirectDebitElementForManualPayment?.SkipButton);
        }

		[Test]
        public async Task GetErrorOnWrongInputs()
        {
	        Sut.SetupDirectDebitElementForManualPayment.NameInput.Value = App.Fixture.Create<string>();
	        Sut.SetupDirectDebitElementForManualPayment.IbanInput.Value = App.Fixture.Create<string>();
			var page = (await Sut.ClickOnElement(Sut.SetupDirectDebitElementForManualPayment.AddDebitCardButton))
		        .CurrentPageAs<Step3PaymentDetailsPage>();
	        Assert.AreEqual("Please enter a Bank Account name", page.SetupDirectDebitElementForManualPayment.NameInputError?.TextContent);
	        Assert.AreEqual("Please enter a valid IBAN", page.SetupDirectDebitElementForManualPayment.IbanError?.TextContent);
		}

		[Test]
        public async Task CanSkipDirectDebitSetup()
        {
	        (await Sut.ClickOnElement(Sut.SetupDirectDebitElementForManualPayment.SkipButton))
		        .CurrentPageAs<Step4BillingFrequencyPage>();
        }

		[Test]
        public async Task CanAddDirectDebitSetup()
        {
			Sut.SetupDirectDebitElementForManualPayment.NameInput.Value = "Name Surename";
			Sut.SetupDirectDebitElementForManualPayment.IbanInput.Value = "IE62AIBK93104777372010";
			(await Sut.ClickOnElement(Sut.SetupDirectDebitElementForManualPayment.AddDebitCardButton))
				.CurrentPageAs<Step4BillingFrequencyPage>();
		}

        [Test]
        public async Task CanAddDirectDebitAfterError()
        {
	        Sut.SetupDirectDebitElementForManualPayment.NameInput.Value = string.Empty;
	        Sut.SetupDirectDebitElementForManualPayment.IbanInput.Value = string.Empty;
	        var page = (await Sut.ClickOnElement(Sut.SetupDirectDebitElementForManualPayment.AddDebitCardButton))
		        .CurrentPageAs<Step3PaymentDetailsPage>();
	        page.SetupDirectDebitElementForManualPayment.NameInput.Value = "Name Surename";
	        page.SetupDirectDebitElementForManualPayment.IbanInput.Value = "IE62AIBK93104777372010";
	        (await page.ClickOnElement(page.SetupDirectDebitElementForManualPayment.AddDebitCardButton))
		        .CurrentPageAs<Step4BillingFrequencyPage>();
		}
	}
}