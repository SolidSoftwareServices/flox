using System;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;
using AngleSharp.Dom;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step5.MoveAndAddAccount
{
	[TestFixture]
	internal abstract class WhenMoveOneAccountAndAddOtherAccount : MyAccountCommonTests<ChoosePaymentOptionsPage>
	{
		protected abstract ClientAccountType ScenarioAccountType { get; }

		protected abstract PaymentMethodType ScenarioPaymentMethodType { get; }

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

				if (ScenarioAccountType == ClientAccountType.Electricity)
				{
					UserConfig
						.AddElectricityAccount(withPaperBill: false, paymentType: ScenarioPaymentMethodType, isPRNDeRegistered: IsPRNDegistered, configureDefaultDevice: false, canAddNewAccount: true)
						.WithElectricity24HrsDevices()
						;
				}

				if (ScenarioAccountType == ClientAccountType.Gas)
				{
					UserConfig
						.AddGasAccount(withPaperBill: false, paymentType: ScenarioPaymentMethodType, newPrnAddressExists: true, isPrnDeregistered: IsPRNDegistered, configureDefaultDevice: false,canAddNewAccount:true)
						.WithGasDevice()
						;
				}

				UserConfig.Execute();
			}

			async Task<ChoosePaymentOptionsPage> ResolveScenarioSut()
			{
                await ((ResidentialPortalApp) await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
				var movingHomeLandingPage = (await App.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMovingHome())
					.CurrentPageAs<Step0LandingPage>();

				step1Page =
				   (await movingHomeLandingPage.ClickOnElement(movingHomeLandingPage.PopupButton2))
				   .CurrentPageAs<Step1InputMoveOutPage>()
				   .InputFormValues(UserConfig);

				
				var step2Page1 = (await step1Page.ClickOnElement(step1Page.GetNextPRNButton()))
					.CurrentPageAs<Step2InputPrnsPage>();

				step2Page1.InputFormValues(UserConfig);


				var step2Page2 = (await step2Page1.ClickOnElement(step2Page1.SubmitPRNS)).CurrentPageAs<Step2ConfirmAddressPage>();
				step3Page = (await step2Page2.ClickOnElement(step2Page2.ButtonContinue)).CurrentPageAs<Step3InputMoveInPropertyDetailsPage>().InputFormValues(UserConfig);
				await step3Page.ClickOnElement(step3Page.NextPaymentOptions);

				var choosePaymentOptionsPage = App.CurrentPageAs<ChoosePaymentOptionsPage>();
				choosePaymentOptionsPage = await AssertTermsAndConditionsValidationIfNecessary(choosePaymentOptionsPage);
				return choosePaymentOptionsPage;
			}

			async Task<ChoosePaymentOptionsPage> AssertTermsAndConditionsValidationIfNecessary(ChoosePaymentOptionsPage choosePaymentOptionsPage)
			{
				if (ScenarioPaymentMethodType == PaymentMethodType.DirectDebit)
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

			if (ScenarioPaymentMethodType == PaymentMethodType.DirectDebit)
			{
				choosePaymentOptionsPage = (await choosePaymentOptionsPage.SelectExistingDirectDebit()).CurrentPageAs<ChoosePaymentOptionsPage>();
				Assert.IsTrue(
					choosePaymentOptionsPage
						.UseExistingDirectDebitConfirmationNotCheckedErrorMessage
						.TextContent
						.Contains("Please confirm that you are authorised to provide Electric Ireland with this information"));
			}

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
			var step3Page = (await step2Page2.ClickOnElement(step2Page2.ButtonContinue)).CurrentPageAs<Step3InputMoveInPropertyDetailsPage>().InputFormValues(UserConfig); ;
			await step3Page.ClickOnElement(step3Page.NextPaymentOptions);


			var choosePaymentOptionsPage = App.CurrentPageAs<ChoosePaymentOptionsPage>();

			if (ScenarioPaymentMethodType == PaymentMethodType.DirectDebit)
			{
				choosePaymentOptionsPage = (await choosePaymentOptionsPage.SelectExistingDirectDebit()).CurrentPageAs<ChoosePaymentOptionsPage>();
				Assert.IsTrue(
					choosePaymentOptionsPage
						.UseExistingDirectDebitConfirmationNotCheckedErrorMessage
						.TextContent
						.Contains("Please confirm that you are authorised to provide Electric Ireland with this information"));
			}

			var inputDirectDebitDetailsPage2 = await Step4InputFormValues(choosePaymentOptionsPage);
			var step5Page = (await inputDirectDebitDetailsPage2.ClickOnElement(inputDirectDebitDetailsPage2.CompleteDirectDebitSetup))
				.CurrentPageAs<Step5ReviewAndCompletePage>();
			var accountConfigurators = UserConfig.ElectricityAndGasAccountConfigurators.ToArray();

			if (ScenarioAccountType == ClientAccountType.Electricity)
			{
				var devicesMeterReading = accountConfigurators[0].Premise.ElectricityDevice().Registers.Single();
				Assert.AreEqual(step1InputPage.GetElecReadingInput(devicesMeterReading.MeterNumber).Value,
					step5Page.ShowMovingDatesAndMeterReadings.MoveOutElectricityMeterReadingValue.TextContent);
			}
			else
			{
				var devicesMeterReading = accountConfigurators[0].Premise.GasDevice().Registers.Single();
				Assert.AreEqual(step1InputPage.GetGasReadingInput(devicesMeterReading.MeterNumber).Value,
					step5Page.ShowMovingDatesAndMeterReadings.MoveOutGasMeterReadingValue.TextContent);
			}
		}

        [Test]
        public async Task Ste5ReviewPage_ClickOnMoveOutEdit_ChangeMoveOutDateToBack()
        {
            var inputDirectDebitDetailsPage = await Step4InputFormValues(Sut);

            var step5 = (await inputDirectDebitDetailsPage.ClickOnElement(inputDirectDebitDetailsPage.CompleteDirectDebitSetup))
                .CurrentPageAs<Step5ReviewAndCompletePage>();
            AssertStep5Review(step5);

            var step1InputPage = (await step5.ClickOnElement(step5.ShowMovingDatesAndMeterReadings.EditMoveOutDetailsButton))
                .CurrentPageAs<Step1InputMoveOutPage>();

            Assert.AreEqual(step1Page.MoveOutDatePicker.Value, step1InputPage.MoveOutDatePicker.Value);

            var changedMoveOutDate = DateTime.Today.Subtract(TimeSpan.FromDays(7.0));
            step1InputPage.MoveOutDatePicker.Value = changedMoveOutDate.ToShortDateString();

            var step2Page1 = (await step1InputPage.ClickOnElement(step1InputPage.GetNextPRNButton()))
                .CurrentPageAs<Step2InputPrnsPage>();
            var step2Page2 = (await step2Page1.ClickOnElement(step2Page1.SubmitPRNS)).CurrentPageAs<Step2ConfirmAddressPage>();
            var step3Page = (await step2Page2.ClickOnElement(step2Page2.ButtonContinue)).CurrentPageAs<Step3InputMoveInPropertyDetailsPage>(); 

            var expectedMoveInDate = changedMoveOutDate.AddDays(1);
            Assert.AreEqual(step3Page.MoveOutDatePicker.Value, expectedMoveInDate.ToShortDateString());
        }

        [Test]
        public async Task Ste5ReviewPage_ClickOnMoveOutEdit_ChangeMoveOutDateToFuture()
        {
            var inputDirectDebitDetailsPage = await Step4InputFormValues(Sut);

            var step5 = (await inputDirectDebitDetailsPage.ClickOnElement(inputDirectDebitDetailsPage.CompleteDirectDebitSetup))
                .CurrentPageAs<Step5ReviewAndCompletePage>();
            AssertStep5Review(step5);

            var step1InputPage = (await step5.ClickOnElement(step5.ShowMovingDatesAndMeterReadings.EditMoveOutDetailsButton))
                .CurrentPageAs<Step1InputMoveOutPage>();

            Assert.AreEqual(step1Page.MoveOutDatePicker.Value, step1InputPage.MoveOutDatePicker.Value);

            var changedMoveOutDate = DateTime.Today.Add(TimeSpan.FromDays(7.0));
            step1InputPage.MoveOutDatePicker.Value = changedMoveOutDate.ToShortDateString();

            var step2Page1 = (await step1InputPage.ClickOnElement(step1InputPage.GetNextPRNButton()))
                .CurrentPageAs<Step2InputPrnsPage>();
            var step2Page2 = (await step2Page1.ClickOnElement(step2Page1.SubmitPRNS)).CurrentPageAs<Step2ConfirmAddressPage>();
            var step3Page = (await step2Page2.ClickOnElement(step2Page2.ButtonContinue)).CurrentPageAs<Step3InputMoveInPropertyDetailsPage>();

            var expectedMoveInDate = changedMoveOutDate.AddDays(1);
            Assert.AreEqual(step3Page.MoveOutDatePicker.Value, expectedMoveInDate.ToShortDateString());
        }

        [Test]
		public async Task Ste5ReviewPage_ClickOnEdit_Payments_Then_Choose_Manual()
		{
			var inputDirectDebitDetailsPage = await Step4InputFormValues(Sut);

			var step5 = (await inputDirectDebitDetailsPage.ClickOnElement(inputDirectDebitDetailsPage.CompleteDirectDebitSetup))
				.CurrentPageAs<Step5ReviewAndCompletePage>();
			AssertStep5Review(step5);

			var choosePaymentOptionsPage = (await step5.ClickOnElement(step5.ShowPayments.PrimaryPaymentEdit))
				.CurrentPageAs<ChoosePaymentOptionsPage>();

			if (ScenarioPaymentMethodType == PaymentMethodType.DirectDebit)
			{
				choosePaymentOptionsPage = (await choosePaymentOptionsPage.SelectExistingDirectDebit()).CurrentPageAs<ChoosePaymentOptionsPage>();
				Assert.IsTrue(
					choosePaymentOptionsPage
						.UseExistingDirectDebitConfirmationNotCheckedErrorMessage
						.TextContent
						.Contains("Please confirm that you are authorised to provide Electric Ireland with this information"));
			}

			await choosePaymentOptionsPage.SelectManualPayment();
			var step5Page = App.CurrentPageAs<Step5ReviewAndCompletePage>();

			Assert.IsTrue(step5Page.ShowPayments.PrimaryAccountType.TextContent.Contains(ClientAccountType.Electricity));
			Assert.IsTrue(step5Page.ShowPayments.PrimaryPaymentType.TextContent.Contains("Manual"));
			Assert.IsNotNull(step5Page.ShowPayments.PrimaryPaymentEdit);
			Assert.IsTrue(step5Page.ShowPayments.SecondaryAccountType.TextContent.Contains(ClientAccountType.Gas));
			Assert.IsTrue(step5Page.ShowPayments.SecondaryPaymentType.TextContent.Contains("Manual"));
			Assert.IsNotNull(step5Page.ShowPayments.SecondaryPaymentEdit);
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

			if (ScenarioPaymentMethodType == PaymentMethodType.DirectDebit)
			{
				choosePaymentOptionsPage = (await choosePaymentOptionsPage.SelectExistingDirectDebit()).CurrentPageAs<ChoosePaymentOptionsPage>();
				Assert.IsTrue(
					choosePaymentOptionsPage
						.UseExistingDirectDebitConfirmationNotCheckedErrorMessage
						.TextContent
						.Contains("Please confirm that you are authorised to provide Electric Ireland with this information"));
			}

			var inputDirectDebitDetailsPage2 = await Step4InputFormValues(choosePaymentOptionsPage, "IE65AIBK93104715784037");
			var step5Page = (await inputDirectDebitDetailsPage2.ClickOnElement(inputDirectDebitDetailsPage2.CompleteDirectDebitSetup))
				.CurrentPageAs<Step5ReviewAndCompletePage>();
			AssertStep5Review(step5Page);

			Assert.IsTrue(step5Page.ShowPayments.PrimaryAccountType.TextContent.Contains(ClientAccountType.Electricity));
			Assert.IsTrue(step5Page.ShowPayments.PrimaryPaymentType.TextContent.Contains("Direct Debit"));
			Assert.IsNotNull(step5Page.ShowPayments.PrimaryPaymentEdit);
			Assert.IsTrue(step5Page.ShowPayments.SecondaryAccountType.TextContent.Contains(ClientAccountType.Gas));
			Assert.IsTrue(step5Page.ShowPayments.SecondaryPaymentType.TextContent.Contains("4037"));
			Assert.IsNotNull(step5Page.ShowPayments.SecondaryPaymentEdit);
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

			if (ScenarioPaymentMethodType == PaymentMethodType.DirectDebit)
			{
				choosePaymentOptionsPage = (await choosePaymentOptionsPage.SelectExistingDirectDebit()).CurrentPageAs<ChoosePaymentOptionsPage>();
				Assert.IsTrue(
					choosePaymentOptionsPage
						.UseExistingDirectDebitConfirmationNotCheckedErrorMessage
						.TextContent
						.Contains("Please confirm that you are authorised to provide Electric Ireland with this information"));
			}

			var inputDirectDebitDetailsPage2 = await Step4InputFormValues(choosePaymentOptionsPage);
			var step5Page = (await inputDirectDebitDetailsPage2.ClickOnElement(inputDirectDebitDetailsPage2.CompleteDirectDebitSetup))
				.CurrentPageAs<Step5ReviewAndCompletePage>();
			var accountConfigurators = UserConfig.ElectricityAndGasAccountConfigurators.ToArray();

			if (ScenarioAccountType == ClientAccountType.Electricity)
			{
				var devicesMeterReading = accountConfigurators[0].NewPremise.ElectricityDevice().Registers.Single();
				Assert.AreEqual(step3InputPage.GetElectricityReadingInput(devicesMeterReading.MeterNumber).Value,
					step5Page.ShowMovingDatesAndMeterReadings.MoveInElectricityMeterReadingValue.TextContent);
			}
			else
			{
				var devicesMeterReading = accountConfigurators[0].NewPremise.GasDevice().Registers.Single();
				Assert.AreEqual(step3InputPage.GetGasReadingInput(devicesMeterReading.MeterNumber).Value,
					step5Page.ShowMovingDatesAndMeterReadings.MoveInGasMeterReadingValue.TextContent);
			}
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
			var accountInfo = UserConfig.Accounts.ElementAt(0);
			Assert.AreEqual("Review your details", reviewPage.ReviewDetailsHeader.TextContent);
			Assert.IsTrue(reviewPage.ReviewDetailsContent.TextContent.Contains("You're almost done, please review and confirm the details of your move"));

			//account info
			Assert.AreEqual("Account Details", reviewPage.ShowAccountDetails.AccountDetailsHeader.TextContent);
			if (ScenarioAccountType == ClientAccountType.Electricity)
			{
				Assert.IsNotNull(reviewPage.ShowAccountDetails.AccountDetailsElectricityAccountType);
				Assert.IsNotNull(reviewPage.ShowAccountDetails.AccountDetailsElectricityAccountNumber);
				Assert.IsTrue(
					reviewPage.ShowAccountDetails.AccountDetailsElectricityAccountType.TextContent.Contains(
						ScenarioAccountType));
				Assert.IsTrue(
					reviewPage.ShowAccountDetails.AccountDetailsElectricityAccountNumber.TextContent.Contains(
						accountInfo.AccountNumber));

				Assert.AreEqual(reviewPage.ShowAccountDetails.AccountDetailsGasAccountType.TextContent, ClientAccountType.Gas.ToString());
				Assert.IsTrue(reviewPage.ShowAccountDetails.NewGasAccountText.TextContent.Contains("We are creating your new account and in the next four weeks you'll receive your account number"));
			}
			else
			{
				Assert.IsNotNull(reviewPage.ShowAccountDetails.AccountDetailsGasAccountType);
				Assert.IsNotNull(reviewPage.ShowAccountDetails.AccountDetailsGasAccountNumber);
				Assert.IsTrue(
					reviewPage.ShowAccountDetails.AccountDetailsGasAccountType.TextContent.Contains(
						ScenarioAccountType));
				Assert.IsTrue(
					reviewPage.ShowAccountDetails.AccountDetailsGasAccountNumber.TextContent.Contains(
						accountInfo.AccountNumber));

				Assert.AreEqual(reviewPage.ShowAccountDetails.AccountDetailsElectricityAccountType.TextContent, ClientAccountType.Electricity.ToString());
				Assert.IsTrue(reviewPage.ShowAccountDetails.NewElectricityAccountText.TextContent.Contains("We are creating your new account and in the next four weeks you'll receive your account number"));
			}

			//PropertyDetails
			Assert.AreEqual("Previous Property", reviewPage.ShowPropertyDetails.PreviousPropertyHeader.TextContent);
			Assert.IsNotNull(reviewPage.ShowPropertyDetails.PreviousAddressInfo);
            var previousAddress = reviewPage.ShowPropertyDetails.PreviousAddressInfo.TextContent.ToString();
            Assert.AreEqual(" ",
                previousAddress[(previousAddress.IndexOf("Street", StringComparison.Ordinal)) - 1].ToString());

            Assert.AreEqual("New Property", reviewPage.ShowPropertyDetails.NewPropertyHeader.TextContent);
			Assert.IsNotNull(reviewPage.ShowPropertyDetails.NewAddressInfo);
            var newAddress = reviewPage.ShowPropertyDetails.NewAddressInfo.TextContent.ToString();
            Assert.AreEqual(" ", newAddress[(newAddress.IndexOf("Street", StringComparison.Ordinal)) - 1].ToString());

            Assert.IsNotNull(reviewPage.ShowPropertyDetails.EditNewAddressButton);

			//Moving dates and meter readings
			Assert.AreEqual("Moving Dates and Meter Readings", reviewPage.ShowMovingDatesAndMeterReadings.MovingDateHeader.TextContent);
			Assert.IsTrue(reviewPage.ShowMovingDatesAndMeterReadings.MoveOutDateTitle.TextContent.Contains("Move out date:"));

			DateTime moveOutDate = DateTime.Parse(step1Page.MoveOutDatePicker.Value);
			Assert.AreEqual(moveOutDate.ToDisplayDate(), reviewPage.ShowMovingDatesAndMeterReadings.MoveOutDate.TextContent);

			var accountConfigurators = UserConfig.ElectricityAndGasAccountConfigurators.ToArray();


			if (ScenarioAccountType == ClientAccountType.Electricity)
			{
				var devicesMeterReading = accountConfigurators[0].Premise.ElectricityDevice().Registers.Single();
				Assert.AreEqual(step1Page.GetElecReadingInput(devicesMeterReading.MeterNumber).Value,
					reviewPage.ShowMovingDatesAndMeterReadings.MoveOutElectricityMeterReadingValue.TextContent);
				
				devicesMeterReading = accountConfigurators[0].NewPremise.ElectricityDevice().Registers.Single();
				Assert.AreEqual(step3Page.GetElectricityReadingInput(devicesMeterReading.MeterNumber).Value,
					reviewPage.ShowMovingDatesAndMeterReadings.MoveInElectricityMeterReadingValue.TextContent);

				devicesMeterReading = accountConfigurators[0].NewPremise.GasDevice().Registers.Single();
				Assert.AreEqual(step3Page.GetGasReadingInput(devicesMeterReading.MeterNumber).Value,
					reviewPage.ShowMovingDatesAndMeterReadings.MoveInGasMeterReadingValue.TextContent);
			}
			else
			{
				var devicesMeterReading = accountConfigurators[0].Premise.Devices.Single().Registers.Single();
				Assert.AreEqual(step1Page.GetGasReadingInput(devicesMeterReading.MeterNumber).Value,
					reviewPage.ShowMovingDatesAndMeterReadings.MoveOutGasMeterReadingValue.TextContent);
				devicesMeterReading = accountConfigurators[0].NewPremise.ElectricityDevice().Registers.Single();
				Assert.AreEqual(step3Page.GetElectricityReadingInput(devicesMeterReading.MeterNumber).Value,
					reviewPage.ShowMovingDatesAndMeterReadings.MoveInElectricityMeterReadingValue.TextContent);

				devicesMeterReading = accountConfigurators[0].NewPremise.GasDevice().Registers.Single();
				Assert.AreEqual(step3Page.GetGasReadingInput(devicesMeterReading.MeterNumber).Value,
					reviewPage.ShowMovingDatesAndMeterReadings.MoveInGasMeterReadingValue.TextContent);
			}

			//payments
			Assert.AreEqual("Payment Method", reviewPage.ShowPayments.PaymentHeader.TextContent);

			Assert.AreEqual("Electricity payment", reviewPage.ShowPayments.PrimaryAccountType.TextContent);
			Assert.IsNotNull(reviewPage.ShowPayments.PrimaryPaymentEdit);

			Assert.AreEqual("Gas payment", reviewPage.ShowPayments.SecondaryAccountType.TextContent);
			Assert.IsNotNull(reviewPage.ShowPayments.PrimaryPaymentEdit);

			//price plan
			Assert.AreEqual("Price Plan", reviewPage.ShowPricePlan.PricePlanHeader.TextContent);
			Assert.IsNotNull(reviewPage.ShowPricePlan.PricePlanText);
		}
	}
}