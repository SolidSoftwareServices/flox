using System;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Accounts;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step4.MoveBothAccounts
{
	[TestFixture]
	internal abstract class WhenMovingBothAccounts : MyAccountCommonTests<ChoosePaymentOptionsPage>
	{
		protected abstract PaymentMethodType ScenarioElectricityPaymentMethodType { get; }
		protected abstract PaymentMethodType ScenarioGasPaymentMethodType { get; }

		protected abstract bool IsPRNDegistered { get; }

		protected string AccountName { get; set; }
		protected string AccountName2 { get; set; }

		private Step1InputMoveOutPage step1Page;

		private Step3InputMoveInPropertyDetailsPage step3Page;

		protected override async Task TestScenarioArrangement()
		{
			AccountName = "Ulster Bank";
			AccountName2 = "Bank of Ireland";

			ConfigureAccounts();

			Sut = await ResolveScenarioSut();

			void ConfigureAccounts()
			{
				UserConfig = App.ConfigureUser("a@A.com", "test");

				var gasAccount = UserConfig
					.AddGasAccount(paymentType: ScenarioGasPaymentMethodType, newPrnAddressExists: true, isPrnDeregistered: IsPRNDegistered,configureDefaultDevice: false)
					.WithGasDevice()
					;

				UserConfig
					.AddElectricityAccount(paymentType: ScenarioElectricityPaymentMethodType, duelFuelSisterAccount: gasAccount, isPRNDeRegistered: IsPRNDegistered,configureDefaultDevice:false)
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
				   (await movingHomeLandingPage.ClickOnElement(movingHomeLandingPage.PopupButton1))
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
				if (ScenarioGasPaymentMethodType == PaymentMethodType.DirectDebit || ScenarioElectricityPaymentMethodType == PaymentMethodType.DirectDebit)
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
			var step0LandingPage = (await Sut.ClickOnCancelConfirm()).CurrentPageAs<Step0LandingPage>();
			App.CurrentPageAs<Step0LandingPage>();
		}


		[Test]
		public virtual async Task HandlesUserPath_USE_EXISTING_USE_SAME_DD_ShowsStep5WithCorrectInfo()
		{
			Sut.CheckBoxConfirmContinueDebit.IsChecked = true;
			Sut.ConfirmDetailsAreCorrectExistingDirectDebitCheckBox.IsChecked = true;
			await Sut.SelectExistingDirectDebit();

			var step5 = App.CurrentPageAs<Step5ReviewAndCompletePage>();
			AssertStep5Review(step5);
			Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains("Direct Debit"));
			Assert.IsTrue(step5.ShowPayments.SecondaryPaymentType.TextContent.Contains("Direct Debit"));

			Assert.AreEqual(step5.ShowPayments.PrimaryPaymentType.Text().Replace("\t", "").Replace(" ", "").Trim(), step5.ShowPayments.SecondaryPaymentType.Text().Replace("\t", "").Replace(" ", "").Trim());
			if (IsPRNDegistered)
			{
				Assert.IsTrue(
					step5.ShowPricePlan.PricePlanText.TextContent.Contains("Your price plan and savings will reflect your details displayed above."));
			}
			else
			{
				Assert.IsTrue(step5.ShowPricePlan.PricePlanText.TextContent.Contains("Your price plan and savings will reflect your details displayed above"));
			}
		}

		[Test]
		public virtual async Task HandlesUserPath_USE_EXISTING_USE_INDIVIDUAL_DD_ShowsStep5WithCorrectInfo()
		{
			Sut.CheckBoxUseExistingDDSetupForAllAccounts.IsChecked = false;
			Sut.ConfirmDetailsAreCorrectExistingDirectDebitCheckBox.IsChecked = true;

			await Sut.SelectExistingDirectDebit();

			var inputPage = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();

			AssertElectricityAccountHeader(inputPage);
			AssertDirectDebitInputFieldsArePopulated(inputPage, UserConfig.ElectricityAccounts().First());

			inputPage.AssertCancelDirectDebitAndSkipSetupLinks(shouldShowCancelLink: false, shouldShowSkipLink: true);

			var gasAccount = UserConfig.GasAccounts().Single();
			var ibanValue = "IE03AIBK93504210987045";
			inputPage.Iban.Value = ibanValue;
			inputPage.InputFormValues(ibanValue, gasAccount.Model.IncomingBankAccount.NameInBankAccount, true);

			var step5 = (await inputPage.Complete()).CurrentPageAs<Step5ReviewAndCompletePage>();
			AssertStep5Review(step5);
			Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains("Direct Debit"));
			Assert.IsTrue(step5.ShowPayments.SecondaryPaymentType.TextContent.Contains("Direct Debit"));
			if (IsPRNDegistered)
			{
				Assert.IsTrue(
					step5.ShowPricePlan.PricePlanText.TextContent.Contains("Your price plan and savings will reflect your details displayed above."));
			}
			else
			{
				Assert.IsTrue(step5.ShowPricePlan.PricePlanText.TextContent.Contains("Your price plan and savings will reflect your details displayed above"));
			}
		}

		[Test]
		public virtual async Task HandlesUserPath_NEWDD_Use_Individual_DD_Complete_Cancel_New_Complete_Complete_ShowsStep5WithCorrectInfo()
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
				var inputDirectDebitPageOne = (await App.CurrentPageAs<ChoosePaymentOptionsPage>().SelectNewDirectDebit()).CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
				inputDirectDebitPageOne.AssertCancelDirectDebitAndSkipSetupLinks(shouldShowCancelLink: false, shouldShowSkipLink: true);
				inputDirectDebitPageOne.InputFormValues(ibanValue, AccountName, confirmTerms: true);
				var inputDirectDebitPageTwo = (await inputDirectDebitPageOne.Complete()).CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
				inputDirectDebitPageTwo.AssertCancelDirectDebitAndSkipSetupLinks(shouldShowCancelLink: true, shouldShowSkipLink: false);
				inputDirectDebitPageTwo.InputFormValues(ibanValue, AccountName, confirmTerms: true);
			}

			async Task CancelYesImSureDirectDebitFlow()
			{
				await App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>().CancelDirectDebitSetup();
			}

			async Task CompleteDirectDebitSetup()
			{
				var step5 = (await App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>().Complete()).CurrentPageAs<Step5ReviewAndCompletePage>();

				AssertStep5Review(step5);
				Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains("Direct Debit"));
				Assert.IsTrue(step5.ShowPayments.SecondaryPaymentType.TextContent.Contains("Direct Debit"));

				Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains(ibanValue.Substring(ibanValue.Length - 4)));
				Assert.IsTrue(step5.ShowPayments.SecondaryPaymentType.TextContent.Contains(ibanValue.Substring(ibanValue.Length - 4)));

				if (IsPRNDegistered)
				{
					Assert.IsTrue(
						step5.ShowPricePlan.PricePlanText.TextContent.Contains("Your price plan and savings will reflect your details displayed above."));
				}
				else
				{
					Assert.IsTrue(step5.ShowPricePlan.PricePlanText.TextContent.Contains("Your price plan and savings will reflect your details displayed above"));
				}
			}
		}

		[Test]
		public virtual async Task HandlesUserPath_NEWDD_USE_SAME_DD_ShowsStep5WithCorrectInfo()
		{
			var electricityAccountInfo = UserConfig.Accounts.ElementAt(1);
			var gasAccountInfo = UserConfig.Accounts.ElementAt(0);

			Sut.UseSameNewDirectDebitForAllAccountsCheckBox.IsChecked = true;
			await Sut.SelectNewDirectDebit();

			var inputPage = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			inputPage.AssertCancelDirectDebitAndSkipSetupLinks(shouldShowCancelLink: true, shouldShowSkipLink: false);

			Assert.AreEqual($"{electricityAccountInfo.ClientAccountType} ({electricityAccountInfo.AccountNumber}) & {gasAccountInfo.ClientAccountType} ({gasAccountInfo.AccountNumber})", inputPage.HeaderElement.TextContent);
			var ibanValue = "IE03AIBK93504210987045";
			inputPage.InputFormValues(ibanValue, AccountName);
			await inputPage.Complete();

			var step5 = App.CurrentPageAs<Step5ReviewAndCompletePage>();

			AssertStep5Review(step5);
			Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains("Direct Debit"));
			Assert.IsTrue(step5.ShowPayments.SecondaryPaymentType.TextContent.Contains("Direct Debit"));

			Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains(ibanValue.Substring(ibanValue.Length - 4)));
			Assert.IsTrue(step5.ShowPayments.SecondaryPaymentType.TextContent.Contains(ibanValue.Substring(ibanValue.Length - 4)));

			if (IsPRNDegistered)
			{
				Assert.IsTrue(
					step5.ShowPricePlan.PricePlanText.TextContent.Contains("Your price plan and savings will reflect your details displayed above."));
			}
			else
			{
				Assert.IsTrue(step5.ShowPricePlan.PricePlanText.TextContent.Contains("Your price plan and savings will reflect your details displayed above"));
			}
		}

		[Test]
		public virtual async Task HandlesUserPath_NEWDD_USE_INDIVIDUAL_DD_ShowsStep5WithCorrectInfo()
		{
			Sut.UseSameNewDirectDebitForAllAccountsCheckBox.IsChecked = false;
			await Sut.SelectNewDirectDebit();

			var inputPage = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			inputPage.AssertCancelDirectDebitAndSkipSetupLinks(shouldShowCancelLink: false, shouldShowSkipLink: true);
			AssertElectricityAccountHeader(inputPage);
			Assert.AreEqual("Continue to gas direct debit", inputPage.CompleteDirectDebitSetup.TextContent);

			await AssertDirectDebitInputFieldValidation(inputPage);
			inputPage = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
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
			Assert.IsTrue(step5.ShowPayments.SecondaryPaymentType.TextContent.Contains("Direct Debit"));

			Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains(ibanValue.Substring(ibanValue.Length - 4)));
			Assert.IsTrue(step5.ShowPayments.SecondaryPaymentType.TextContent.Contains(ibanValue2.Substring(ibanValue2.Length - 4)));

			if (IsPRNDegistered)
			{
				Assert.IsTrue(
					step5.ShowPricePlan.PricePlanText.TextContent.Contains("Your price plan and savings will reflect your details displayed above."));
			}
			else
			{
				Assert.IsTrue(step5.ShowPricePlan.PricePlanText.TextContent.Contains("Your price plan and savings will reflect your details displayed above"));
			}
		}

		[Test]
		public virtual async Task HandlesUserPath_NEWDD_USE_INDIVIDUAL_DD_SKIP_then_COMPLETE_ShowsStep5WithCorrectInfo()
		{
			Sut.UseSameNewDirectDebitForAllAccountsCheckBox.IsChecked = false;
			var inputPage = (await Sut.SelectNewDirectDebit()).CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			inputPage.AssertCancelDirectDebitAndSkipSetupLinks(shouldShowCancelLink: false, shouldShowSkipLink: true);

			inputPage = (await inputPage.Skip()).CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();

			inputPage.AssertCancelDirectDebitAndSkipSetupLinks(shouldShowCancelLink: true, shouldShowSkipLink: false);
			AssertGasAccountHeader(inputPage);

			var ibanValue2 = "IE07BOFI90159756578872";
			inputPage.InputFormValues(ibanValue2, AccountName2);

			await inputPage.Complete();

			var step5 = App.CurrentPageAs<Step5ReviewAndCompletePage>();

			AssertStep5Review(step5);
			Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains("Manual"));
			Assert.IsTrue(step5.ShowPayments.SecondaryPaymentType.TextContent.Contains("Direct Debit"));

			Assert.IsTrue(step5.ShowPayments.SecondaryPaymentType.TextContent.Contains(ibanValue2.Substring(ibanValue2.Length - 4)));

			if (IsPRNDegistered)
			{
				Assert.IsTrue(
					step5.ShowPricePlan.PricePlanText.TextContent.Contains("Your price plan and savings will reflect your details displayed above."));
			}
			else
			{
				Assert.IsTrue(step5.ShowPricePlan.PricePlanText.TextContent.Contains("Your price plan and savings will reflect your details displayed above"));
			}

		}

		[Test]
		public virtual async Task HandlesUserPath_NEWDD_Use_Same_CancelSetup_Use_Individual_DD_Complete_ShowsCorrectHeaders()
		{
			Sut.UseSameNewDirectDebitForAllAccountsCheckBox.IsChecked = true;
			var ibanValue = "IE03AIBK93504210987045";
			//var accName = ModelsBuilder.Create<string>();

			var inputDirectDebitPageOne = (await Sut.SelectNewDirectDebit()).CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();

			var choosePaymentOptionsPage = (await inputDirectDebitPageOne.CancelDirectDebitSetup()).CurrentPageAs<ChoosePaymentOptionsPage>();
			choosePaymentOptionsPage.UseSameNewDirectDebitForAllAccountsCheckBox.IsChecked = false;

			inputDirectDebitPageOne = (await choosePaymentOptionsPage.SelectNewDirectDebit()).CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			inputDirectDebitPageOne.InputFormValues(ibanValue, AccountName, confirmTerms: true);

			AssertElectricityAccountHeader(inputDirectDebitPageOne);

			var inputDirectDebitPageTwo = (await inputDirectDebitPageOne.Complete()).CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();

			AssertGasAccountHeader(inputDirectDebitPageTwo);
		}
	
		protected void AssertDirectDebitInputFieldsArePopulated(InputDirectDebitDetailsWhenMovingHomePage directDebitSut, CommonElectricityAndGasAccountConfigurator account)
		{
			Assert.AreEqual(account.Model.IncomingBankAccount.IBAN.Substring
						(account.Model.IncomingBankAccount.IBAN.Length - 4),
					directDebitSut.Iban.Value.Substring(directDebitSut.Iban.Value.Length - 4));
		}

		protected void AssertDirectDebitInputFieldsAreBlank(InputDirectDebitDetailsWhenMovingHomePage inputPage)
		{
			Assert.AreEqual(string.Empty, inputPage.Iban.Value);
			Assert.AreEqual(string.Empty, inputPage.CustomerName.Value);
		}

		protected void AssertGasAccountHeader(InputDirectDebitDetailsWhenMovingHomePage inputPage)
		{
			var gasAccountInfo = UserConfig.Accounts.ElementAt(0);
			Assert.AreEqual($"{gasAccountInfo.ClientAccountType} ({gasAccountInfo.AccountNumber})", inputPage.HeaderElement.TextContent);
		}

		protected void AssertElectricityAccountHeader(InputDirectDebitDetailsWhenMovingHomePage inputPage)
		{
			var electricityAccountInfo = UserConfig.Accounts.ElementAt(1);
			Assert.AreEqual($"{electricityAccountInfo.ClientAccountType} ({electricityAccountInfo.AccountNumber})", inputPage.HeaderElement.TextContent);
		}

		protected void AssertInputFieldHasValue(IHtmlInputElement inputElement, string expectedText)
		{
			Assert.AreEqual(expectedText, inputElement.Value);
		}

		protected void AssertUseNewForBothIsChecked(ChoosePaymentOptionsPage choosePaymentOptionsPage)
		{
			Assert.IsTrue(choosePaymentOptionsPage.UseSameNewDirectDebitForAllAccountsCheckBox.IsChecked, "UseNewSingleSetupForAllAccounts should be checked after user cancels direct debit setup");
		}

		protected async Task AssertDirectDebitInputFieldValidation(InputDirectDebitDetailsWhenMovingHomePage inputPage)
		{
			var invalidIban = "invalid iban";
			var invalidAccount = "Inv4L!D Acc0int#";

			inputPage.InputFormValues(invalidIban, accountName: invalidAccount, confirmTerms: false);
			inputPage = (await inputPage.Complete()).CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			AssertInputFieldHasValue(inputPage.Iban, invalidIban);
			AssertInputFieldHasValue(inputPage.CustomerName, invalidAccount);
			Assert.AreEqual(inputPage.IbanErrorMessage.TextContent, "Please enter a valid IBAN");
			Assert.AreEqual(inputPage.CustomerNameErrorMessage.TextContent, "Please enter a Bank Account name");

			inputPage.InputFormValues(iban: string.Empty, accountName: string.Empty, confirmTerms: false);
			inputPage = (await inputPage.Complete()).CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			AssertInputFieldHasValue(inputPage.Iban, string.Empty);
			AssertInputFieldHasValue(inputPage.CustomerName, string.Empty);
			Assert.AreEqual(inputPage.IbanErrorMessage.TextContent, "Please enter a valid IBAN");
			Assert.AreEqual(inputPage.CustomerNameErrorMessage.TextContent, "Please enter a Bank Account name");
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
			Assert.IsTrue(
				reviewPage.ShowAccountDetails.AccountDetailsPrimaryAccountType.TextContent.Contains(
					ClientAccountType.Electricity));
			Assert.IsTrue(
				reviewPage.ShowAccountDetails.AccountDetailsPrimaryAccountNumber.TextContent.Contains(
					electricityAccountInfo.AccountNumber));

			Assert.IsNotNull(reviewPage.ShowAccountDetails.AccountDetailsSecondaryAccountType);
			Assert.IsNotNull(reviewPage.ShowAccountDetails.AccountDetailsSecondaryAccountNumber);
			Assert.IsTrue(
				reviewPage.ShowAccountDetails.AccountDetailsSecondaryAccountType.TextContent.Contains(
					ClientAccountType.Gas));
			Assert.IsTrue(
				reviewPage.ShowAccountDetails.AccountDetailsSecondaryAccountNumber.TextContent.Contains(
					gasAccountInfo.AccountNumber));


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
			gasDevicesMeterReading = accountConfigurators[0].NewPremise.GasDevice().Registers.Single();
			Assert.AreEqual(step3Page.GetGasReadingInput(gasDevicesMeterReading.MeterNumber).Value,
				reviewPage.ShowMovingDatesAndMeterReadings.MoveInGasMeterReadingValue.TextContent);

			Assert.IsNotNull(reviewPage.ShowMovingDatesAndMeterReadings.EditMoveOutDetailsButton);
			Assert.IsNotNull(reviewPage.ShowMovingDatesAndMeterReadings.EditMoveInDetailsButton);

			//payments
			Assert.AreEqual("Payment Method", reviewPage.ShowPayments.PaymentHeader.TextContent);

			Assert.IsTrue(reviewPage.ShowPayments.PrimaryAccountType.TextContent.Contains(ClientAccountType.Electricity));
			Assert.IsNotNull(reviewPage.ShowPayments.PrimaryPaymentEdit);

			Assert.IsTrue(reviewPage.ShowPayments.SecondaryAccountType.TextContent.Contains(ClientAccountType.Gas));
			Assert.IsNotNull(reviewPage.ShowPayments.PrimaryPaymentEdit);

			//price plan
			Assert.AreEqual("Price Plan", reviewPage.ShowPricePlan.PricePlanHeader.TextContent);
			Assert.IsNotNull(reviewPage.ShowPricePlan.PricePlanText);
		}
	}
}