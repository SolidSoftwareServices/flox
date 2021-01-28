using System;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;
using EI.RP.CoreServices.System;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step4.OneAccount
{
	[TestFixture]
	internal abstract class WhenMovingOneAccount : MyAccountCommonTests<ChoosePaymentOptionsPage>
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
						.AddElectricityAccount(paymentType: ScenarioPaymentMethodType,isPRNDeRegistered: IsPRNDegistered,configureDefaultDevice:false)
						.WithElectricity24HrsDevices()
						;
				}

				if (ScenarioAccountType == ClientAccountType.Gas)
				{
					UserConfig
						.AddGasAccount(paymentType: ScenarioPaymentMethodType,isPrnDeregistered: IsPRNDegistered, configureDefaultDevice: false)
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
				   (await movingHomeLandingPage.ClickOnElement(movingHomeLandingPage.PopupButton1))
				   .CurrentPageAs<Step1InputMoveOutPage>()
				   .InputFormValues(UserConfig);

				var step2Page1 = (await step1Page.ClickOnElement(step1Page.GetNextPRNButton()))
					.CurrentPageAs<Step2InputPrnsPage>();
				step2Page1.InputFormValues(UserConfig);

				var step2Page2 = (await step2Page1.ClickOnElement(step2Page1.SubmitPRNS)).CurrentPageAs<Step2ConfirmAddressPage>();
				step3Page = (await step2Page2.ClickOnElement(step2Page2.ButtonContinue)).CurrentPageAs<Step3InputMoveInPropertyDetailsPage>().InputFormValues(UserConfig); ;
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
		public async Task HandlesUserPath_MANUAL_ThenNEW_ShowsSkipOrCancelDirectDebitSetup()
		{
			//bypassing javascript as still experimental on anglesharp and not working
			await Sut.SelectNewDirectDebitFromDialog();
			var inputDirectDebitDetailsPage = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();

			if (ScenarioPaymentMethodType == PaymentMethodType.Manual)
			{
				Assert.IsNotNull(inputDirectDebitDetailsPage.SkipDirectDebitSetupLink, "Should have 'skip direct debit' link");
			}
			else if (ScenarioPaymentMethodType == PaymentMethodType.DirectDebit)
			{
				Assert.IsNotNull(inputDirectDebitDetailsPage.CancelDirectDebitSetupLink, "Should have 'cancel direct debit' link");
			}
		}

		[Test]
		public async Task HandlesUserPath_NEW_InputComplete_ShowsStep5WithCorrectInfo()
		{
			const string iban = "IE65AIBK93104715784037";
			const string ibanWithNonNumeric = "IE29AIBK93115212345678#";
			const string badCustomerBankName = "NotAR34lN4me!";

			var inputDirectDebitDetailsPage = (await Sut.SelectNewDirectDebit()).CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();

			AssertAccountHeader(inputDirectDebitDetailsPage);

			Assert.IsEmpty(inputDirectDebitDetailsPage.IbanErrorMessage.TextContent);
			Assert.IsEmpty(inputDirectDebitDetailsPage.CustomerNameErrorMessage.TextContent);
			inputDirectDebitDetailsPage.Iban.Value = ibanWithNonNumeric;
			inputDirectDebitDetailsPage.CustomerName.Value = badCustomerBankName;
			inputDirectDebitDetailsPage = (await inputDirectDebitDetailsPage.Complete()).CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			Assert.IsNotEmpty(inputDirectDebitDetailsPage.CustomerNameErrorMessage.TextContent);
			Assert.IsNotEmpty(inputDirectDebitDetailsPage.IbanErrorMessage.TextContent);

			inputDirectDebitDetailsPage.Iban.Value = iban;
			inputDirectDebitDetailsPage.CustomerName.Value = "Ulster Bank";
			inputDirectDebitDetailsPage.TermsAndConditions.IsChecked = true;

			var step5 = (await inputDirectDebitDetailsPage.ClickOnElement(inputDirectDebitDetailsPage.CompleteDirectDebitSetup))
				.CurrentPageAs<Step5ReviewAndCompletePage>();
			AssertStep5Review(step5);

			Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains(iban.Substring(iban.Length - 4)));
			if (IsPRNDegistered && ScenarioAccountType != ClientAccountType.Gas)
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

		protected void AssertAccountHeader(InputDirectDebitDetailsWhenMovingHomePage inputPage)
		{
			var accountInfo = UserConfig.Accounts.ElementAt(0);
			Assert.AreEqual($"{accountInfo.ClientAccountType} ({accountInfo.AccountNumber})", inputPage.HeaderElement.TextContent);
		}

		protected void AssertStep5Review(Step5ReviewAndCompletePage reviewPage)
		{
			var accountInfo = UserConfig.Accounts.ElementAt(0);
			Assert.AreEqual("Review your details", reviewPage.ReviewDetailsHeader.TextContent);
			Assert.IsTrue(reviewPage.ReviewDetailsContent.TextContent.Contains("You're almost done, please review and confirm the details of your move"));

			//account info
			Assert.AreEqual("Account Details", reviewPage.ShowAccountDetails.AccountDetailsHeader.TextContent);
			Assert.IsNotNull(reviewPage.ShowAccountDetails.AccountDetailsPrimaryAccountType);
			Assert.IsNotNull(reviewPage.ShowAccountDetails.AccountDetailsPrimaryAccountNumber);
			Assert.IsTrue(reviewPage.ShowAccountDetails.AccountDetailsPrimaryAccountType.TextContent.Contains(ScenarioAccountType));
			Assert.IsTrue(reviewPage.ShowAccountDetails.AccountDetailsPrimaryAccountNumber.TextContent.Contains(accountInfo.AccountNumber));

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
				devicesMeterReading = accountConfigurators[0].NewPremise.Devices.Single().Registers.Single();
				Assert.AreEqual(step3Page.GetElectricityReadingInput(devicesMeterReading.MeterNumber).Value,
					reviewPage.ShowMovingDatesAndMeterReadings.MoveInElectricityMeterReadingValue.TextContent);
			}
			else
			{
				var devicesMeterReading = accountConfigurators[0].Premise.Devices.Single().Registers.Single();
				Assert.AreEqual(step1Page.GetGasReadingInput(devicesMeterReading.MeterNumber).Value,
					reviewPage.ShowMovingDatesAndMeterReadings.MoveOutGasMeterReadingValue.TextContent);
				devicesMeterReading = accountConfigurators[0].NewPremise.Devices.Single().Registers.Single();
				Assert.AreEqual(step3Page.GetGasReadingInput(devicesMeterReading.MeterNumber).Value,
					reviewPage.ShowMovingDatesAndMeterReadings.MoveInGasMeterReadingValue.TextContent);
			}

			//payments
			Assert.AreEqual("Payment Method", reviewPage.ShowPayments.PaymentHeader.TextContent);

			Assert.IsTrue(reviewPage.ShowPayments.PrimaryAccountType.TextContent.Contains(ScenarioAccountType));
			Assert.IsNotNull(reviewPage.ShowPayments.PrimaryPaymentEdit);

			//price plan
			Assert.AreEqual("Price Plan", reviewPage.ShowPricePlan.PricePlanHeader.TextContent);
			Assert.IsNotNull(reviewPage.ShowPricePlan.PricePlanText);
		}
	}
}