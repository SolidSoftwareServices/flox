using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step4.OneAccount
{
	[TestFixture]
	class WhenMoveElectricityOnly_ExistingPaymentIsDirectDebit : WhenMovingOneAccount
	{
		protected override ClientAccountType ScenarioAccountType { get; } = ClientAccountType.Electricity;
		protected override PaymentMethodType ScenarioPaymentMethodType { get; } = PaymentMethodType.DirectDebit;

		protected override bool IsPRNDegistered { get; } = true;

		const string _ValidIban = "IE65AIBK93104715784037";
		const string _InvalidIban = "this is an invalid iban number";
		const string _CustomerAccountName = "Customer Account";

		[Test]
		public void TheOptionsScreenHasUseExistingDirectDebit()
		{
			Sut.FirstOptionHeader.TextContent = "Use Existing Direct Debit";
		}

		[Test]
		public async Task HandlerUserPath_UserHasExistingDD_GetsWarningWhenConfirmNotChecked()
		{
			Sut = (await Sut.ClickOnElement(Sut.UseExistingDirectDebitButton))
				.CurrentPageAs<ChoosePaymentOptionsPage>();

			Assert.IsTrue(
				Sut.UseExistingDirectDebitConfirmationNotCheckedErrorMessage.TextContent.Contains("Please confirm that you are authorised to provide Electric Ireland with this information"));
		}

		[Test]
		public virtual async Task HandlerUserPath_UserHasExistingDD_ConfirmChecked_GoesToNextStep()
		{
			Sut.ConfirmContinueDebit.IsChecked = true;
			var account = UserConfig.ElectricityAccounts().Single();

			var step5 = (await Sut.ClickOnElement(Sut.UseExistingDirectDebitButton))
				.CurrentPageAs<Step5ReviewAndCompletePage>();
			AssertStep5Review(step5);
			Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains("Direct Debit"));
			Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains(account.Model.IncomingBankAccount.IBAN.Substring(account.Model.IncomingBankAccount.IBAN.Length - 4)));
		}

		[Test]
		public async Task HandlesUserPath_NEW_Cancel_ConfirmCancel_ShowsChoosePaymentOption()
		{
			var inputDirectDebitDetailsPage = (await Sut.SelectNewDirectDebitFromDialog())
				.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();

			(await inputDirectDebitDetailsPage.ClickOnElement(inputDirectDebitDetailsPage.CancelDirectDebitSetupYesImSureButton))
				.CurrentPageAs<ChoosePaymentOptionsPage>();
		}

		[Test]
		public async Task HandlesUserPath_NEW_InputWithErrors()
		{
			var inputDirectDebitDetailsPage = (await Sut.SelectNewDirectDebit()).CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();

			AssertDirectDebitInputFieldsAreBlankOnFirstLoad();

			await ClickCompleteDirectDebitSetup();

			AssertValidationErrorMessages(expectIbanValidationError: true, expectCustomerNameValidationError: true, expectTermsAndConditionsValidationError: true);

			SetDirectDebitInputFields(inputDirectDebitDetailsPage, _ValidIban, customerAccountName: null, isTermsAndConditionsChecked: false);
			await ClickCompleteDirectDebitSetup();

			AssertValidationErrorMessages(expectIbanValidationError: false, expectCustomerNameValidationError: true, expectTermsAndConditionsValidationError: true);
			AssertFieldHasText(inputDirectDebitDetailsPage.Iban, _ValidIban);


			SetDirectDebitInputFields(inputDirectDebitDetailsPage, iban: string.Empty, customerAccountName: string.Empty, isTermsAndConditionsChecked: false);
			await ClickCompleteDirectDebitSetup();
			AssertFieldHasText(inputDirectDebitDetailsPage.Iban, string.Empty);
			AssertFieldHasText(inputDirectDebitDetailsPage.CustomerName, string.Empty);

			SetDirectDebitInputFields(inputDirectDebitDetailsPage, _ValidIban, _CustomerAccountName, isTermsAndConditionsChecked: false);
			await ClickCompleteDirectDebitSetup();

			AssertValidationErrorMessages(expectIbanValidationError: false, expectCustomerNameValidationError: false, expectTermsAndConditionsValidationError: true);
			AssertFieldHasText(inputDirectDebitDetailsPage.Iban, _ValidIban);
			AssertFieldHasText(inputDirectDebitDetailsPage.CustomerName, _CustomerAccountName);

			SetDirectDebitInputFields(inputDirectDebitDetailsPage, _InvalidIban, _CustomerAccountName, isTermsAndConditionsChecked: true);
			await ClickCompleteDirectDebitSetup();

			AssertValidationErrorMessages(expectIbanValidationError: true, expectCustomerNameValidationError: false, expectTermsAndConditionsValidationError: false);

			SetDirectDebitInputFields(inputDirectDebitDetailsPage, _ValidIban, _CustomerAccountName, isTermsAndConditionsChecked: true);

			var step5ReviewAndCompletePage = (await inputDirectDebitDetailsPage.ClickOnElement(inputDirectDebitDetailsPage.CompleteDirectDebitSetup))
				.CurrentPageAs<Step5ReviewAndCompletePage>();

			async Task ClickCompleteDirectDebitSetup()
			{
				inputDirectDebitDetailsPage = (await inputDirectDebitDetailsPage.ClickOnElement(inputDirectDebitDetailsPage.CompleteDirectDebitSetup))
					.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			}

			void AssertDirectDebitInputFieldsAreBlankOnFirstLoad()
			{
				Assert.IsTrue(inputDirectDebitDetailsPage.Iban.Value == string.Empty);
				Assert.IsTrue(inputDirectDebitDetailsPage.CustomerName.Value == string.Empty);
			}

			void AssertFieldHasText(IHtmlInputElement inputElement, string expectedText)
			{
				Assert.AreEqual(expectedText, inputElement.Value);
			}

			void AssertValidationErrorMessages(
				bool expectIbanValidationError,
				bool expectCustomerNameValidationError,
				bool expectTermsAndConditionsValidationError)
			{
				Assert.IsTrue(
					expectIbanValidationError ?
						inputDirectDebitDetailsPage.IbanErrorMessage.TextContent == "Please enter a valid IBAN" :
						inputDirectDebitDetailsPage.IbanErrorMessage?.TextContent == string.Empty);

				Assert.IsTrue(
					expectCustomerNameValidationError ?
						inputDirectDebitDetailsPage.CustomerNameErrorMessage.TextContent == "Please enter a Bank Account name" :
						inputDirectDebitDetailsPage.CustomerNameErrorMessage?.TextContent == string.Empty);

				Assert.IsTrue(
					expectTermsAndConditionsValidationError ?
						inputDirectDebitDetailsPage.TermsAndConditionsErrorMessage.TextContent == "Please confirm that you have read and accept the Electric Ireland Terms and Conditions" :
						inputDirectDebitDetailsPage.TermsAndConditionsErrorMessage?.TextContent == string.Empty);
			}
		}

		[Test]
		public async Task HandlesUserPath_NEW_Cancel_GoBack_Input_Complete_ShowsStep5WithCorrectInfo()
		{
			//New
			var inputDirectDebitDetailsPage = (await Sut.SelectNewDirectDebit())
				.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();

			//cancel
			var choosePaymentOptions = (await inputDirectDebitDetailsPage.ClickOnElement(inputDirectDebitDetailsPage.CancelDirectDebitSetupYesImSureButton))
				.CurrentPageAs<ChoosePaymentOptionsPage>();

			//GoBack
			inputDirectDebitDetailsPage = (await choosePaymentOptions.ClickOnElement(choosePaymentOptions.SetUpNewDirectDebitButton))
				.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();

			//input
			SetDirectDebitInputFields(inputDirectDebitDetailsPage, _ValidIban, _CustomerAccountName, isTermsAndConditionsChecked: true);

			//complete
			var step5 = (await inputDirectDebitDetailsPage.ClickOnElement(inputDirectDebitDetailsPage.CompleteDirectDebitSetup))
				.CurrentPageAs<Step5ReviewAndCompletePage>();

			//correct info
			Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains(_ValidIban.Substring(_ValidIban.Length - 4)));
		}

		private void SetDirectDebitInputFields(
			InputDirectDebitDetailsWhenMovingHomePage inputDirectDebitDetailsPage,
			string iban,
			string customerAccountName,
			bool isTermsAndConditionsChecked)
		{
			inputDirectDebitDetailsPage.Iban.Value = iban;
			inputDirectDebitDetailsPage.CustomerName.Value = customerAccountName;
			inputDirectDebitDetailsPage.TermsAndConditions.IsChecked = isTermsAndConditionsChecked;
		}
	}
}