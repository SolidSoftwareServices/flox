using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.DomainServices.Commands.Banking.DirectDebit.SetUpDirectDebit;
using EI.RP.DomainServices.Commands.Contracts.MovingHouse;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step5Confirmation
{
	[TestFixture]
	abstract class WhenConfirmationScreen : MyAccountCommonTests<Step5ConfirmationPage>
	{
		protected abstract MovingHouseType MovingHouseType { get; }
		protected virtual PaymentMethodType ExistingPaymentMethodType => PaymentMethodType.Manual;
		protected abstract PaymentMethodType ScenarioPaymentMethodType { get; }
		protected abstract bool IsPRNDegistered { get; }
		protected abstract bool HasFreeElectricityAllowance { get; }

		protected const string ElectricityAndGasPricingUrl = "https://www.electricireland.ie/residential/helpful-links/terms-conditions/residential-electricity-and-gas-pricing";
		protected const string ReadTermsandConditionsUrl = "https://www.electricireland.ie/residential/helpful-links/terms-conditions/residential-electricity-and-gas-pricing";
		protected const string CreatingNewAccountNoticeText = "We are creating your new account and in the next four weeks you'll receive your account number. Your new savings will be applied within 24 hours.";


		#region TestScenarioArrangement

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
			}

			Sut = await ToStep5ConfirmationScreen(inputDirectDebitDetailsPage);
		}

		protected virtual void ConfigureAccounts()
		{
			UserConfig = App.ConfigureUser("a@A.com", "test");


			if (MovingHouseType == MovingHouseType.MoveElectricity ||
				MovingHouseType == MovingHouseType.MoveElectricityAndAddGas ||
				MovingHouseType == MovingHouseType.MoveElectricityAndCloseGas ||
				MovingHouseType == MovingHouseType.MoveElectricityAndGas)
			{
				UserConfig
					.AddElectricityAccount(

					paymentType: ExistingPaymentMethodType,
					isPRNDeRegistered: IsPRNDegistered,
					configureDefaultDevice: false,
					newPrnAddressExists: true,
					canAddNewAccount: MovingHouseType == MovingHouseType.MoveElectricityAndAddGas,
					hasFreeElectricityAllowance: HasFreeElectricityAllowance)
					.WithElectricity24HrsDevices();
			}
			if (MovingHouseType == MovingHouseType.MoveGas ||
				MovingHouseType == MovingHouseType.MoveElectricityAndCloseGas ||
				MovingHouseType == MovingHouseType.MoveElectricityAndGas ||
				MovingHouseType == MovingHouseType.MoveGasAndAddElectricity)
			{
				UserConfig
					.AddGasAccount(
						paymentType: ExistingPaymentMethodType,
						isPrnDeregistered: IsPRNDegistered,
						configureDefaultDevice: false,
						canAddNewAccount: MovingHouseType == MovingHouseType.MoveGasAndAddElectricity,
						newPrnAddressExists: true,
						duelFuelSisterAccount: UserConfig.ElectricityAccount(),
						hasFreeElectricityAllowance: HasFreeElectricityAllowance)
					.WithGasDevice();


			}


			UserConfig.Execute();
		}

		protected async Task ValidSession()
		{
			await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role);
		}

		protected async Task<Step0LandingPage> ToStep0()
		{
			if (UserConfig.Accounts.Count() > 1)
			{
				await App.CurrentPageAs<AccountSelectionPage>()
						.SelectAccount(UserConfig.ElectricityAccounts().Single().Model.AccountNumber);
			}
			else
			{
				await App.CurrentPageAs<AccountSelectionPage>().SelectFirstAccount();
			}

			return (await App.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMovingHome()).CurrentPageAs<Step0LandingPage>();
		}

		protected virtual async Task<Step1InputMoveOutPage> ToStep1()
		{
			var movingHomeLandingPage = App.CurrentPageAs<Step0LandingPage>();
			return (await movingHomeLandingPage.ClickOnElement(movingHomeLandingPage.PopupButton1)).CurrentPageAs<Step1InputMoveOutPage>();
		}

		protected virtual async Task<Step2InputPrnsPage> ToStep2InputPrns()
		{
			var step1Page = App.CurrentPageAs<Step1InputMoveOutPage>().InputFormValues(UserConfig);
			return (await step1Page.ClickOnElement(step1Page.GetNextPRNButton()))
				.CurrentPageAs<Step2InputPrnsPage>();
		}

		protected async Task<Step2ConfirmAddressPage> ToStep2ConfirmAddressPage()
		{
			var step2InputPrnsPage = App.CurrentPageAs<Step2InputPrnsPage>();
			step2InputPrnsPage.InputFormValues(UserConfig);
			return (await step2InputPrnsPage.ClickOnElement(step2InputPrnsPage.SubmitPRNS)).CurrentPageAs<Step2ConfirmAddressPage>();
		}

		protected async Task<Step3InputMoveInPropertyDetailsPage> ToStep3InputMoveInPropertyDetails()
		{
			var step2ConfirmAddressPage = App.CurrentPageAs<Step2ConfirmAddressPage>();
			return (await step2ConfirmAddressPage.ClickOnElement(step2ConfirmAddressPage.ButtonContinue))
				.CurrentPageAs<Step3InputMoveInPropertyDetailsPage>();
		}

		protected virtual async Task<ChoosePaymentOptionsPage> ToStep4()
		{
			var step3Page = App.CurrentPageAs<Step3InputMoveInPropertyDetailsPage>();
			step3Page.InputFormValues(UserConfig); ;
			return (await step3Page.ClickOnElement(step3Page.NextPaymentOptions)).CurrentPageAs<ChoosePaymentOptionsPage>();
		}

		protected async Task<InputDirectDebitDetailsWhenMovingHomePage> ToStep4InputDirectDebitDetailsWhenMovingHomePage()
		{
			var page = App.CurrentPageAs<ChoosePaymentOptionsPage>();
			return (await page.SelectNewDirectDebit()).CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
		}

		protected async Task<Step5ReviewAndCompletePage> ToStep5ReviewPageWithManualPayments()
		{
			var page = App.CurrentPageAs<ChoosePaymentOptionsPage>();
			return (await page.SelectManualPayment()).CurrentPageAs<Step5ReviewAndCompletePage>();
		}

		protected async Task<Step5ReviewAndCompletePage> ToStep5ReviewAndCompletePage()
		{
			const string iban = "IE29AIBK93115212345678";
			var inputDirectDebitDetailsPage = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			inputDirectDebitDetailsPage.Iban.Value = iban;
			inputDirectDebitDetailsPage.CustomerName.Value = "Ulster Bank";
			inputDirectDebitDetailsPage.TermsAndConditions.IsChecked = true;
			return (await inputDirectDebitDetailsPage.Complete()).CurrentPageAs<Step5ReviewAndCompletePage>();
		}

		protected List<SetUpDirectDebitDomainCommand> GetDirectDebitDomainCommandsForScenario(
			InputDirectDebitDetailsWhenMovingHomePage inputDirectDebitDetailsPage = null)
		{
			var setUpDirectDebitDomainCommands = new List<SetUpDirectDebitDomainCommand>();

			if (ScenarioPaymentMethodType == PaymentMethodType.Manual)
			{
				return setUpDirectDebitDomainCommands;
			}

			var accountInfo = UserConfig.Accounts.ElementAt(0);
			var incomingBankAccount = UserConfig.ElectricityAndGasAccountConfigurators.First().Model.IncomingBankAccount;

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

		protected virtual async Task<Step5ConfirmationPage> ToStep5ConfirmationScreen(
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

			return App.CurrentPageAs<Step5ConfirmationPage>();
		}

		#endregion

		[Test]
		public async Task CanNavigateToAccountDashboard()
		{
			if (UserConfig.Accounts.Count() > 1)
			{
				(await App.ClickOnElement(Sut.BackToMyAccountsLink)).CurrentPageAs<AccountSelectionPage>();
				return;
			}

			(await App.ClickOnElement(Sut.BackToMyAccountsLink)).CurrentPageAs<AccountSelectionPage>();
		}
	}
}