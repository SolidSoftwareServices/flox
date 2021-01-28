using System;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step5.MoveElectricityAndCloseGas
{
	[TestFixture]
	internal abstract class WhenMovingElectricityAndClosingGas : MyAccountCommonTests<ChoosePaymentOptionsPage>
	{

		protected abstract PaymentMethodType ScenarioElectricityPaymentMethodType { get; }
		protected abstract PaymentMethodType ScenarioGasPaymentMethodType { get; }

		protected abstract bool IsPRNDegistered { get; }

		private Step1InputMoveOutPage step1Page;

		private Step3InputMoveInPropertyDetailsPage step3Page;

		protected override async Task TestScenarioArrangement()
		{
			ConfigureAccounts();

			Sut = await ResolveScenarioSut();

			void ConfigureAccounts()
			{
				UserConfig = App.ConfigureUser("a@A.com", "test");

				var gasAccount = UserConfig.AddGasAccount(paymentType: ScenarioGasPaymentMethodType, newPrnAddressExists: true, isPrnDeregistered: IsPRNDegistered, configureDefaultDevice: false)
						.WithGasDevice();

				UserConfig.AddElectricityAccount(paymentType: ScenarioElectricityPaymentMethodType, duelFuelSisterAccount: gasAccount, isPRNDeRegistered: IsPRNDegistered, configureDefaultDevice: false)
					.WithElectricity24HrsDevices();

				UserConfig.Execute();
			}

			async Task<ChoosePaymentOptionsPage> ResolveScenarioSut()
			{
				await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role);
				await App.CurrentPageAs<AccountSelectionPage>()
					.SelectAccount(UserConfig.ElectricityAccounts().Single().Model.AccountNumber);
				await App.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMovingHome();
				var movingHomeLandingPage = App.CurrentPageAs<Step0LandingPage>();

				step1Page =
				   (await movingHomeLandingPage.ClickOnElement(movingHomeLandingPage.PopupButton2))
				   .CurrentPageAs<Step1InputMoveOutPage>()
				   .InputFormValues(UserConfig);

				var step2Page1 = (await step1Page.ClickOnElement(step1Page.GetNextPRNButton()))
					.CurrentPageAs<Step2InputPrnsPage>();

				step2Page1.InputFormValues(UserConfig);


				var step2Page2 = (await step2Page1.ClickOnElement(step2Page1.SubmitPRNS))
					.CurrentPageAs<Step2ConfirmAddressPage>();
				step3Page = (await step2Page2.ClickOnElement(step2Page2.ButtonContinue))
				   .CurrentPageAs<Step3InputMoveInPropertyDetailsPage>().InputFormValues(UserConfig);
				await step3Page.ClickOnElement(step3Page.NextPaymentOptions);

				var choosePaymentOptionsPage = App.CurrentPageAs<ChoosePaymentOptionsPage>();
				choosePaymentOptionsPage = await AssertTermsAndConditionsValidationIfNecessary(choosePaymentOptionsPage);
				return choosePaymentOptionsPage;
			}

			async Task<ChoosePaymentOptionsPage> AssertTermsAndConditionsValidationIfNecessary(ChoosePaymentOptionsPage choosePaymentOptionsPage)
			{
				if (ScenarioElectricityPaymentMethodType == PaymentMethodType.DirectDebit)
				{
					choosePaymentOptionsPage = (await choosePaymentOptionsPage.SelectExistingDirectDebit()).CurrentPageAs<ChoosePaymentOptionsPage>();
					Assert.IsTrue(
						choosePaymentOptionsPage
							.UseExistingDirectDebitConfirmationNotCheckedErrorMessage
							.TextContent
							.Contains("Please confirm that you are authorised to provide Electric Ireland with this information"));
				}

				return choosePaymentOptionsPage;
			}
		}

		[Test]
		public async Task Ste5ReviewPage_ClickOnEdit_NewProperty()
		{
			var inputDirectDebitDetailsPage = await Step4InputFormValues(Sut);

			var step5 = (await inputDirectDebitDetailsPage.ClickOnElement(inputDirectDebitDetailsPage.CompleteDirectDebitSetup))
				.CurrentPageAs<Step5ReviewAndCompletePage>();
			AssertStep5Review(step5);
			var step2InputPage = (await step5.ClickOnElement(step5.ShowPropertyDetails.EditNewAddressButton))
				.CurrentPageAs<Step2InputPrnsPage>();

			step2InputPage.InputFormValues_NewProperty(UserConfig);

			var step2Page2 = (await step2InputPage.ClickOnElement(step2InputPage.SubmitPRNS)).CurrentPageAs<Step2ConfirmAddressPage>();
			var step3Page = (await step2Page2.ClickOnElement(step2Page2.ButtonContinue)).CurrentPageAs<Step3InputMoveInPropertyDetailsPage>().InputFormValues(UserConfig); ;
			await step3Page.ClickOnElement(step3Page.NextPaymentOptions);


			var choosePaymentOptionsPage = App.CurrentPageAs<ChoosePaymentOptionsPage>();

			var inputDirectDebitDetailsPage2 = await Step4InputFormValues(choosePaymentOptionsPage);
			var step5Page = (await inputDirectDebitDetailsPage2.ClickOnElement(inputDirectDebitDetailsPage2.CompleteDirectDebitSetup))
				.CurrentPageAs<Step5ReviewAndCompletePage>();
			AssertStep5Review(step5Page);
			Assert.AreEqual(step2Page2.MPRNAddress.Text().Replace("\t", "").Replace(" ", "").Trim(),
				step5Page.ShowPropertyDetails.NewAddressInfo.Text().Replace("\t", "").Replace(" ", "").Trim());
		}

		[Test]
		public async Task Ste5ReviewPage_ClickOnMoveOutEdit_MeterReadings()
		{
			var inputDirectDebitDetailsPage = await Step4InputFormValues(Sut);

			var step5 = (await inputDirectDebitDetailsPage.ClickOnElement(inputDirectDebitDetailsPage.CompleteDirectDebitSetup))
				.CurrentPageAs<Step5ReviewAndCompletePage>();
			AssertStep5Review(step5);
			var step1InputPage = (await step5.ClickOnElement(step5.ShowMovingDatesAndMeterReadings.EditMoveOutDetailsButton))
				.CurrentPageAs<Step1InputMoveOutPage>();
			Assert.AreEqual(step1Page.MoveOutDatePicker.Value, step1InputPage.MoveOutDatePicker.Value);
			step1InputPage.InputFormValues(UserConfig);
			var step2Page1 = (await step1InputPage.ClickOnElement(step1InputPage.GetNextPRNButton()))
				.CurrentPageAs<Step2InputPrnsPage>();
			var step2Page2 = (await step2Page1.ClickOnElement(step2Page1.SubmitPRNS)).CurrentPageAs<Step2ConfirmAddressPage>();
			var step3InputPage = (await step2Page2.ClickOnElement(step2Page2.ButtonContinue)).CurrentPageAs<Step3InputMoveInPropertyDetailsPage>().InputFormValues(UserConfig); ;
			await step3InputPage.ClickOnElement(step3InputPage.NextPaymentOptions);


			var choosePaymentOptionsPage = App.CurrentPageAs<ChoosePaymentOptionsPage>();

			var inputDirectDebitDetailsPage2 = await Step4InputFormValues(choosePaymentOptionsPage);
			var step5Page = (await inputDirectDebitDetailsPage2.ClickOnElement(inputDirectDebitDetailsPage2.CompleteDirectDebitSetup))
				.CurrentPageAs<Step5ReviewAndCompletePage>();

			var accountConfigurators = UserConfig.ElectricityAndGasAccountConfigurators.ToArray();

			var electricityDevicesMeterReading = accountConfigurators[1].Premise.ElectricityDevice().Registers.Single();
			var gasDevicesMeterReading = accountConfigurators[0].Premise.GasDevice().Registers.Single();

			Assert.AreEqual(step1InputPage.GetElecReadingInput(electricityDevicesMeterReading.MeterNumber).Value,
				step5Page.ShowMovingDatesAndMeterReadings.MoveOutElectricityMeterReadingValue.TextContent);

			Assert.AreEqual(step1InputPage.GetGasReadingInput(gasDevicesMeterReading.MeterNumber).Value,
				step5Page.ShowMovingDatesAndMeterReadings.MoveOutGasMeterReadingValue.TextContent);
			electricityDevicesMeterReading = accountConfigurators[1].NewPremise.ElectricityDevice().Registers.Single();
			Assert.AreEqual(step3InputPage.GetElectricityReadingInput(electricityDevicesMeterReading.MeterNumber).Value,
				step5Page.ShowMovingDatesAndMeterReadings.MoveInElectricityMeterReadingValue.TextContent);
		}

		[Test]
		public virtual async Task Ste5ReviewPage_ClickOnEdit_Payments_Then_Choose_Manual()
		{
			var inputDirectDebitDetailsPage = await Step4InputFormValues(Sut);

			var step5 = (await inputDirectDebitDetailsPage.ClickOnElement(inputDirectDebitDetailsPage.CompleteDirectDebitSetup))
				.CurrentPageAs<Step5ReviewAndCompletePage>();
			AssertStep5Review(step5);

			var choosePaymentOptionsPage = (await step5.ClickOnElement(step5.ShowPayments.PrimaryPaymentEdit))
				.CurrentPageAs<ChoosePaymentOptionsPage>();

			await choosePaymentOptionsPage.SelectManualPayment();
			var step5Page = App.CurrentPageAs<Step5ReviewAndCompletePage>();

			Assert.IsTrue(step5Page.ShowPayments.PrimaryAccountType.TextContent.Contains(ClientAccountType.Electricity));
			Assert.IsTrue(step5Page.ShowPayments.PrimaryPaymentType.TextContent.Contains("Manual"));
			Assert.IsNotNull(step5Page.ShowPayments.PrimaryPaymentEdit);
		}

		[Test]
		public async Task Ste5ReviewPage_ClickOnEdit_Payments_Then_Choose_OtherDirectDebitDetails()
		{
			var inputDirectDebitDetailsPage = await Step4InputFormValues(Sut);

			var step5 = (await inputDirectDebitDetailsPage.ClickOnElement(inputDirectDebitDetailsPage.CompleteDirectDebitSetup))
				.CurrentPageAs<Step5ReviewAndCompletePage>();
			AssertStep5Review(step5);

			var choosePaymentOptionsPage = (await step5.ClickOnElement(step5.ShowPayments.PrimaryPaymentEdit))
				.CurrentPageAs<ChoosePaymentOptionsPage>();

			var inputDirectDebitDetailsPage2 = await Step4InputFormValues(choosePaymentOptionsPage, "IE65AIBK93104715784037");
			var step5Page = (await inputDirectDebitDetailsPage2.ClickOnElement(inputDirectDebitDetailsPage2.CompleteDirectDebitSetup))
				.CurrentPageAs<Step5ReviewAndCompletePage>();
			AssertStep5Review(step5Page);

			Assert.IsTrue(step5Page.ShowPayments.PrimaryAccountType.TextContent.Contains(ClientAccountType.Electricity));
			Assert.IsTrue(step5Page.ShowPayments.PrimaryPaymentType.TextContent.Contains("Direct Debit"));
			Assert.IsTrue(step5Page.ShowPayments.PrimaryPaymentType.TextContent.Contains("4037"));
			Assert.IsNotNull(step5Page.ShowPayments.PrimaryPaymentEdit);
		}

		[Test]
		public async Task Ste5ReviewPage_ClickOnMoveIn_Edit_MeterReadings()
		{
			var inputDirectDebitDetailsPage = await Step4InputFormValues(Sut);

			var step5 = (await inputDirectDebitDetailsPage.ClickOnElement(inputDirectDebitDetailsPage.CompleteDirectDebitSetup))
				.CurrentPageAs<Step5ReviewAndCompletePage>();
			AssertStep5Review(step5);
			var step3InputPage = (await step5.ClickOnElement(step5.ShowMovingDatesAndMeterReadings.EditMoveInDetailsButton))
				.CurrentPageAs<Step3InputMoveInPropertyDetailsPage>();
			Assert.AreEqual(step3Page.MoveOutDatePicker.Value, step3InputPage.MoveOutDatePicker.Value);
			step3InputPage.InputFormValues(UserConfig, "4567");
			await step3InputPage.ClickOnElement(step3InputPage.NextPaymentOptions);

			var choosePaymentOptionsPage = App.CurrentPageAs<ChoosePaymentOptionsPage>();

			var inputDirectDebitDetailsPage2 = await Step4InputFormValues(choosePaymentOptionsPage);
			var step5Page = (await inputDirectDebitDetailsPage2.ClickOnElement(inputDirectDebitDetailsPage2.CompleteDirectDebitSetup))
				.CurrentPageAs<Step5ReviewAndCompletePage>();
			var accountConfigurators = UserConfig.ElectricityAndGasAccountConfigurators.ToArray();

			var electricityDevicesMeterReading = accountConfigurators[1].NewPremise.ElectricityDevice().Registers.Single();

			Assert.AreEqual(step3InputPage.GetElectricityReadingInput(electricityDevicesMeterReading.MeterNumber).Value,
				step5Page.ShowMovingDatesAndMeterReadings.MoveInElectricityMeterReadingValue.TextContent);

		}

		[Test]
		public async Task Ste5ReviewPage_Click_StartOver()
		{
			var inputDirectDebitDetailsPage = await Step4InputFormValues(Sut);

			var step5 = (await inputDirectDebitDetailsPage.ClickOnElement(inputDirectDebitDetailsPage.CompleteDirectDebitSetup))
				.CurrentPageAs<Step5ReviewAndCompletePage>();
			AssertStep5Review(step5);
			var landingPage = (await step5.ClickOnElement(step5.ShowAccountDetails.StartOverButton))
				.CurrentPageAs<Step0LandingPage>();
		}

		protected async Task<InputDirectDebitDetailsWhenMovingHomePage> Step4InputFormValues(ChoosePaymentOptionsPage page, string newIBAN = null)
		{
			const string iban = "IE29AIBK93115212345678";
			if (string.IsNullOrEmpty(newIBAN))
			{
				newIBAN = iban;
			}

			const string ibanWithNonNumeric = "IE29AIBK93115212345678#";
			const string badCustomerBankName = "NotAR34lN4me!";

			var inputDirectDebitDetailsPage = (await page.SelectNewDirectDebit()).CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();


			Assert.IsEmpty(inputDirectDebitDetailsPage.IbanErrorMessage.TextContent);
			Assert.IsEmpty(inputDirectDebitDetailsPage.CustomerNameErrorMessage.TextContent);
			inputDirectDebitDetailsPage.Iban.Value = ibanWithNonNumeric;
			inputDirectDebitDetailsPage.CustomerName.Value = badCustomerBankName;
			inputDirectDebitDetailsPage = (await inputDirectDebitDetailsPage.Complete()).CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			Assert.IsNotEmpty(inputDirectDebitDetailsPage.CustomerNameErrorMessage.TextContent);
			Assert.IsNotEmpty(inputDirectDebitDetailsPage.IbanErrorMessage.TextContent);

			inputDirectDebitDetailsPage.Iban.Value = newIBAN;
			inputDirectDebitDetailsPage.CustomerName.Value = "Ulster Bank";
			inputDirectDebitDetailsPage.TermsAndConditions.IsChecked = true;
			return inputDirectDebitDetailsPage;
		}

		protected void AssertStep5Review(Step5ReviewAndCompletePage reviewPage)
		{
			var electricityAccountInfo = UserConfig.Accounts.ElementAt(1);
			var gasAccountInfo = UserConfig.Accounts.ElementAt(0);
			Assert.AreEqual("Review your details", reviewPage.ReviewDetailsHeader.TextContent);
			Assert.IsTrue(reviewPage.ReviewDetailsContent.TextContent.Contains("You're almost done, please review and confirm the details of your move"));

			//account info
			Assert.AreEqual("Account Details", reviewPage.ShowAccountDetails.AccountDetailsHeader.TextContent);
			Assert.IsNotNull(reviewPage.ShowAccountDetails.AccountDetailsPrimaryAccountType);
			Assert.IsNotNull(reviewPage.ShowAccountDetails.AccountDetailsPrimaryAccountNumber);
			Assert.IsTrue(reviewPage.ShowAccountDetails.AccountDetailsPrimaryAccountType.TextContent.Contains(ClientAccountType.Electricity));
			Assert.IsTrue(reviewPage.ShowAccountDetails.AccountDetailsPrimaryAccountNumber.TextContent.Contains(electricityAccountInfo.AccountNumber));
			Assert.IsTrue(reviewPage.ShowAccountDetails.AccountDetailsGasAccountType.TextContent.Contains(ClientAccountType.Gas));
			Assert.IsTrue(reviewPage.ShowAccountDetails.AccountDetailsGasAccountNumber.TextContent.Contains(gasAccountInfo.AccountNumber));

			Assert.IsNotNull(reviewPage.ShowAccountDetails.StartOverButton);
			Assert.IsTrue(reviewPage.ShowAccountDetails.StartOverButton.TextContent.Contains("Start Over"));
			//PropertyDetails
			Assert.AreEqual("Previous Property", reviewPage.ShowPropertyDetails.PreviousPropertyHeader.TextContent);
			Assert.IsNotNull(reviewPage.ShowPropertyDetails.PreviousAddressInfo);

			Assert.AreEqual("New Property", reviewPage.ShowPropertyDetails.NewPropertyHeader.TextContent);
			Assert.IsNotNull(reviewPage.ShowPropertyDetails.NewAddressInfo);
			Assert.IsNotNull(reviewPage.ShowPropertyDetails.EditNewAddressButton);

			//Moving dates and meter readings
			Assert.AreEqual("Moving Dates and Meter Readings", reviewPage.ShowMovingDatesAndMeterReadings.MovingDateHeader.TextContent);
			Assert.IsTrue(reviewPage.ShowMovingDatesAndMeterReadings.MoveOutDateTitle.TextContent.Contains("Move out date:"));

			DateTime moveOutDate = DateTime.Parse(step1Page.MoveOutDatePicker.Value);
			Assert.AreEqual(moveOutDate.ToDisplayDate(), reviewPage.ShowMovingDatesAndMeterReadings.MoveOutDate.TextContent);

			var accountConfigurators = UserConfig.ElectricityAndGasAccountConfigurators.ToArray();


			var electricityDevicesMeterReading = accountConfigurators[1].Premise.ElectricityDevice().Registers.Single();
			var gasDevicesMeterReading = accountConfigurators[0].Premise.GasDevice().Registers.Single();

			Assert.AreEqual(step1Page.GetElecReadingInput(electricityDevicesMeterReading.MeterNumber).Value,
				reviewPage.ShowMovingDatesAndMeterReadings.MoveOutElectricityMeterReadingValue.TextContent);

			Assert.AreEqual(step1Page.GetGasReadingInput(gasDevicesMeterReading.MeterNumber).Value,
				reviewPage.ShowMovingDatesAndMeterReadings.MoveOutGasMeterReadingValue.TextContent);

			electricityDevicesMeterReading = accountConfigurators[1].NewPremise.ElectricityDevice().Registers.Single();
			Assert.AreEqual(step3Page.GetElectricityReadingInput(electricityDevicesMeterReading.MeterNumber).Value,
				reviewPage.ShowMovingDatesAndMeterReadings.MoveInElectricityMeterReadingValue.TextContent);

			Assert.IsNull(reviewPage.ShowMovingDatesAndMeterReadings.MoveInGasMeterReadingValue);
			Assert.IsNotNull(reviewPage.ShowMovingDatesAndMeterReadings.EditMoveOutDetailsButton);
			Assert.IsNotNull(reviewPage.ShowMovingDatesAndMeterReadings.EditMoveInDetailsButton);

			//payments
			Assert.AreEqual("Payment Method", reviewPage.ShowPayments.PaymentHeader.TextContent);

			Assert.IsTrue(reviewPage.ShowPayments.PrimaryAccountType.TextContent.Contains(ClientAccountType.Electricity));
			Assert.IsNotNull(reviewPage.ShowPayments.PrimaryPaymentEdit);
		}

	}
}
