using System;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step4.MoveElectricityAndCloseGas
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

				var gasAccount = UserConfig.AddGasAccount(paymentType: ScenarioGasPaymentMethodType, newPrnAddressExists: true, isPrnDeregistered: IsPRNDegistered)
					;

				UserConfig.AddElectricityAccount(paymentType: ScenarioElectricityPaymentMethodType, duelFuelSisterAccount: gasAccount, isPRNDeRegistered: IsPRNDegistered, configureDefaultDevice: false)
					.WithElectricity24HrsDevices()
					;

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

		void AssertSkipDirectDebitLink(InputDirectDebitDetailsWhenMovingHomePage inputDirectDebitDetailsWhenMovingHomePage)
		{
			Assert.IsNotNull(inputDirectDebitDetailsWhenMovingHomePage.SkipDirectDebitSetupLink);
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
		}

		[Test]
		public async Task HandlesUserPath_MANUAL_ThenNEW_ShowsInputScreen()
		{
			//bypassing javascript as still experimental on anglesharp and not working
			await Sut.SelectNewDirectDebitFromDialog();
			App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
		}

		[Test]
		public virtual async Task HandlesUserPath_NEW_InputComplete_ShowsStep5WithCorrectInfo()
		{
			const string iban = "IE65AIBK93104715784037";

			var inputDirectDebitDetailsPage = (await Sut.SelectNewDirectDebit()).CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			inputDirectDebitDetailsPage.Iban.Value = iban;
			inputDirectDebitDetailsPage.CustomerName.Value = "Ulster Bank";
			inputDirectDebitDetailsPage.TermsAndConditions.IsChecked = true;

			AssertSkipDirectDebitLink(inputDirectDebitDetailsPage);
			var step5 = (await inputDirectDebitDetailsPage.ClickOnElement(inputDirectDebitDetailsPage.CompleteDirectDebitSetup))
				.CurrentPageAs<Step5ReviewAndCompletePage>();
			AssertStep5Review(step5);
			Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains("Direct Debit"));
			Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains(iban.Substring(iban.Length - 4)));
			if (IsPRNDegistered)
			{
				Assert.IsTrue(step5.ShowPricePlan.PricePlanText.TextContent.Contains("Your price plan and savings will reflect your details displayed above."));
			}
			else
			{
				Assert.IsTrue(step5.ShowPricePlan.PricePlanText.TextContent.Contains("Your price plan and savings will reflect your details displayed above"), step5.ShowPricePlan.PricePlanText.TextContent);
			}
		}


		[Test]
		public async Task TheOptionsScreenIsCorrect()
		{
			Assert.IsNotNull(Sut.CancelButton);
			Assert.AreEqual("Cancel", Sut.CancelButton.TextContent);

			Assert.IsNotNull(Sut.SetUpNewDirectDebitButton);
			Assert.IsNotNull(Sut.ManualPaymentsButton);
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
