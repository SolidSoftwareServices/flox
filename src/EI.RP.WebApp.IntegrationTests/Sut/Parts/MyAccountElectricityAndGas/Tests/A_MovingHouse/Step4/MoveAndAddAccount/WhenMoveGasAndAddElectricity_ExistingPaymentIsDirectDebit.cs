using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step4.MoveAndAddAccount
{
	class WhenMoveGasAndAddElectricity_ExistingPaymentIsDirectDebit : WhenMoveElectricityAndAddGas_ExistingPaymentIsDirectDebit
	{
		protected override ClientAccountType ScenarioAccountType { get; } = ClientAccountType.Gas;
		protected override PaymentMethodType ScenarioPaymentMethodType { get; } = PaymentMethodType.DirectDebit;

		protected override bool IsPRNDegistered { get; } = true;
		[Test]
		public override async Task HandlesUserPath_NEW_Skip_ConfirmSkip_WhenUseSameForAllAccountsCheckBox_False_ShowsStep5WithCorrectInfo()
		{
			Sut.UseSameNewDirectDebitForAllAccountsCheckBox.IsChecked = false;

			await Sut.SelectNewDirectDebit();
			var directDebitSut = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			AssertElectricityAccountHeader(directDebitSut);
			AssertPageComponents(directDebitSut);
			Assert.IsNotNull(directDebitSut.SkipDirectDebitSetupLink);
			await directDebitSut.SelectManual();
			directDebitSut = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			AssertGasAccountHeader(directDebitSut);
			(await directDebitSut.SelectManual()).CurrentPageAs<Step5ReviewAndCompletePage>();
		}

		[Ignore("NotApplicable")]
		[Test]
		public override async Task HandlesUserPath_NEW_Skip_ConfirmSkip_WhenUseSameForAllAccountsCheckBox_True_ShowsStep5WithCorrectInfo()
		{

		}

		[Ignore("NotApplicable")]
		[Test]
		public override async Task TheDirectDebitScreenShowsCanCelButton_ClickCancel_RedirectToPaymentOptions()
		{
		}

		[Ignore("NotApplicable")]
		[Test]
		public override async Task TheDirectDebitScreenShowsCanCelButton_WhenUseSameForAllAccountsCheckBox_False()
		{
		}

		[Test]
		public override async Task TheDirectDebitScreen_ClickCancel_FromSecondaryClientTypeDirectDebitPage_RedirectToPaymentOptions()
		{
			Sut.UseSameNewDirectDebitForAllAccountsCheckBox.IsChecked = false;
			await Sut.SelectNewDirectDebit();
			var directDebitSut = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			Assert.IsNotNull(directDebitSut.CustomerName);
			Assert.IsNotNull(directDebitSut.Iban);
			Assert.IsNotNull(directDebitSut.ConfirmTerms);
			Assert.IsFalse(directDebitSut.ConfirmTerms.IsChecked);
			Assert.IsNotNull(directDebitSut.SkipDirectDebitSetupLink);

			directDebitSut.InputFormValues();
			directDebitSut = (await directDebitSut.Complete()).CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			Assert.IsNotNull(directDebitSut.CancelDirectDebitSetupLink);

			(await directDebitSut.ClickOnElement(directDebitSut.CancelDirectDebitSetupYesImSureButton)).
				CurrentPageAs<ChoosePaymentOptionsPage>();
		}

		[Test]
		public override async Task TheDirectDebitScreenIsCorrect_WhenUseSameForAllAccountsCheckBox_False()
		{
			Sut.UseSameNewDirectDebitForAllAccountsCheckBox.IsChecked = false;
			await Sut.SelectNewDirectDebit();
			var directDebitSut = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			AssertElectricityAccountHeader(directDebitSut);
			Assert.AreEqual($"Continue to {ClientAccountType.Gas.ToString().ToLower()} direct debit", directDebitSut.CompleteDirectDebitSetup.TextContent);
			AssertPageComponents(directDebitSut);
			Assert.IsNotNull(directDebitSut.SkipDirectDebitSetupLink);
			directDebitSut.InputFormValues();
			directDebitSut = (await directDebitSut.Complete()).CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			Assert.AreEqual("Complete Direct Debit Setup", directDebitSut.CompleteDirectDebitSetup.TextContent);
			AssertGasAccountHeader(directDebitSut);
			Assert.IsNotNull(directDebitSut.CancelDirectDebitSetupLink);
			directDebitSut.InputFormValues();
			var step5ReviewPage = (await directDebitSut.Complete()).CurrentPageAs<Step5ReviewAndCompletePage>();
			AssertStep5Review(step5ReviewPage);
			Assert.IsTrue(step5ReviewPage.ShowPayments.PrimaryPaymentType.TextContent.Contains("Direct Debit"));
			Assert.IsTrue(step5ReviewPage.ShowPayments.SecondaryPaymentType.TextContent.Contains("Direct Debit"));
		}

		[Test]
		public override async Task TheDirectDebitScreenIsCorrect_WhenUseSameForAllAccountsCheckBox_True()
		{
			Sut.UseSameNewDirectDebitForAllAccountsCheckBox.IsChecked = true;
			await Sut.SelectNewDirectDebit();
			var directDebitSut = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			AssertElectricityAndGasHeader(directDebitSut);
			Assert.AreEqual("Complete Direct Debit Setup", directDebitSut.CompleteDirectDebitSetup.TextContent);
			AssertPageComponents(directDebitSut);
			Assert.IsNotNull(directDebitSut.CancelDirectDebitSetupLink);
			directDebitSut.InputFormValues();
			var step5ReviewPage = (await directDebitSut.Complete()).CurrentPageAs<Step5ReviewAndCompletePage>();

			AssertStep5Review(step5ReviewPage);
			Assert.IsTrue(step5ReviewPage.ShowPayments.PrimaryPaymentType.TextContent.Contains("Direct Debit"));
			Assert.IsTrue(step5ReviewPage.ShowPayments.PrimaryPaymentType.TextContent.Contains("4037"));
			Assert.IsTrue(step5ReviewPage.ShowPayments.SecondaryPaymentType.TextContent.Contains("Direct Debit"));
			Assert.IsTrue(step5ReviewPage.ShowPayments.SecondaryPaymentType.TextContent.Contains("4037"));
			Assert.IsTrue(
				step5ReviewPage.ShowPricePlan.PricePlanText.TextContent.Contains("Your price plan and savings will reflect your details displayed above."));
		}

		void AssertPageComponents(InputDirectDebitDetailsWhenMovingHomePage inputDirectDebitDetailsWhenMovingHomePage)
		{
			Assert.IsNotNull(inputDirectDebitDetailsWhenMovingHomePage.CustomerName);
			Assert.IsNotNull(inputDirectDebitDetailsWhenMovingHomePage.Iban);
			Assert.IsNotNull(inputDirectDebitDetailsWhenMovingHomePage.ConfirmTerms);
		}

		void AssertElectricityAndGasHeader(InputDirectDebitDetailsWhenMovingHomePage inputDirectDebitDetailsWhenMovingHomePage)
		{
			var gasAccountInfo = UserConfig.Accounts.ElementAt(0);
			Assert.AreEqual($"{ClientAccountType.Electricity} & {ClientAccountType.Gas} ({gasAccountInfo.AccountNumber})", inputDirectDebitDetailsWhenMovingHomePage.HeaderElement.TextContent);
		}

		void AssertElectricityAccountHeader(InputDirectDebitDetailsWhenMovingHomePage inputDirectDebitDetailsWhenMovingHomePage)
		{
			Assert.AreEqual($"{ClientAccountType.Electricity}",
				inputDirectDebitDetailsWhenMovingHomePage.HeaderElement.TextContent);
		}

		void AssertGasAccountHeader(InputDirectDebitDetailsWhenMovingHomePage inputDirectDebitDetailsWhenMovingHomePage)
		{
			var gasAccountInfo = UserConfig.Accounts.ElementAt(0);
			Assert.AreEqual($"{gasAccountInfo.ClientAccountType} ({gasAccountInfo.AccountNumber})",
				inputDirectDebitDetailsWhenMovingHomePage.HeaderElement.TextContent);
		}
	}
}