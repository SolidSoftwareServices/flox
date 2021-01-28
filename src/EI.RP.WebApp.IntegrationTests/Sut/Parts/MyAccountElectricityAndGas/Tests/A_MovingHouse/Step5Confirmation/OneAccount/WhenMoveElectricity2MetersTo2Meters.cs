using System;
using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step5Confirmation.OneAccount
{
	[TestFixture]
	internal class WhenMoveElectricity2MetersTo2Meters : WhenConfirmationScreen
	{
		protected override PaymentMethodType ScenarioPaymentMethodType => PaymentMethodType.DirectDebit;

		protected override bool IsPRNDegistered => false;

		protected override bool HasFreeElectricityAllowance => false;
		
		protected override MovingHouseType MovingHouseType => MovingHouseType.MoveElectricity;

		private const string InputMeterMoveOutElectricity24Hrs = "10001";
		private const string InputMeterMoveOutElectricityNightStorageHeater = "10002";
		private const string InputMeterMoveInElectricity24Hrs = "20003";
		private const string InputMeterMoveInElectricityNightStorageHeater = "20004";
		
		protected override void ConfigureAccounts()
		{
			UserConfig = App.ConfigureUser("a@A.com", "test");

			UserConfig
				.AddElectricityAccount(

				paymentType: ExistingPaymentMethodType,
				isPRNDeRegistered: IsPRNDegistered,
				configureDefaultDevice: false,
				newPrnAddressExists: true,
				canAddNewAccount: MovingHouseType == MovingHouseType.MoveElectricityAndAddGas,
				hasFreeElectricityAllowance: HasFreeElectricityAllowance)
					.WithElectricity24HrsDevices()
					.WithElectricityNightStorageHeaterDevice();

			UserConfig.Execute();
		}

		protected override async Task TestScenarioArrangement()
		{
			InputDirectDebitDetailsWhenMovingHomePage inputDirectDebitDetailsPage = null;
			ConfigureAccounts();
			await ValidSession();
			await ToStep0();
			await ToStep1();
			await ToStep2InputPrns();
			await ToStep2ConfirmAddressPage();
			await ToStep3InputMoveInPropertyDetails();
			await ToStep4();

			if (ScenarioPaymentMethodType == PaymentMethodType.Manual)
			{
				await ToStep5ReviewPageWithManualPayments();
			}
			else
			{
				inputDirectDebitDetailsPage = (await ToStep4InputDirectDebitDetailsWhenMovingHomePage());
				await ToStep5ReviewAndCompletePage();
				var step5ReviewAndCompletePage = App.CurrentPageAs<Step5ReviewAndCompletePage>();
				AssertMetersMoveOutResults(step5ReviewAndCompletePage);
				AssertMetersMoveInResults(step5ReviewAndCompletePage);
			}

			Sut = await ToStep5ConfirmationScreen(inputDirectDebitDetailsPage);
		}

		protected override async Task<Step2InputPrnsPage> ToStep2InputPrns()
		{
			var step1Page = App.CurrentPageAs<Step1InputMoveOutPage>();
			step1Page.MoveOutDatePicker.Value = DateTime.Today.Subtract(TimeSpan.FromDays(3.0)).ToShortDateString();

			var electricityAccount = UserConfig.ElectricityAndGasAccountConfigurators
												.SingleOrDefault(x => x.Model.ClientAccountType == ClientAccountType.Electricity);

			var devices = electricityAccount.Premise.Devices.Where(x => x.Registers.Any(r=>r.MeterType.IsElectricity()));
			foreach (var device in devices)
			{
				var registerInfos = device.Registers;
				foreach (var registerInfo in registerInfos)
				{
					var inputElement = step1Page.GetElecReadingInput(registerInfo.MeterNumber);

					if (registerInfo.MeterType == MeterType.Electricity24h)
						inputElement.Value = InputMeterMoveOutElectricity24Hrs;

					if (registerInfo.MeterType == MeterType.ElectricityNightStorageHeater)
						inputElement.Value = InputMeterMoveOutElectricityNightStorageHeater;
				}
			}
			
			step1Page.IncomingOccupantNoCheckedBox.IsChecked = true;
			step1Page.IncomingOccupantYesCheckedBox.IsChecked = false;
			step1Page.CheckBoxDetails.IsChecked = true;
			step1Page.CheckBoxTerms.IsChecked = true;

			return (await step1Page.ClickOnElement(step1Page.GetNextPRNButton()))
				.CurrentPageAs<Step2InputPrnsPage>();
		}

		protected override async Task<ChoosePaymentOptionsPage> ToStep4()
		{
			var step3Page = App.CurrentPageAs<Step3InputMoveInPropertyDetailsPage>();
			step3Page.MoveOutDatePicker.Value = DateTime.Today.Subtract(TimeSpan.FromDays(2.0)).ToShortDateString();

			step3Page.ContactNumber.Value = "0879876543";

			var electricityAccount = UserConfig.ElectricityAndGasAccountConfigurators
												.SingleOrDefault(x => x.Model.ClientAccountType == ClientAccountType.Electricity);

			var devices = electricityAccount.NewPremise.Devices.Where(x => x.Registers.Any(r => r.MeterType.IsElectricity()));
			foreach (var device in devices)
			{
				var registerInfos = device.Registers;
				foreach (var registerInfo in registerInfos)
				{
					var inputElement = step3Page.GetElectricityReadingInput(registerInfo.MeterNumber);

					if (registerInfo.MeterType == MeterType.Electricity24h)
						inputElement.Value = InputMeterMoveInElectricity24Hrs;

					if (registerInfo.MeterType == MeterType.ElectricityNightStorageHeater)
						inputElement.Value = InputMeterMoveInElectricityNightStorageHeater;
				}
			}

			step3Page.CheckBoxDetails.IsChecked = true;
			step3Page.CheckBoxTerms.IsChecked = true;

			return (await step3Page.ClickOnElement(step3Page.NextPaymentOptions)).CurrentPageAs<ChoosePaymentOptionsPage>();
		}

		[Test]
		public void Step5ConfirmationPage_Test_CanSeeComponents()
		{
			CanSeeComponents();
		}

		private void CanSeeComponents()
		{
			Assert.IsNotNull(Sut.ConfirmationElectricityAccountNumber.TextContent);

			Assert.AreEqual(
				"Thank you, your account has been moved to your new address.",
				Sut.ThankYouAccountMoveNotice.TextContent.Trim());

			Assert.IsNull(
				Sut.FreeElectricityAllowanceNotice,
				"Should not see Free Electricity Allowance Notice for this scenario");

			Assert.AreEqual(
				"Your Electricity bill will be paid by direct debit.",
				Sut.YourElectricityAccountWillBePaidByNotice.TextContent.Trim());

			Assert.IsNotNull(
				Sut.BackToMyAccountsLink,
				"expected Back to my accounts link");

			Assert.IsTrue(Sut.DirectDebitChangeConfirmation?.TextContent.Trim().Equals("You have changed your payment method to Direct Debit, your savings will be applied within 12 days."));

			Assert.IsTrue(Sut.ReadTermsAndConditions?.Href.Equals("https://www.electricireland.ie/residential/helpful-links/terms-conditions/residential-electricity-and-gas-pricing"));
			Assert.IsTrue(Sut.ReadTermsAndConditions?.TextContent.Trim().Equals("Read T&Cs."));

			Assert.IsTrue(Sut.DirectDebitBillMessage?.TextContent.Trim().Equals("Your first Direct Debit payment will be made 14 days after your next bill is issued. To update your Direct Debit details at any time go to"));
			Assert.IsTrue(Sut.BillAndPaymentLink?.TextContent.Trim().Equals("Payments"));
		}

		protected void AssertMetersMoveOutResults(Step5ReviewAndCompletePage reviewPage)
		{
			var electricityAccount = UserConfig.ElectricityAndGasAccountConfigurators
									.SingleOrDefault(x => x.Model.ClientAccountType == ClientAccountType.Electricity);

			var devices = electricityAccount.Premise.Devices.Where(x => x.Registers.Any(r => r.MeterType.IsElectricity()));
			foreach (var device in devices)
			{
				var registerInfos = device.Registers;
				foreach (var registerInfo in registerInfos)
				{
					var readingMoveOutElement = reviewPage.ShowAccountDetails.GetElecReadingMoveOutElement(registerInfo.MeterNumber);
					Assert.IsNotNull(readingMoveOutElement);

					var readingMoveOutResult = reviewPage.ShowAccountDetails.GetElecReadingMoveOutResult(registerInfo.MeterNumber);
					Assert.IsNotNull(readingMoveOutResult);
				
					if (registerInfo.MeterType == MeterType.Electricity24h)
						Assert.IsTrue(readingMoveOutResult.InnerHtml.Contains(InputMeterMoveOutElectricity24Hrs));

					if (registerInfo.MeterType == MeterType.ElectricityNightStorageHeater)
						Assert.IsTrue(readingMoveOutResult.InnerHtml.Contains(InputMeterMoveOutElectricityNightStorageHeater));
				}
			}
		}

		protected void AssertMetersMoveInResults(Step5ReviewAndCompletePage reviewPage)
		{
			var electricityAccount = UserConfig.ElectricityAndGasAccountConfigurators
									.SingleOrDefault(x => x.Model.ClientAccountType == ClientAccountType.Electricity);

			var devices = electricityAccount.NewPremise.Devices.Where(x => x.Registers.Any(r => r.MeterType.IsElectricity()));
			foreach (var device in devices)
			{
				var registerInfos = device.Registers;
				foreach (var registerInfo in registerInfos)
				{
					var readingMoveOutElement = reviewPage.ShowAccountDetails.GetElecReadingMoveInElement(registerInfo.MeterNumber);
					Assert.IsNotNull(readingMoveOutElement);

					var readingMoveOutResult = reviewPage.ShowAccountDetails.GetElecReadingMoveInResult(registerInfo.MeterNumber);
					Assert.IsNotNull(readingMoveOutResult);

					if (registerInfo.MeterType == MeterType.Electricity24h)
						Assert.IsTrue(readingMoveOutResult.InnerHtml.Contains(InputMeterMoveInElectricity24Hrs));

					if (registerInfo.MeterType == MeterType.ElectricityNightStorageHeater)
						Assert.IsTrue(readingMoveOutResult.InnerHtml.Contains(InputMeterMoveInElectricityNightStorageHeater));
				}
			}
		}

		[Test]
		public async Task CanNavigateToPaymentOptionsPage()
		{
			(await App.ClickOnElement(Sut.BillAndPaymentLink)).CurrentPageAs<ShowPaymentsHistoryPage>();
		}
	}
}