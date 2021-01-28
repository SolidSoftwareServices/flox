using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step4.MoveBothAccounts
{
	class WhenElectricityAndGasAreDirectDebit : WhenMovingBothAccounts
	{
		protected override PaymentMethodType ScenarioElectricityPaymentMethodType { get; } =
			PaymentMethodType.DirectDebit;

		protected override PaymentMethodType ScenarioGasPaymentMethodType { get; } = PaymentMethodType.DirectDebit;

		protected override bool IsPRNDegistered { get; } = true;

		[Test]
		public async Task HandlesUserPath_Cancel_ThenGoBack_ThenCancel_ThenCancel_ShowsLandingPage()
		{
			await Sut.ClickOnCancelConfirm();
			App.CurrentPageAs<Step0LandingPage>();
		}

		[Test]
		public async Task HandlesUserPath_USE_EXISTING_DD_NotCheck_ConfirmContinue_ThrowsValidation()
		{
			Sut.CheckBoxConfirmContinueDebit.IsChecked = false;
			await Sut.SelectExistingDirectDebit();

			App.CurrentPageAs<ChoosePaymentOptionsPage>();
		}

		[Test]
		public async Task HandlesUserPath_USE_EXISTING_DD_ShowsStep5WithCorrectInfo()
		{
			Assert.IsNull(Sut.UseExistingDDSetupForAllAccountsCheckBox);
			Sut.CheckBoxConfirmContinueDebit.IsChecked = true;
			await Sut.SelectExistingDirectDebit();

			var gasAccount = UserConfig.GasAccounts().Single();
			var electricityAccount = UserConfig.ElectricityAccounts().Single();
			var step5 = App.CurrentPageAs<Step5ReviewAndCompletePage>();

			AssertStep5Review(step5);
			Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains("Direct Debit"));
			Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains(electricityAccount.Model.IncomingBankAccount.IBAN.Substring(electricityAccount.Model.IncomingBankAccount.IBAN.Length - 4)));

			Assert.IsTrue(step5.ShowPayments.SecondaryPaymentType.TextContent.Contains("Direct Debit"));
			Assert.IsTrue(step5.ShowPayments.SecondaryPaymentType.TextContent.Contains(gasAccount.Model.IncomingBankAccount.IBAN.Substring(gasAccount.Model.IncomingBankAccount.IBAN.Length - 4)));
		}

		[Ignore("Not Applicable")]
		public override async Task HandlesUserPath_USE_EXISTING_USE_SAME_DD_ShowsStep5WithCorrectInfo()
		{
		}

		[Ignore("Not Applicable")]
		public override async Task HandlesUserPath_USE_EXISTING_USE_INDIVIDUAL_DD_ShowsStep5WithCorrectInfo()
		{

		}

		[Test]
		public override async Task
			HandlesUserPath_NEWDD_Use_Individual_DD_Complete_Cancel_New_Complete_Complete_ShowsStep5WithCorrectInfo()
		{
			var ibanValue = "IE03AIBK93504210987045";
			var numberOfTimesToTestCancelDirectDebit = 2;

			for (var i = 0; i < numberOfTimesToTestCancelDirectDebit; i++)
			{
				await GoToSecondDirectDebitInputScreen();
				await CancelYesImSureDirectDebitFlow();
			}

			await GoToSecondDirectDebitInputScreen();
			await CompleteDirectDebitSetup();

			async Task GoToSecondDirectDebitInputScreen()
			{
				AssertUseNewForBothIsChecked(App.CurrentPageAs<ChoosePaymentOptionsPage>());
				App.CurrentPageAs<ChoosePaymentOptionsPage>().UseSameNewDirectDebitForAllAccountsCheckBox.IsChecked = false;
				var inputDirectDebitPageOne =
					(await App.CurrentPageAs<ChoosePaymentOptionsPage>().SelectNewDirectDebit())
					.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
				inputDirectDebitPageOne.AssertCancelDirectDebitAndSkipSetupLinks(shouldShowCancelLink: true,
					shouldShowSkipLink: false);
				inputDirectDebitPageOne.InputFormValues(ibanValue, AccountName, confirmTerms: true);
				var inputDirectDebitPageTwo = (await inputDirectDebitPageOne.Complete())
					.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
				inputDirectDebitPageTwo.AssertCancelDirectDebitAndSkipSetupLinks(shouldShowCancelLink: true,
					shouldShowSkipLink: false);
				inputDirectDebitPageTwo.InputFormValues(ibanValue, AccountName2, confirmTerms: true);
			}

			async Task CancelYesImSureDirectDebitFlow()
			{
				await App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>().CancelDirectDebitSetup();
			}

			async Task CompleteDirectDebitSetup()
			{
				var step5 = (await App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>().Complete())
					 .CurrentPageAs<Step5ReviewAndCompletePage>();

				AssertStep5Review(step5);
				Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains("Direct Debit"));
				Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains(ibanValue.Substring(ibanValue.Length - 4)));

				Assert.IsTrue(step5.ShowPayments.SecondaryPaymentType.TextContent.Contains("Direct Debit"));
				Assert.IsTrue(step5.ShowPayments.SecondaryPaymentType.TextContent.Contains(ibanValue.Substring(ibanValue.Length - 4)));
			}
		}

		[Ignore("Not Applicable")]
		public override async Task
			HandlesUserPath_NEWDD_USE_INDIVIDUAL_DD_SKIP_then_COMPLETE_ShowsStep5WithCorrectInfo()
		{
		}

		[Test]
		public override async Task HandlesUserPath_NEWDD_USE_INDIVIDUAL_DD_ShowsStep5WithCorrectInfo()
		{
			Sut.UseSameNewDirectDebitForAllAccountsCheckBox.IsChecked = false;
			await Sut.SelectNewDirectDebit();

			var inputPage = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			inputPage.AssertCancelDirectDebitAndSkipSetupLinks(shouldShowCancelLink: true, shouldShowSkipLink: false);
			AssertElectricityAccountHeader(inputPage);

			Assert.AreEqual("Continue to gas direct debit", inputPage.CompleteDirectDebitSetup.TextContent);
			var ibanValue = "IE03AIBK93504210987045";
			inputPage.InputFormValues(ibanValue, AccountName);
			await inputPage.Complete();

			inputPage = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			inputPage.AssertCancelDirectDebitAndSkipSetupLinks(shouldShowCancelLink: true, shouldShowSkipLink: false);

			AssertGasAccountHeader(inputPage);
			Assert.AreEqual("Complete Direct Debit Setup", inputPage.CompleteDirectDebitSetup.TextContent);
			var ibanValue2 = "IE07BOFI90159756578872";
			inputPage.InputFormValues(ibanValue2, AccountName2);
			await inputPage.Complete();

			var step5 = App.CurrentPageAs<Step5ReviewAndCompletePage>();

			AssertStep5Review(step5);
			Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains("Direct Debit"));
			Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains(ibanValue.Substring(ibanValue.Length - 4)));

			Assert.IsTrue(step5.ShowPayments.SecondaryPaymentType.TextContent.Contains("Direct Debit"));
			Assert.IsTrue(step5.ShowPayments.SecondaryPaymentType.TextContent.Contains(ibanValue2.Substring(ibanValue2.Length - 4)));
		}
	}
}
