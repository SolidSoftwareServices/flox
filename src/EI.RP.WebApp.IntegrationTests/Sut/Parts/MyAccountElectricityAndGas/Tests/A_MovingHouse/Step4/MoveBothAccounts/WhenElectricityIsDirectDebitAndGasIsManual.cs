using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step4.MoveBothAccounts
{
	internal class WhenElectricityIsDirectDebitAndGasIsManual : WhenMovingBothAccounts
	{
		protected override PaymentMethodType ScenarioElectricityPaymentMethodType { get; } = PaymentMethodType.DirectDebit;
		protected override PaymentMethodType ScenarioGasPaymentMethodType { get; } = PaymentMethodType.Manual;

		protected override bool IsPRNDegistered { get; } = true;

		[Test]
		public override async Task HandlesUserPath_USE_EXISTING_USE_INDIVIDUAL_DD_ShowsStep5WithCorrectInfo()
		{
			var electricityAccountInfo = UserConfig.Accounts.ElementAt(1);
			var gasAccountInfo = UserConfig.Accounts.ElementAt(0);

			Sut.CheckBoxUseExistingDDSetupForAllAccounts.IsChecked = false;
			Sut.ConfirmDetailsAreCorrectExistingDirectDebitCheckBox.IsChecked = true;

			await Sut.SelectExistingDirectDebit();

			var inputPage = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();

			AssertGasAccountHeader(inputPage);
			AssertDirectDebitInputFieldsArePopulated(inputPage, UserConfig.ElectricityAccounts().First());
			inputPage.AssertCancelDirectDebitAndSkipSetupLinks(shouldShowCancelLink: false, shouldShowSkipLink: true);

			var electricityAccount = UserConfig.ElectricityAccounts().Single();
			var gasAccountCustomerName = "Mrs. Gas Account";
			var ibanValue = "IE03AIBK93504210987045";
			inputPage.Iban.Value = ibanValue;
			inputPage.InputFormValues(ibanValue, gasAccountCustomerName, true);

			var step5 = (await inputPage.Complete()).CurrentPageAs<Step5ReviewAndCompletePage>();

			AssertStep5Review(step5);
			Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains("Direct Debit"));
			Assert.IsTrue(step5.ShowPayments.SecondaryPaymentType.TextContent.Contains("Direct Debit"));
			Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains(electricityAccount.Model.IncomingBankAccount.IBAN.Substring(electricityAccount.Model.IncomingBankAccount.IBAN.Length - 4)));
			Assert.IsTrue(step5.ShowPayments.SecondaryPaymentType.TextContent.Contains(ibanValue.Substring(ibanValue.Length - 4)));


			Assert.IsTrue(
				step5.ShowPricePlan.PricePlanText.TextContent.Contains("Your price plan and savings will reflect your details displayed above."));
		}

		[Test]
		public override async Task HandlesUserPath_NEWDD_USE_INDIVIDUAL_DD_ShowsStep5WithCorrectInfo()
		{
			Sut.UseSameNewDirectDebitForAllAccountsCheckBox.IsChecked = false;
			var inputPage = (await Sut.SelectNewDirectDebit()).CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();

			inputPage.AssertCancelDirectDebitAndSkipSetupLinks(shouldShowCancelLink: true, shouldShowSkipLink: false);
			AssertDirectDebitInputFieldsAreBlank(inputPage);
			AssertElectricityAccountHeader(inputPage);
			Assert.AreEqual("Continue to gas direct debit", inputPage.CompleteDirectDebitSetup.TextContent);
			await AssertDirectDebitInputFieldValidation(inputPage);
			inputPage = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			var ibanValue = "IE03AIBK93504210987045";
			inputPage.InputFormValues(ibanValue, AccountName);
			await inputPage.Complete();

			inputPage = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			inputPage.AssertCancelDirectDebitAndSkipSetupLinks(shouldShowCancelLink: false, shouldShowSkipLink: true);
			AssertDirectDebitInputFieldsAreBlank(inputPage);
			AssertGasAccountHeader(inputPage);
			Assert.AreEqual("Complete Direct Debit Setup", inputPage.CompleteDirectDebitSetup.TextContent);
			await AssertDirectDebitInputFieldValidation(inputPage);
			inputPage = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			var ibanValue2 = "IE07BOFI90159756578872";
			inputPage.InputFormValues(ibanValue2, AccountName2);
			await inputPage.Complete();

			var step5 = App.CurrentPageAs<Step5ReviewAndCompletePage>();

			AssertStep5Review(step5);

			Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains("Direct Debit"));
			Assert.IsTrue(step5.ShowPayments.SecondaryPaymentType.TextContent.Contains("Direct Debit"));
			Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains(ibanValue.Substring(ibanValue.Length - 4)));
			Assert.IsTrue(step5.ShowPayments.SecondaryPaymentType.TextContent.Contains(ibanValue2.Substring(ibanValue2.Length - 4)));

			Assert.IsTrue(
				step5.ShowPricePlan.PricePlanText.TextContent.Contains("Your price plan and savings will reflect your details displayed above."));
		}

		[Test]
		public override async Task HandlesUserPath_NEWDD_USE_INDIVIDUAL_DD_SKIP_then_COMPLETE_ShowsStep5WithCorrectInfo()
		{
			Sut.UseSameNewDirectDebitForAllAccountsCheckBox.IsChecked = false;
			var inputPage = (await Sut.SelectNewDirectDebit()).CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			inputPage.AssertCancelDirectDebitAndSkipSetupLinks(shouldShowCancelLink: true, shouldShowSkipLink: false);
			AssertDirectDebitInputFieldsAreBlank(inputPage);
			await AssertDirectDebitInputFieldValidation(inputPage);
			inputPage = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			inputPage = (await inputPage.Skip()).CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();

			inputPage.AssertCancelDirectDebitAndSkipSetupLinks(shouldShowCancelLink: false, shouldShowSkipLink: true);
			AssertGasAccountHeader(inputPage);
			AssertDirectDebitInputFieldsAreBlank(inputPage);
			await AssertDirectDebitInputFieldValidation(inputPage);
			inputPage = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			var ibanValue2 = "IE07BOFI90159756578872";
			inputPage.InputFormValues(ibanValue2, AccountName2);

			await inputPage.Complete();

			var step5 = App.CurrentPageAs<Step5ReviewAndCompletePage>();
			AssertStep5Review(step5);
			Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains("Manual"));
			Assert.IsTrue(step5.ShowPayments.SecondaryPaymentType.TextContent.Contains("Direct Debit"));

			Assert.IsTrue(step5.ShowPayments.SecondaryPaymentType.TextContent.Contains(ibanValue2.Substring(ibanValue2.Length - 4)));

			Assert.IsTrue(
				step5.ShowPricePlan.PricePlanText.TextContent.Contains("Your price plan and savings will reflect your details displayed above."));
		}

		[Test]
		public async Task HandlesUserPath_NEWDD_USE_INDIVIDUAL_DD_Cancel_ShowsStep4()
		{
			Sut.UseSameNewDirectDebitForAllAccountsCheckBox.IsChecked = false;

			var inputPage = (await Sut.SelectNewDirectDebit()).CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();

			inputPage.AssertCancelDirectDebitAndSkipSetupLinks(shouldShowCancelLink: true, shouldShowSkipLink: false);

			(await inputPage.CancelDirectDebitSetup()).CurrentPageAs<ChoosePaymentOptionsPage>();
		}

		[Test]
		public override async Task HandlesUserPath_NEWDD_Use_Individual_DD_Complete_Cancel_New_Complete_Complete_ShowsStep5WithCorrectInfo()
		{
			var ibanValue = "IE03AIBK93504210987045";
			var numberOfTimesToTestCancelDirectDebit = 2;

			for (var i = 0; i < numberOfTimesToTestCancelDirectDebit; i++)
			{
				await GoToFirstDirectDebitInputScreen();
				await CancelYesImSureDirectDebitFlow();
				AssertUseNewForBothIsChecked(App.CurrentPageAs<ChoosePaymentOptionsPage>());
			}

			await GoToFirstDirectDebitInputScreen();
			await GoToSecondDirectDebitInputScreen();
			await CompleteDirectDebitSetup();

			async Task GoToFirstDirectDebitInputScreen()
			{
				App.CurrentPageAs<ChoosePaymentOptionsPage>().UseSameNewDirectDebitForAllAccountsCheckBox.IsChecked = false;
				var inputDirectDebitPageOne = (await App.CurrentPageAs<ChoosePaymentOptionsPage>().SelectNewDirectDebit()).CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
				inputDirectDebitPageOne.AssertCancelDirectDebitAndSkipSetupLinks(shouldShowCancelLink: true, shouldShowSkipLink: false);
				AssertElectricityAccountHeader(inputDirectDebitPageOne);
			}

			async Task GoToSecondDirectDebitInputScreen()
			{
				var inputPage = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>().InputFormValues(ibanValue, AccountName, confirmTerms: true);
				var inputDirectDebitPageTwo = (await inputPage.Complete()).CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
				AssertGasAccountHeader(inputDirectDebitPageTwo);
			}

			async Task CancelYesImSureDirectDebitFlow()
			{
				await App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>().CancelDirectDebitSetup();
			}

			async Task CompleteDirectDebitSetup()
			{
				var inputPage = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
				inputPage.InputFormValues(ibanValue, AccountName, confirmTerms: true);
				var step5 = (await inputPage.Complete()).CurrentPageAs<Step5ReviewAndCompletePage>();

				AssertStep5Review(step5);

				Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains("Direct Debit"));
				Assert.IsTrue(step5.ShowPayments.SecondaryPaymentType.TextContent.Contains("Direct Debit"));
				Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains(ibanValue.Substring(ibanValue.Length - 4)));
				Assert.IsTrue(step5.ShowPayments.SecondaryPaymentType.TextContent.Contains(ibanValue.Substring(ibanValue.Length - 4)));

				Assert.IsTrue(
					step5.ShowPricePlan.PricePlanText.TextContent.Contains("Your price plan and savings will reflect your details displayed above."));
			}
		}
	}
}