using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.DomainServices.Commands.Banking.DirectDebit.SetUpDirectDebit;
using EI.RP.DomainServices.Commands.Contracts.MovingHouse;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step5Confirmation.MoveAndAddFolder
{
	[TestFixture]
	class WhenMoveGasAndAddElectricityDirectDebitAndManualPayment : WhenConfirmationScreen
	{
		protected override PaymentMethodType ScenarioPaymentMethodType => PaymentMethodType.Manual;

		protected override bool IsPRNDegistered => false;

		protected override bool HasFreeElectricityAllowance => false;

		protected override MovingHouseType MovingHouseType { get => MovingHouseType.MoveGasAndAddElectricity; }

		protected override async Task<Step1InputMoveOutPage> ToStep1()
		{
			var movingHomeLandingPage = App.CurrentPageAs<Step0LandingPage>();
			return (await movingHomeLandingPage.ClickOnElement(movingHomeLandingPage.PopupButton2))
				.CurrentPageAs<Step1InputMoveOutPage>();
		}

		protected override async Task TestScenarioArrangement()
		{
			ConfigureAccounts();
			await ValidSession();
            await ToStep0();
			await ToStep1();
			await ToStep2InputPrns();
			await ToStep2ConfirmAddressPage();
			await ToStep3InputMoveInPropertyDetails();
			await ToStep4();

			var page = App.CurrentPageAs<ChoosePaymentOptionsPage>();
			page.UseSameNewDirectDebitForAllAccountsCheckBox.IsChecked = false;

			(await page.SelectNewDirectDebit()).CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();

			(await App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>().Skip()).CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>().InputFormValues("IE07BOFI90159756578872", accountName: "Nick");
			await (App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>().Complete());

			var reviewPage = App.CurrentPageAs<Step5ReviewAndCompletePage>();
			await reviewPage.ClickOnElement(reviewPage.CompleteMoveHouse);
			Sut = App.CurrentPageAs<Step5ConfirmationPage>();			
		}

		protected override void ConfigureAccounts()
		{
			UserConfig = App.ConfigureUser("a@A.com", "test");
			var gasAccount = UserConfig
					.AddGasAccount(
					paymentType: PaymentMethodType.Manual,
					isPrnDeregistered: IsPRNDegistered, 
					configureDefaultDevice: false,
					newPrnAddressExists: true,
					canAddNewAccount: true)
					.WithGasDevice();

			UserConfig.Execute();
			UserConfig.GasAccount().Premise.InstallationInfo.HasFreeElectricityAllowance = HasFreeElectricityAllowance;
			UserConfig.GasAccount().NewPremise.InstallationInfo.HasFreeElectricityAllowance = HasFreeElectricityAllowance;
        }

		protected override async Task<Step5ConfirmationPage> ToStep5ConfirmationScreen(
			InputDirectDebitDetailsWhenMovingHomePage inputDirectDebitDetailsPage)
		{
			var setUpDirectDebitDomainCommands = GetDirectDebitDomainCommandsForScenario(inputDirectDebitDetailsPage);

			var cmd = new MoveHouse(
				UserConfig.ElectricityAccounts().FirstOrDefault()?.Model?.AccountNumber,
				UserConfig.GasAccounts().FirstOrDefault()?.Model?.AccountNumber,
				MovingHouseType,
				setUpDirectDebitDomainCommands,
				UserConfig.Accounts.First().ClientAccountType);

			App.DomainFacade.CommandDispatcher.ExpectCommandAndSuccess(cmd);
			var step5 = App.CurrentPageAs<Step5ReviewAndCompletePage>();
			await step5.ClickOnElement(step5.CompleteMoveHouse);
			App.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(cmd);

			return App.CurrentPageAs<Step5ConfirmationPage>();
		}

		private List<SetUpDirectDebitDomainCommand> GetDirectDebitDomainCommands(
			InputDirectDebitDetailsWhenMovingHomePage inputDirectDebitDetailsPage = null)
		{
			var setUpDirectDebitDomainCommands = new List<SetUpDirectDebitDomainCommand>();

			var electricityAccountInfo = UserConfig.ElectricityAccounts().First();

			var accountInfo = UserConfig.Accounts.ElementAt(0);
			var incomingBankAccount = UserConfig.ElectricityAndGasAccountConfigurators.Single().Model.IncomingBankAccount;

			var command = new SetUpDirectDebitDomainCommand(
			accountInfo.AccountNumber,
			inputDirectDebitDetailsPage.CustomerName.Value,
			incomingBankAccount.IBAN,
			inputDirectDebitDetailsPage.Iban.Value,
			accountInfo.Partner,
			accountInfo.ClientAccountType,
			PaymentMethodType.DirectDebit,
			null,
			null,
			null);

			setUpDirectDebitDomainCommands.Add(command);
			return setUpDirectDebitDomainCommands;
		}

		[Test]
		public async Task Step5ConfirmationPage_Test_CanSeeComponents()
		{
			CanSeeComponents();
		}

		private void CanSeeComponents()
		{
			Assert.IsNull(Sut.ConfirmationElectricityAccountNumber);
			Assert.IsNotNull(Sut.ConfirmationGasAccountNumber);

			Assert.AreEqual(
				"Thank you, your accounts have been moved to your new address.",
				Sut.ThankYouAccountMoveNotice.TextContent.Trim());

			Assert.AreEqual(
				"We are creating your new account and in the next four weeks you'll receive your account number. Your new savings will be applied within 24 hours.",
				Sut.CreatingNewAccountsNotice.TextContent.Trim());

			Assert.IsNull(Sut.FreeElectricityAllowanceNotice, "Did not expect to see Free Electricty Allowance Notice");

			Assert.AreEqual(
				"Your Electricity bill will be paid manually.",
				Sut.YourElectricityAccountWillBePaidByNotice.TextContent.Trim());

			Assert.AreEqual(
				"Your Gas bill will be paid by direct debit.",
				Sut.YourGasAccountWillBePaidByNotice.TextContent.Trim());

			Assert.IsNotNull(
				Sut.BackToMyAccountsLink,
				"expected Back to my accounts link");
		}
	}
}