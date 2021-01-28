using System;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AutoFixture;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Commands.Banking.DirectDebit.SetUpDirectDebit;
using EI.RP.DomainServices.Commands.Contracts.AddAdditionalAccount.Gas;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration.DirectDebit;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AddGasAccount;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.Plan;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.AddGasAccount
{
	[TestFixture]
	internal class WhenInChoosePaymentOptionsPageTest : MyAccountCommonTests<ChoosePaymentOptionsPage>
	{
		protected override async Task TestScenarioArrangement()
		{
			UserConfig = App.ConfigureUser("a@A.com", "test");
			UserConfig.AddElectricityAccount(canAddNewAccount: true);
			UserConfig.Execute();

			var app = await ((ResidentialPortalApp) await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
			app = await app.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMeterReading();
			app = await app.CurrentPageAs<MyAccountElectricityAndGasPage>().ToPlanPage();

			var inputConsumptionDetailsPage = SetInput((await app.ClickOnElement(app.CurrentPageAs<PlanPage>().AddGasFlow)).CurrentPageAs<CollectAccountConsumptionDetailsPage>());
			var confirmAddressPage =
				(await inputConsumptionDetailsPage.ClickOnElement(inputConsumptionDetailsPage.SubmitButton))
				.CurrentPageAs<ConfirmAddressPage>();
			Sut = (await confirmAddressPage.ClickOnElement(confirmAddressPage.ConfirmAddressButton))
				.CurrentPageAs<ChoosePaymentOptionsPage>();

			CollectAccountConsumptionDetailsPage SetInput(CollectAccountConsumptionDetailsPage consumptionDetailsPage)
			{
				consumptionDetailsPage.GPRN.Value = (string)_gprnValue;
				consumptionDetailsPage.GasMeterReading.Value = _meterReadingVal;
				consumptionDetailsPage.AuthorizationCheck.IsChecked = true;
				consumptionDetailsPage.TermsAndConditionsAccepted.IsChecked = true;
				consumptionDetailsPage.DebtFlagAndArrearsTermsAndConditions.IsChecked = true;
				consumptionDetailsPage.PricePlanTermsAndConditions.IsChecked = true;
				return consumptionDetailsPage;
			}
		}

		private readonly GasPointReferenceNumber _gprnValue = Fixture.Create<GasPointReferenceNumber>();
		private readonly string _meterReadingVal = new Random((int)DateTime.UtcNow.Ticks).Next(100000, 600000).ToString();
		[Test]
		public async Task AndWhenClickingGoBack_Then_ShowsInputPage_And_NoCommandWasCalled()
		{
			var actual = (await Sut.ClickOnElement(Sut.GoBack)).CurrentPageAs<CollectAccountConsumptionDetailsPage>();
			Assert.IsNotNull(actual, "this cannot happen");
			Assert.IsTrue(string.IsNullOrWhiteSpace(actual.GPRN.Value));
			Assert.IsTrue(string.IsNullOrWhiteSpace(actual.GasMeterReading.Value));
			App.DomainFacade.CommandDispatcher.AssertCommandWasNotExecuted<AddGasAccountCommand>();
			App.DomainFacade.CommandDispatcher.AssertCommandWasNotExecuted<SetUpDirectDebitDomainCommand>();
		}

		[Test]
		public async Task AndWhenClickingSkipAndComplete_Then_ShowsConfirmationPage_And_Account_IsCreated()
		{
			var button = Sut.SkipAndCompleteButton;

			await ManualPaymentTest(button);
		}

		[Test]
		public async Task AndWhenClickingManualPayment_Then_ShowsConfirmationPage_And_Account_IsCreated()
		{
			var button = Sut.ManualPaymentsButton;

			await ManualPaymentTest(button);
		}
		private async Task ManualPaymentTest(IHtmlElement button)
		{
			var accountInfo = UserConfig.Accounts.Single();
			var addGasAccountCommand = new AddGasAccountCommand(_gprnValue, accountInfo.AccountNumber,
				decimal.Parse(_meterReadingVal), PaymentSetUpType.Manual, null, null);

			var gasAccountStub = new AccountInfo
			{
				ClientAccountType = ClientAccountType.Gas,
				AccountNumber = Fixture.Create<string>()
			};
			App.DomainFacade.QueryResolver.ExpectQuery(new AccountInfoQuery
			{
				Prn = _gprnValue
			}, gasAccountStub.ToOneItemArray());
			var btnSkipAndComplete = await Sut.ClickOnElement(button);
			var actual = btnSkipAndComplete.CurrentPageAs<AddGasDirectDebitPageConfirmation>();

			App.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(addGasAccountCommand);
			App.DomainFacade.CommandDispatcher.AssertCommandWasNotExecuted<SetUpDirectDebitDomainCommand>();
		}
		[Test]
		public async Task AndWhenClickingSetUpDirectDebit_Then_ShowsConfirmationPage_And_Account_AndDD_AreCreated()
		{
			var accountInfo = UserConfig.Accounts.Single();
			
			var gasAccountStub = new AccountInfo
			{
				ClientAccountType = ClientAccountType.Gas,
				AccountNumber = Fixture.Create<string>()
			};
			App.DomainFacade.QueryResolver.ExpectQuery(new AccountInfoQuery
			{
				Prn = _gprnValue
			}, gasAccountStub.ToOneItemArray());
			var clickOnElement = await Sut.ClickOnElement(Sut.SetUpNewDirectDebitButton);
			var inputDirectDebitPage=App.CurrentPageAs<ConfigureDirectDebitPage>();
			var incomingBankAccount = UserConfig.ElectricityAndGasAccountConfigurators.Single().Model.IncomingBankAccount;
			var ibanValue = "IE65AIBK93104715784037";
			inputDirectDebitPage.Iban.Value = ibanValue;
			var customerNameValue = "Mr. Firstname LastName";
			inputDirectDebitPage.CustomerName.Value = customerNameValue;
			inputDirectDebitPage.ConfirmTermsBox.IsChecked = true;
			(await inputDirectDebitPage.ClickOnElement(inputDirectDebitPage.UpdateDetailsButton)).CurrentPageAs<AddGasDirectDebitPageConfirmation>();
			var addGasAccountCommand = new AddGasAccountCommand(_gprnValue, accountInfo.AccountNumber,
				decimal.Parse(_meterReadingVal), PaymentSetUpType.SetUpNewDirectDebit, ibanValue, customerNameValue);

			App.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(addGasAccountCommand);
			App.DomainFacade.CommandDispatcher.AssertCommandWasNotExecuted(new SetUpDirectDebitDomainCommand(gasAccountStub.AccountNumber,customerNameValue,null,ibanValue,accountInfo.Partner,ClientAccountType.Gas,PaymentMethodType.DirectDebit,null,null,null));
		}

		[Test]
		public async Task WhenClickingSetUpDirectDebitAndElectricIrelandIbanUsed_Then_ShowsInputPage()
		{
			var accountInfo = UserConfig.Accounts.Single();

			var gasAccountStub = new AccountInfo
			{
				ClientAccountType = ClientAccountType.Gas,
				AccountNumber = Fixture.Create<string>()
			};
			App.DomainFacade.QueryResolver.ExpectQuery(new AccountInfoQuery
			{
				Prn = _gprnValue
			}, gasAccountStub.ToOneItemArray());
			var clickOnElement = await Sut.ClickOnElement(Sut.SetUpNewDirectDebitButton);
			var inputDirectDebitPage = App.CurrentPageAs<ConfigureDirectDebitPage>();
			var incomingBankAccount = UserConfig.ElectricityAndGasAccountConfigurators.Single().Model.IncomingBankAccount;
			var ibanValue = "IE15AIBK93208681900756";
			inputDirectDebitPage.Iban.Value = ibanValue;
			var customerNameValue = "Mr. Firstname LastName";
			inputDirectDebitPage.CustomerName.Value = customerNameValue;
			inputDirectDebitPage.ConfirmTermsBox.IsChecked = true;
			(await inputDirectDebitPage.ClickOnElement(inputDirectDebitPage.UpdateDetailsButton)).CurrentPageAs<ConfigureDirectDebitPage>();
			var addGasAccountCommand = new AddGasAccountCommand(_gprnValue, accountInfo.AccountNumber,
				decimal.Parse(_meterReadingVal), PaymentSetUpType.SetUpNewDirectDebit, ibanValue, customerNameValue);

			App.DomainFacade.CommandDispatcher.AssertCommandWasNotExecuted(addGasAccountCommand);
			App.DomainFacade.CommandDispatcher.AssertCommandWasNotExecuted<SetUpDirectDebitDomainCommand>();
		}

		[Test]
		public async Task TheViewShowsTheExpectedInformation()
		{
			Assert.IsNotNull(Sut.SetUpNewDirectDebitButton);
			Assert.AreEqual("Set up new Direct Debit", Sut.SetUpNewDirectDebitButton.TextContent);
			Assert.IsNotNull(Sut.SetUpNewDirectDebitHeader);
			Assert.AreEqual("Set Up New Direct Debit", Sut.SetUpNewDirectDebitHeader.TextContent);
			Assert.IsNotNull(Sut.SetUpNewDirectDebitContent);
			Assert.IsTrue(Sut.SetUpNewDirectDebitContent.TextContent.Contains(
				"Set up a new Direct Debit for your bill payments. You will need your IBAN and the name on your bank account"));

			Assert.IsNotNull(Sut.ManualPaymentsHeader);
			Assert.AreEqual("Manual Bill Payments", Sut.ManualPaymentsHeader.Text());
			Assert.IsNotNull(Sut.ManualPaymentsContent);
			Assert.IsTrue(Sut.ManualPaymentsContent.TextContent.Contains(
				"You have previously paid your bills manually. You can continue to pay your bills as before"));
			Assert.IsNotNull(Sut.GoBack);
			Assert.IsTrue(Sut.GoBack.TextContent.Trim().Contains("Go back"));
			Assert.IsNotNull(Sut.ManualPaymentsContent);
			Assert.IsTrue(Sut.SkipAndCompleteButton.TextContent.Trim().Contains("Skip & complete account set up"));

			var collectAccountInfoPage = (await Sut.ClickOnElement(Sut.GoBack))
				.CurrentPageAs<CollectAccountConsumptionDetailsPage>();
			Assert.IsNotNull(collectAccountInfoPage);
		}
	}
}