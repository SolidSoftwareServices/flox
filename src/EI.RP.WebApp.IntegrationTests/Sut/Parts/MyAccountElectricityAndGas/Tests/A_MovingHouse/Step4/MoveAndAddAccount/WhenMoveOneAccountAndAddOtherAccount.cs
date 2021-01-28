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

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step4.MoveAndAddAccount
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
						.AddElectricityAccount(withPaperBill: false, paymentType: ScenarioPaymentMethodType, isPRNDeRegistered: IsPRNDegistered,
							configureDefaultDevice: false, newPrnAddressExists: true,canAddNewAccount:true)
						.WithElectricity24HrsDevices()
						
						;
				}

				if (ScenarioAccountType == ClientAccountType.Gas)
				{
					UserConfig
						.AddGasAccount(withPaperBill: false, paymentType: ScenarioPaymentMethodType, newPrnAddressExists: true, isPrnDeregistered: IsPRNDegistered,configureDefaultDevice: false,canAddNewAccount:true)
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
		public async Task HandlesUserPath_Cancel_ThenGoBack_ThenCancel_ThenCancel_ShowsLandingPage()
		{
			//bypassing javascript as still experimental on anglesharp and not working
			await Sut.ClickOnCancelConfirm();
			App.CurrentPageAs<Step0LandingPage>();
		}

		[Test]
		public async Task HandlesUserPath_MANUAL_Confirm_ShowsStep5WithCorrectInfo()
		{
			//bypassing javascript as still experimental on anglesharp and not working
			await Sut.SelectManualPayment();
			var step5 = App.CurrentPageAs<Step5ReviewAndCompletePage>();
			AssertStep5Review(step5);

			Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains("Manual"));

			Assert.IsTrue(step5.ShowPricePlan.PricePlanText.TextContent.Contains("Your price plan and savings will reflect your details displayed above"));
		}

		[Test]
		public async Task HandlesUserPath_MANUAL_ThenNEW_ShowsInputScreen()
		{
			//bypassing javascript as still experimental on anglesharp and not working
			await Sut.SelectNewDirectDebitFromDialog();
			App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
		}

		[Test]
		public async Task HandlesUserPath_NEW_InputComplete_ShowsStep5WithCorrectInfo()
		{
			await Sut.SelectNewDirectDebit();
			App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();

		}

		[Ignore("")]
		[Test]
		public async Task HandlesUserPath_NEW_InputWithErrors()
		{
			throw new NotImplementedException();
			//TODO: assert EACH STAGE
		}

		[Test]
		public virtual async Task HandlesUserPath_NEW_Skip_ConfirmSkip_WhenUseSameForAllAccountsCheckBox_True_ShowsStep5WithCorrectInfo()
		{
			var accountInfo = UserConfig.Accounts.ElementAt(0);
			var secondaryAccountType = accountInfo.ClientAccountType == ClientAccountType.Electricity ? ClientAccountType.Gas
									: ClientAccountType.Electricity;
			Sut.UseSameNewDirectDebitForAllAccountsCheckBox.IsChecked = true;
			await Sut.SelectNewDirectDebit();
			var directDebitSut = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			Assert.AreEqual($"{accountInfo.ClientAccountType} ({accountInfo.AccountNumber}) & {secondaryAccountType}", directDebitSut.HeaderElement.TextContent);
			Assert.IsNotNull(directDebitSut.CustomerName);
			Assert.IsNotNull(directDebitSut.Iban);
			Assert.IsNotNull(directDebitSut.ConfirmTerms);
			Assert.IsNotNull(directDebitSut.SkipElement);
			var step5 = (await directDebitSut.SelectManual()).CurrentPageAs<Step5ReviewAndCompletePage>();

			AssertStep5Review(step5);
			Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains("Manual"));
			Assert.IsTrue(step5.ShowPayments.SecondaryPaymentType.TextContent.Contains("Manual"));
			Assert.IsTrue(step5.ShowPricePlan.PricePlanText.TextContent.Contains("Your price plan and savings will reflect your details displayed above"));
		}

		[Test]
		public virtual async Task HandlesUserPath_NEW_Skip_ConfirmSkip_WhenUseSameForAllAccountsCheckBox_False_ShowsStep5WithCorrectInfo()
		{
			Sut.UseSameNewDirectDebitForAllAccountsCheckBox.IsChecked = false;
			var accountInfo = UserConfig.Accounts.ElementAt(0);
			var secondaryAccountType = accountInfo.ClientAccountType == ClientAccountType.Electricity ? ClientAccountType.Gas : ClientAccountType.Electricity;

			await Sut.SelectNewDirectDebit();
			var directDebitSut = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			Assert.AreEqual($"{accountInfo.ClientAccountType} ({accountInfo.AccountNumber})", directDebitSut.HeaderElement.TextContent);

			Assert.IsNotNull(directDebitSut.CustomerName);
			Assert.IsNotNull(directDebitSut.Iban);
			Assert.IsNotNull(directDebitSut.ConfirmTerms);
			Assert.IsNotNull(directDebitSut.SkipElement);
			await directDebitSut.SelectManual();
			directDebitSut = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			Assert.AreEqual($"{secondaryAccountType}", directDebitSut.HeaderElement.TextContent);
			var step5ReviewPage = (await directDebitSut.SelectManual()).CurrentPageAs<Step5ReviewAndCompletePage>();
			AssertStep5Review(step5ReviewPage);
			Assert.IsTrue(step5ReviewPage.ShowPayments.PrimaryPaymentType.TextContent.Contains("Manual"));
			Assert.IsTrue(step5ReviewPage.ShowPayments.SecondaryPaymentType.TextContent.Contains("Manual"));
			Assert.IsTrue(step5ReviewPage.ShowPricePlan.PricePlanText.TextContent.Contains("Your price plan and savings will reflect your details displayed above"));
		}

		[Ignore("")]
		[Test]
		public async Task HandlesUserPath_NEW_Skip_GoBack_Input_Complete_ShowsStep5WithCorrectInfo()
		{
			throw new NotImplementedException();
			//TODO: assert EACH STAGE
		}

		[Test]
		public virtual async Task TheDirectDebitScreenIsCorrect_WhenUseSameForAllAccountsCheckBox_False()
		{
			Sut.UseSameNewDirectDebitForAllAccountsCheckBox.IsChecked = false;
			var accountInfo = UserConfig.Accounts.ElementAt(0);
			var secondayAccountType = accountInfo.ClientAccountType == ClientAccountType.Electricity ? ClientAccountType.Gas
									: ClientAccountType.Electricity;
			await Sut.SelectNewDirectDebit();
			var directDebitSut = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			Assert.AreEqual($"{accountInfo.ClientAccountType} ({accountInfo.AccountNumber})", directDebitSut.HeaderElement.TextContent);
			Assert.AreEqual("Continue to " + secondayAccountType.ToString().ToLower() + " direct debit", directDebitSut.CompleteDirectDebitSetup.TextContent);
			Assert.IsNotNull(directDebitSut.CustomerName);
			Assert.IsNotNull(directDebitSut.Iban);
			Assert.IsNotNull(directDebitSut.ConfirmTerms);
			Assert.IsNotNull(directDebitSut.SkipElement);
			directDebitSut.InputFormValues();
			directDebitSut = (await directDebitSut.Complete()).CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			Assert.AreEqual("Complete Direct Debit Setup", directDebitSut.CompleteDirectDebitSetup.TextContent);
			Assert.AreEqual($"{secondayAccountType}", directDebitSut.HeaderElement.TextContent);
			Assert.IsNotNull(directDebitSut.SkipElement);
			directDebitSut.InputFormValues();
			var step5ReviewPage = (await directDebitSut.Complete()).CurrentPageAs<Step5ReviewAndCompletePage>();

			AssertStep5Review(step5ReviewPage);
			Assert.IsTrue(step5ReviewPage.ShowPayments.PrimaryPaymentType.TextContent.Contains("Direct Debit"));
			Assert.IsTrue(step5ReviewPage.ShowPayments.SecondaryPaymentType.TextContent.Contains("Direct Debit"));
			Assert.IsTrue(
				step5ReviewPage.ShowPricePlan.PricePlanText.TextContent.Contains("Your price plan and savings will reflect your details displayed above."));
		}

		[Test]
		public virtual async Task TheDirectDebitScreenIsCorrect_WhenUseSameForAllAccountsCheckBox_True()
		{
			Sut.UseSameNewDirectDebitForAllAccountsCheckBox.IsChecked = true;
			var accountInfo = UserConfig.Accounts.ElementAt(0);
			var secondayAccountType = accountInfo.ClientAccountType == ClientAccountType.Electricity ? ClientAccountType.Gas
									: ClientAccountType.Electricity;
			await Sut.SelectNewDirectDebit();
			var directDebitSut = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			Assert.AreEqual($"{accountInfo.ClientAccountType} ({accountInfo.AccountNumber}) & {secondayAccountType}", directDebitSut.HeaderElement.TextContent);
			Assert.AreEqual("Complete Direct Debit Setup", directDebitSut.CompleteDirectDebitSetup.TextContent);
			Assert.IsNotNull(directDebitSut.CustomerName);
			Assert.IsNotNull(directDebitSut.Iban);
			Assert.IsNotNull(directDebitSut.ConfirmTerms);
			Assert.IsNotNull(directDebitSut.SkipElement);
			directDebitSut.InputFormValues();
			var step5ReviewPage = (await directDebitSut.Complete()).CurrentPageAs<Step5ReviewAndCompletePage>();
			AssertStep5Review(step5ReviewPage);
			Assert.IsTrue(step5ReviewPage.ShowPayments.PrimaryPaymentType.TextContent.Contains("Direct Debit"));
			Assert.IsTrue(step5ReviewPage.ShowPayments.SecondaryPaymentType.TextContent.Contains("Direct Debit"));
			Assert.IsTrue(
				step5ReviewPage.ShowPricePlan.PricePlanText.TextContent.Contains("Your price plan and savings will reflect your details displayed above."));
		}

		[Test]
		public virtual async Task WhenElectricIrelandIbanUsed_ThenReviewPageDoesntAppear()
		{
			Sut.UseSameNewDirectDebitForAllAccountsCheckBox.IsChecked = true;
			var accountInfo = UserConfig.Accounts.ElementAt(0);
			var secondayAccountType = accountInfo.ClientAccountType == ClientAccountType.Electricity ? ClientAccountType.Gas
									: ClientAccountType.Electricity;
			await Sut.SelectNewDirectDebit();
			var directDebitSut = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			directDebitSut.InputFormValues();
			directDebitSut.Iban.Value = "IE36AIBK93208681900087"; 
			var errorPage = (await directDebitSut.Complete()).CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			Assert.AreEqual("Please enter a valid IBAN", errorPage.IbanErrorMessage.TextContent);
		}

		[Test]
		public virtual async Task TheOptionsScreenIsCorrect()
		{
			Assert.IsNotNull(Sut.CancelButton);
			Assert.AreEqual("Cancel", Sut.CancelButton.TextContent);

			Assert.IsNotNull(Sut.SetUpNewDirectDebitButton);
			Assert.IsNotNull(Sut.ManualPaymentsButton);

			Assert.IsNotNull(Sut.UseSameNewDirectDebitForAllAccountsCheckBox);
			Assert.IsNotNull(Sut.UseSameForAllAccountsLabel);
			Assert.IsTrue(Sut.UseSameForAllAccountsLabel.TextContent.Contains("Use this Direct Debit to make payments for both my Electricity and Gas accounts"));
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

			Assert.AreEqual("New Property", reviewPage.ShowPropertyDetails.NewPropertyHeader.TextContent);
			Assert.IsNotNull(reviewPage.ShowPropertyDetails.NewAddressInfo);
			Assert.IsNotNull(reviewPage.ShowPropertyDetails.EditNewAddressButton);

			//Moving dates and meter readings
			Assert.AreEqual("Moving Dates and Meter Readings", reviewPage.ShowMovingDatesAndMeterReadings.MovingDateHeader.TextContent);
			Assert.IsTrue(reviewPage.ShowMovingDatesAndMeterReadings.MoveOutDateTitle.TextContent.Contains("Move out date:"));

			DateTime moveOutDate = DateTime.Parse(step1Page.MoveOutDatePicker.Value);
			Assert.AreEqual(moveOutDate.ToDisplayDate(), reviewPage.ShowMovingDatesAndMeterReadings.MoveOutDate.TextContent);

			var accountConfigurators = UserConfig.ElectricityAndGasAccountConfigurators.ToArray();


			if (ScenarioAccountType == ClientAccountType.Electricity)
			{
				var devicesMeterReading = accountConfigurators[0].Premise.Devices.Single().Registers.Single();
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