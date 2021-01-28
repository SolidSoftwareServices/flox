using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step4.MoveElectricityAndCloseGas
{
    class WhenElectricityAndGasAreDirectDebit: WhenMovingElectricityAndClosingGas
    {
        protected override PaymentMethodType ScenarioElectricityPaymentMethodType { get; } = PaymentMethodType.DirectDebit;

        protected override PaymentMethodType ScenarioGasPaymentMethodType { get; } = PaymentMethodType.DirectDebit;

        protected override bool IsPRNDegistered { get; } = false;

		[Test]
        public override async Task HandlesUserPath_NEW_InputComplete_ShowsStep5WithCorrectInfo()
        {
            const string iban = "IE65AIBK93104715784037";

            var inputDirectDebitDetailsPage = (await Sut.SelectNewDirectDebit()).CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
            inputDirectDebitDetailsPage.Iban.Value = iban;
            inputDirectDebitDetailsPage.CustomerName.Value = "Ulster Bank";
            inputDirectDebitDetailsPage.TermsAndConditions.IsChecked = true;

            inputDirectDebitDetailsPage.AssertCancelDirectDebitAndSkipSetupLinks(shouldShowCancelLink: true, shouldShowSkipLink: false);
            var step5 = (await inputDirectDebitDetailsPage.ClickOnElement(inputDirectDebitDetailsPage.CompleteDirectDebitSetup))
                .CurrentPageAs<Step5ReviewAndCompletePage>();

            AssertStep5Review(step5);
            Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains("Direct Debit"));
            Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains(iban.Substring(iban.Length - 4)));
			Assert.IsTrue(step5.ShowPricePlan.PricePlanText.TextContent.Contains("Your price plan and savings will reflect your details displayed above."),step5.ShowPricePlan.PricePlanText.TextContent);
		}
	}
}