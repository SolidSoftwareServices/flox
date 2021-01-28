using NUnit.Framework;
using System.Threading.Tasks;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.SmartActivation.Step3
{
	[TestFixture]
    class WhenInSmartActivationStep3_DualFuel_ManualPaymentMethodTest : WhenInSmartActivationStep3_ManualPaymentMethodTest
	{
	    protected override bool IsDualFuel => true;

	    [Test]
        public override async Task CanSeeComponents()
        {
			Assert.IsNotNull(Sut.Stepper);
			Assert.AreEqual("Get a 8% discount when you sign up for direct debit", Sut.PaymentHeading?.TextContent);
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
    }
}