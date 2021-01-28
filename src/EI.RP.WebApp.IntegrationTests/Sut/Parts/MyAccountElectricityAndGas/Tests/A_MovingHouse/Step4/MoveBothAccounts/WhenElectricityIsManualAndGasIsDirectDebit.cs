using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step4.MoveBothAccounts
{
	class WhenElectricityIsManualAndGasIsDirectDebit : WhenMovingBothAccounts
	{
		protected override PaymentMethodType ScenarioElectricityPaymentMethodType { get; } = PaymentMethodType.Manual;
		protected override PaymentMethodType ScenarioGasPaymentMethodType { get; } = PaymentMethodType.DirectDebit;

		protected override bool IsPRNDegistered { get; } = true;

		[Test]
		public virtual async Task HandlesUserPath_NEWDD_USE_INDIVIDUAL_DD_SKIP_then_Cancel_ShowsStep4()
		{
			Sut.UseSameNewDirectDebitForAllAccountsCheckBox.IsChecked = false;

			var inputPage = (await Sut.SelectNewDirectDebit()).CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();

			AssertDirectDebitInputFieldsAreBlank(inputPage);
			inputPage.AssertCancelDirectDebitAndSkipSetupLinks(shouldShowCancelLink: false, shouldShowSkipLink: true);

			inputPage = (await inputPage.Skip()).CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();

			AssertGasAccountHeader(inputPage);

			inputPage.InputFormValues("IE07BOFI90159756578872", accountName: Fixture.Create<string>());

			inputPage.AssertCancelDirectDebitAndSkipSetupLinks(shouldShowCancelLink: true, shouldShowSkipLink: false);

			var choosePaymentOptionsPage = (await inputPage.CancelDirectDebitSetup()).CurrentPageAs<ChoosePaymentOptionsPage>();
		}

		[Test]
		public override async Task HandlesUserPath_USE_EXISTING_USE_INDIVIDUAL_DD_ShowsStep5WithCorrectInfo()
		{
			Sut.CheckBoxUseExistingDDSetupForAllAccounts.IsChecked = false;
			Sut.ConfirmDetailsAreCorrectExistingDirectDebitCheckBox.IsChecked = true;

			await Sut.SelectExistingDirectDebit();

			var inputPage = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();

			AssertElectricityAccountHeader(inputPage);
			AssertDirectDebitInputFieldsArePopulated(inputPage, UserConfig.GasAccounts().First());
			inputPage.AssertCancelDirectDebitAndSkipSetupLinks(shouldShowCancelLink: false, shouldShowSkipLink: true);

			var gasAccount = UserConfig.GasAccounts().Single();
			var gasAccountName = "Mrs. Gas Account";
			var ibanValue = "IE03AIBK93504210987045";
			inputPage.Iban.Value = ibanValue;
			inputPage.InputFormValues(ibanValue, gasAccountName, true);

			var step5 = (await inputPage.Complete()).CurrentPageAs<Step5ReviewAndCompletePage>();
			AssertStep5Review(step5);
			Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains("Direct Debit"));
			Assert.IsTrue(step5.ShowPayments.SecondaryPaymentType.TextContent.Contains("Direct Debit"));
			Assert.IsTrue(step5.ShowPayments.SecondaryPaymentType.TextContent.Contains(gasAccount.Model.IncomingBankAccount.IBAN.Substring(gasAccount.Model.IncomingBankAccount.IBAN.Length - 4)));
			Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains(ibanValue.Substring(ibanValue.Length - 4)));
			Assert.IsTrue(
				step5.ShowPricePlan.PricePlanText.TextContent.Contains("Your price plan and savings will reflect your details displayed above."));
		}
	}
}