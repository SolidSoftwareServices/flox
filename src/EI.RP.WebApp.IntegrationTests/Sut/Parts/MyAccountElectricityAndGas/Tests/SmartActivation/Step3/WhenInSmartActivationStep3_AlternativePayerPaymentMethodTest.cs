using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SmartActivation;
using NUnit.Framework;
using System.Threading.Tasks;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.SmartActivation.Step3
{
	[TestFixture]
    class WhenInSmartActivationStep3_AlternativePayerPaymentMethodTest :WhenInSmartActivationStep3Test
    {
	    protected override PaymentMethodType PaymentMethod => PaymentMethodType.DirectDebitNotAvailable;

	    [Test]
		public override async Task CanSeeComponents()
		{
			Assert.IsNotNull(Sut.Stepper);
			Assert.AreEqual("Payment details", Sut.PaymentHeading?.TextContent);
			Assert.IsNull(Sut.ExistingDirectDebitElement?.ExistingDirectDebitOption);
			Assert.IsNull(Sut.SetupDirectDebitElementForManualPayment?.SetupNewDirectDebitOption);
			Assert.IsNull(Sut.SetupNewDirectDebitPopupElement?.Popup);
			
			Assert.IsNotNull(Sut.AlternativePayerElement?.MainElement);
			Assert.IsNotNull(Sut.AlternativePayerElement?.ContinueButton);
			
			Assert.AreEqual("You are currently paying one or more of your energy accounts through a third party. This means that you will have limited online viewing and payment functionality.", Sut.AlternativePayerElement?.Message?.TextContent.Trim());
			Assert.AreEqual("If you have any queries regarding payment amounts, please contact your Credit Union.", Sut.AlternativePayerElement?.QueryMessage?.TextContent.Trim());
		}

		[Test]
		public async Task CanContinueToNextStep()
		{
			(await Sut.ClickOnElement(Sut.AlternativePayerElement?.ContinueButton))
				.CurrentPageAs<Step4BillingFrequencyPage>();
		}
	}
}
