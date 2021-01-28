using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step4.MoveAndAddAccount
{
	abstract class WhenMoveElectricityAndAddGas_ExistingPaymentIsDirectDebit : WhenMoveOneAccountAndAddOtherAccount
	{
		protected override ClientAccountType ScenarioAccountType { get; } = ClientAccountType.Electricity;
		protected override PaymentMethodType ScenarioPaymentMethodType { get; } = PaymentMethodType.DirectDebit;

		protected override bool IsPRNDegistered { get; } = true;

		[Test]
		public override async Task TheOptionsScreenIsCorrect()
		{
			Assert.IsTrue(Sut.UseUpExistingDirectDebitHeader.TextContent.Contains("Use Existing Direct Debit"));
			Assert.IsTrue(Sut.ExistingDirectDebitDivContent.TextContent.Contains("You have Direct Debit already set up to pay your bills"));
			Assert.IsNotNull(Sut.CheckBoxUseExistingDDSetupForAllAccounts);
			Assert.IsNotNull(Sut.LabelUseExistingDDSetupForAllAccounts);
			Assert.IsTrue(Sut.LabelUseExistingDDSetupForAllAccounts.TextContent.Contains("Use this Direct Debit to make payments for both my Electricity and Gas accounts"));
			Assert.IsNotNull(Sut.CheckBoxConfirmContinueDebit);
			Assert.IsNotNull(Sut.LabelConfirmContinueDebit);
		}

		[Test]
		public async Task HandlerUserPath_UserExistingDDForBothAccounts_ConfirmChecked_GoesToNextStep()
		{
			Sut.ConfirmContinueDebit.IsChecked = true;

			(await Sut.ClickOnElement(Sut.UseExistingDirectDebitButton))
				.CurrentPageAs<Step5ReviewAndCompletePage>();
		}

		[Test]
		public async Task HandlerUserPath_DoNotUseExistingDDForBothAccounts_UseDDForOtherAccount_GoesToNextStep()
		{
			var electricityAccount = UserConfig.ElectricityAccounts().FirstOrDefault();
			var gasAccount = UserConfig.GasAccounts().FirstOrDefault();
			Sut.ConfirmContinueDebit.IsChecked = true;
			Sut.UseExistingDDSetupForAllAccountsCheckBox.IsChecked = false;
			var directDebitSut = (await Sut.ClickOnElement(Sut.UseExistingDirectDebitButton))
				.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			Assert.IsNotNull(directDebitSut.SkipElement);
			if (electricityAccount != null)
			{
				Assert.AreEqual(electricityAccount.Model.IncomingBankAccount.IBAN.Substring
						(electricityAccount.Model.IncomingBankAccount.IBAN.Length - 4),
					directDebitSut.Iban.Value.Substring(directDebitSut.Iban.Value.Length - 4));
			}
			else if (gasAccount != null)
			{
				Assert.AreEqual(gasAccount.Model.IncomingBankAccount.IBAN.Substring
						(gasAccount.Model.IncomingBankAccount.IBAN.Length - 4),
					directDebitSut.Iban.Value.Substring(directDebitSut.Iban.Value.Length - 4));
			}

			var iban = directDebitSut.Iban.Value.Substring(directDebitSut.Iban.Value.Length - 4);
			directDebitSut.InputFormValues();
			var step5ReviewPage = (await directDebitSut.Complete()).CurrentPageAs<Step5ReviewAndCompletePage>();
			AssertStep5Review(step5ReviewPage);
			Assert.IsTrue(step5ReviewPage.ShowPayments.PrimaryPaymentType.TextContent.Contains("Direct Debit"));
			Assert.IsTrue(step5ReviewPage.ShowPayments.PrimaryPaymentType.TextContent.Contains(iban));
			Assert.IsTrue(step5ReviewPage.ShowPayments.SecondaryPaymentType.TextContent.Contains("Direct Debit"));
			Assert.IsTrue(step5ReviewPage.ShowPayments.SecondaryPaymentType.TextContent.Contains("4037"));

			Assert.IsTrue(
				step5ReviewPage.ShowPricePlan.PricePlanText.TextContent.Contains("Your price plan and savings will reflect your details displayed above."));
		}

		[Test]
		public async Task HandlerUserPath_DoNotUseExistingDDForBothAccounts_UseManualForOther_GoesToNextStep()
		{
			var electricityAccount = UserConfig.ElectricityAccounts().FirstOrDefault();
			var gasAccount = UserConfig.GasAccounts().FirstOrDefault();
			Sut.ConfirmContinueDebit.IsChecked = true;
			Sut.UseExistingDDSetupForAllAccountsCheckBox.IsChecked = false;
			await Sut.SelectExistingDirectDebit();
			var directDebitSut = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();

			if (electricityAccount != null)
			{
				Assert.AreEqual(electricityAccount.Model.IncomingBankAccount.IBAN.Substring
						(electricityAccount.Model.IncomingBankAccount.IBAN.Length - 4),
					directDebitSut.Iban.Value.Substring(directDebitSut.Iban.Value.Length - 4));
			}
			else if (gasAccount != null)
			{
				Assert.AreEqual(gasAccount.Model.IncomingBankAccount.IBAN.Substring
						(gasAccount.Model.IncomingBankAccount.IBAN.Length - 4),
					directDebitSut.Iban.Value.Substring(directDebitSut.Iban.Value.Length - 4));
			}

			var iban = directDebitSut.Iban.Value.Substring(directDebitSut.Iban.Value.Length - 4);
			await directDebitSut.SelectManual();
			var step5ReviewPage = App.CurrentPageAs<Step5ReviewAndCompletePage>();
			AssertStep5Review(step5ReviewPage);
			Assert.IsTrue(step5ReviewPage.ShowPayments.PrimaryPaymentType.TextContent.Contains("Direct Debit"));
			Assert.IsTrue(step5ReviewPage.ShowPayments.PrimaryPaymentType.TextContent.Contains(iban));
			Assert.IsTrue(step5ReviewPage.ShowPayments.SecondaryPaymentType.TextContent.Contains("Manual"));
			Assert.IsTrue(
				step5ReviewPage.ShowPricePlan.PricePlanText.TextContent.Contains("Your price plan and savings will reflect your details displayed above."));
		}

		[Test]
		public async Task HandlerUserPath_UserExistingDDForBothAccounts_ConfirmNotChecked_ThrowsError()
		{
			Sut.ConfirmContinueDebit.IsChecked = false;

			(await Sut.ClickOnElement(Sut.UseExistingDirectDebitButton))
				.CurrentPageAs<ChoosePaymentOptionsPage>();

		}

		[Test]
		public virtual async Task TheDirectDebitScreenShowsCanCelButton_WhenUseSameForAllAccountsCheckBox_False()
		{
			Sut.UseSameNewDirectDebitForAllAccountsCheckBox.IsChecked = false;
			await Sut.SelectNewDirectDebit();
			var directDebitSut = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			Assert.IsNotNull(directDebitSut.CustomerName);
			Assert.IsNotNull(directDebitSut.Iban);
			Assert.IsNotNull(directDebitSut.ConfirmTerms);
			Assert.IsFalse(directDebitSut.ConfirmTerms.IsChecked);
			Assert.IsNotNull(directDebitSut.CancelDirectDebitSetupLink);
			directDebitSut.InputFormValues();
			directDebitSut = (await directDebitSut.Complete()).CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			Assert.IsNotNull(directDebitSut.CancelDirectDebitSetupLink);

			directDebitSut.InputFormValues();
			var step5ReviewPage = (await directDebitSut.Complete()).CurrentPageAs<Step5ReviewAndCompletePage>();
			AssertStep5Review(step5ReviewPage);
			Assert.IsTrue(step5ReviewPage.ShowPayments.PrimaryPaymentType.TextContent.Contains("Direct Debit"));
			Assert.IsTrue(step5ReviewPage.ShowPayments.SecondaryPaymentType.TextContent.Contains("Direct Debit"));
			Assert.IsTrue(
				step5ReviewPage.ShowPricePlan.PricePlanText.TextContent.Contains("Your price plan and savings will reflect your details displayed above."));
		}

		[Test]
		public virtual async Task TheDirectDebitScreenShowsCanCelButton_ClickCancel_RedirectToPaymentOptions()
		{
			Sut.UseSameNewDirectDebitForAllAccountsCheckBox.IsChecked = false;
			await Sut.SelectNewDirectDebit();
			var directDebitSut = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			Assert.IsNotNull(directDebitSut.CustomerName);
			Assert.IsNotNull(directDebitSut.Iban);
			Assert.IsNotNull(directDebitSut.ConfirmTerms);
			Assert.IsFalse(directDebitSut.ConfirmTerms.IsChecked);
			Assert.IsNotNull(directDebitSut.CancelDirectDebitSetupLink);
			(await directDebitSut.ClickOnElement(directDebitSut.CancelDirectDebitSetupYesImSureButton)).
				CurrentPageAs<ChoosePaymentOptionsPage>();
		}

		[Test]
		public virtual async Task TheDirectDebitScreen_ClickCancel_FromSecondaryClientTypeDirectDebitPage_RedirectToPaymentOptions()
		{
			Sut.UseSameNewDirectDebitForAllAccountsCheckBox.IsChecked = false;
			await Sut.SelectNewDirectDebit();
			var directDebitSut = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			Assert.IsNotNull(directDebitSut.CustomerName);
			Assert.IsNotNull(directDebitSut.Iban);
			Assert.IsNotNull(directDebitSut.ConfirmTerms);
			Assert.IsFalse(directDebitSut.ConfirmTerms.IsChecked);
			Assert.IsNotNull(directDebitSut.CancelDirectDebitSetupLink);

			directDebitSut.InputFormValues();
			directDebitSut = (await directDebitSut.Complete()).CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			Assert.IsNotNull(directDebitSut.CancelDirectDebitSetupLink);

			(await directDebitSut.ClickOnElement(directDebitSut.CancelDirectDebitSetupYesImSureButton)).
				CurrentPageAs<ChoosePaymentOptionsPage>();

		}

		[Test]
		public async Task TheDirectDebitScreenShowsCanCelButton_WhenUseSameForAllAccountsCheckBox_True()
		{
			Sut.UseSameNewDirectDebitForAllAccountsCheckBox.IsChecked = true;
			await Sut.SelectNewDirectDebit();
			var directDebitSut = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			Assert.IsNotNull(directDebitSut.CustomerName);
			Assert.IsNotNull(directDebitSut.Iban);
			Assert.IsNotNull(directDebitSut.ConfirmTerms);
			Assert.IsNotNull(directDebitSut.CancelDirectDebitSetupLink);
			directDebitSut.InputFormValues();
			var step5ReviewPage = (await directDebitSut.Complete()).CurrentPageAs<Step5ReviewAndCompletePage>();
			AssertStep5Review(step5ReviewPage);
			Assert.IsTrue(step5ReviewPage.ShowPayments.PrimaryPaymentType.TextContent.Contains("Direct Debit"));
			Assert.IsTrue(step5ReviewPage.ShowPayments.SecondaryPaymentType.TextContent.Contains("Direct Debit"));
			Assert.IsTrue(
				step5ReviewPage.ShowPricePlan.PricePlanText.TextContent.Contains("Your price plan and savings will reflect your details displayed above."));
		}
	}
}
