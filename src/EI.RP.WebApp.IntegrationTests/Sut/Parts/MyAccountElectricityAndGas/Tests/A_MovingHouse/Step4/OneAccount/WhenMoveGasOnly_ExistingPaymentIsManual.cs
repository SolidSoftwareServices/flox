using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step4.OneAccount
{
	class WhenMoveGasOnly_ExistingPaymentIsManual : WhenMovingOneAccount
	{
		protected override ClientAccountType ScenarioAccountType { get; } = ClientAccountType.Gas;
		protected override PaymentMethodType ScenarioPaymentMethodType { get; } = PaymentMethodType.Manual;

		protected override bool IsPRNDegistered { get; } = false;

		[Test]
		public async Task HandlesUserPath_NEW_SkipDirectDebit_ConfirmSkip_ShowsStep5WithCorrectInfo()
		{
			var directDebitSut = (await Sut.SelectNewDirectDebit())
				.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();

			AssertPageComponents(directDebitSut);
			directDebitSut.AssertCancelDirectDebitAndSkipSetupLinks(false, true);
			var step5 = (await directDebitSut.SelectManual()).CurrentPageAs<Step5ReviewAndCompletePage>();

			AssertStep5Review(step5);
			Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains("Manual"));
		}

		void AssertPageComponents(InputDirectDebitDetailsWhenMovingHomePage inputDirectDebitDetailsWhenMovingHomePage)
		{
			Assert.IsNotNull(inputDirectDebitDetailsWhenMovingHomePage.CustomerName);
			Assert.IsNotNull(inputDirectDebitDetailsWhenMovingHomePage.Iban);
			Assert.IsNotNull(inputDirectDebitDetailsWhenMovingHomePage.ConfirmTerms);
		}
	}
}