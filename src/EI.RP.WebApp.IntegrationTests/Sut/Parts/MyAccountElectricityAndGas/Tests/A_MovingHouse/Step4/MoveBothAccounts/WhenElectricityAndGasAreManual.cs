using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step4.MoveBothAccounts
{
    class WhenElectricityAndGasAreManual : WhenMovingBothAccounts
    {
        protected override PaymentMethodType ScenarioElectricityPaymentMethodType { get; } = PaymentMethodType.Manual;
        protected override PaymentMethodType ScenarioGasPaymentMethodType { get; } = PaymentMethodType.Manual;

        protected override bool IsPRNDegistered { get; } = false;

		[Test]
        public async Task HandlesUserPath_Cancel_ThenGoBack_ThenCancel_ThenCancel_ShowsLandingPage()
        {
            await Sut.ClickOnCancelConfirm();
            App.CurrentPageAs<Step0LandingPage>();
        }

        [Test]
        public override async Task HandlesUserPath_NEWDD_USE_SAME_DD_ShowsStep5WithCorrectInfo()
        {
            var electricityAccountInfo = UserConfig.Accounts.ElementAt(1);
            var gasAccountInfo = UserConfig.Accounts.ElementAt(0);

            Sut.UseSameNewDirectDebitForAllAccountsCheckBox.IsChecked = true;
            await Sut.SelectNewDirectDebit();

            var inputPage = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
            Assert.AreEqual(
                $"{electricityAccountInfo.ClientAccountType} ({electricityAccountInfo.AccountNumber}) & {gasAccountInfo.ClientAccountType} ({gasAccountInfo.AccountNumber})",
                inputPage.HeaderElement.TextContent);

			inputPage.AssertCancelDirectDebitAndSkipSetupLinks(shouldShowCancelLink: false, shouldShowSkipLink: true);
			AssertDirectDebitInputFieldsAreBlank(inputPage);

			var ibanValue = "IE03AIBK93504210987045";
			inputPage.InputFormValues(ibanValue, AccountName);
            await inputPage.Complete();

            var step5 = App.CurrentPageAs<Step5ReviewAndCompletePage>();

            AssertStep5Review(step5);
            Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains("Direct Debit"));
            Assert.IsTrue(step5.ShowPayments.SecondaryPaymentType.TextContent.Contains("Direct Debit"));
			Assert.IsTrue(step5.ShowPayments.SecondaryPaymentType.TextContent.Contains(ibanValue.Substring(ibanValue.Length - 4)));
			Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains(ibanValue.Substring(ibanValue.Length - 4)));

			Assert.IsTrue(
				step5.ShowPricePlan.PricePlanText.TextContent.Contains("Your price plan and savings will reflect your details displayed above."));
		}

        [Test]
        public override async Task HandlesUserPath_NEWDD_USE_INDIVIDUAL_DD_ShowsStep5WithCorrectInfo()
        {
            var electricityAccountInfo = UserConfig.Accounts.ElementAt(1);
            var gasAccountInfo = UserConfig.Accounts.ElementAt(0);

            Sut.UseSameNewDirectDebitForAllAccountsCheckBox.IsChecked = false;
            await Sut.SelectNewDirectDebit();

            var inputPage = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			AssertDirectDebitInputFieldsAreBlank(inputPage);
			Assert.AreEqual($"{electricityAccountInfo.ClientAccountType} ({electricityAccountInfo.AccountNumber})",
                inputPage.HeaderElement.TextContent);
            Assert.IsNotNull(inputPage.SkipElement);
            var ibanValue = "IE03AIBK93504210987045";
            inputPage.InputFormValues(ibanValue, AccountName);
            await inputPage.Complete();

            inputPage = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			AssertDirectDebitInputFieldsAreBlank(inputPage);
			Assert.AreEqual($"{gasAccountInfo.ClientAccountType} ({gasAccountInfo.AccountNumber})",
                inputPage.HeaderElement.TextContent);
            Assert.IsNotNull(inputPage.SkipElement);
            var ibanValue2 = "IE07BOFI90159756578872";
            inputPage.InputFormValues(ibanValue2, AccountName2);
            await inputPage.Complete();

            var step5 = App.CurrentPageAs<Step5ReviewAndCompletePage>();

            AssertStep5Review(step5);
            Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains("Direct Debit"));
            Assert.IsTrue(step5.ShowPayments.SecondaryPaymentType.TextContent.Contains("Direct Debit"));
            Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains(ibanValue.Substring(ibanValue.Length - 4)));
			Assert.IsTrue(step5.ShowPayments.SecondaryPaymentType.TextContent.Contains(ibanValue2.Substring(ibanValue2.Length - 4)));
			Assert.IsTrue(
				step5.ShowPricePlan.PricePlanText.TextContent.Contains("Your price plan and savings will reflect your details displayed above."));
		}

        [Test]
        public override async Task
            HandlesUserPath_NEWDD_USE_INDIVIDUAL_DD_SKIP_then_COMPLETE_ShowsStep5WithCorrectInfo()
        {
            var electricityAccountInfo = UserConfig.Accounts.ElementAt(1);
            var gasAccountInfo = UserConfig.Accounts.ElementAt(0);

            Sut.UseSameNewDirectDebitForAllAccountsCheckBox.IsChecked = false;
            await Sut.SelectNewDirectDebit();

            var inputPage = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
            Assert.IsNotNull(inputPage.SkipElement);
            await inputPage.Skip();

            inputPage = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			AssertDirectDebitInputFieldsAreBlank(inputPage);
			Assert.AreEqual($"{gasAccountInfo.ClientAccountType} ({gasAccountInfo.AccountNumber})",
                inputPage.HeaderElement.TextContent);
            Assert.IsNotNull(inputPage.SkipElement);
            var ibanValue2 = "IE07BOFI90159756578872";
            inputPage.InputFormValues(ibanValue2, AccountName2);
            await inputPage.Complete();

            var step5 = App.CurrentPageAs<Step5ReviewAndCompletePage>();
            AssertStep5Review(step5);
            Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains("Manual"));
            Assert.IsTrue(step5.ShowPayments.SecondaryPaymentType.TextContent.Contains("Direct Debit"));

            Assert.IsTrue(step5.ShowPayments.SecondaryPaymentType.TextContent.Contains(ibanValue2.Substring(ibanValue2.Length - 4)));

			Assert.IsTrue(
				step5.ShowPricePlan.PricePlanText.TextContent.Contains("Your price plan and savings will reflect your details displayed above."));

		}

        [Test]
        public async Task HandlesUserPath_NEWDD_USE_INDIVIDUAL_DD_COMPLETE_then_SKIP_ShowsStep5WithCorrectInfo()
        {
            var electricityAccountInfo = UserConfig.Accounts.ElementAt(1);
            var gasAccountInfo = UserConfig.Accounts.ElementAt(0);

            Sut.UseSameNewDirectDebitForAllAccountsCheckBox.IsChecked = false;
            await Sut.SelectNewDirectDebit();

            var inputPage = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			AssertDirectDebitInputFieldsAreBlank(inputPage);
			Assert.AreEqual($"{electricityAccountInfo.ClientAccountType} ({electricityAccountInfo.AccountNumber})",
                inputPage.HeaderElement.TextContent);
            var ibanValue2 = "IE07BOFI90159756578872";
            inputPage.InputFormValues(ibanValue2, AccountName2);
            await inputPage.Complete();

            inputPage = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			AssertDirectDebitInputFieldsAreBlank(inputPage);
			Assert.AreEqual($"{gasAccountInfo.ClientAccountType} ({gasAccountInfo.AccountNumber})",
                inputPage.HeaderElement.TextContent);

            await inputPage.Skip();

            var step5 = App.CurrentPageAs<Step5ReviewAndCompletePage>();

            AssertStep5Review(step5);
            
            Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains("Direct Debit"));

            Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains(ibanValue2.Substring(ibanValue2.Length - 4)));

            Assert.IsTrue(step5.ShowPayments.SecondaryPaymentType.TextContent.Contains("Manual"));

			Assert.IsTrue(
				step5.ShowPricePlan.PricePlanText.TextContent.Contains("Your price plan and savings will reflect your details displayed above."));
		}

        [Test]
        public async Task HandlesUserPath_MANUAL_Confirm_ShowsStep5WithCorrectInfo()
        {
            await Sut.SelectManualPayment();
            var step5 = App.CurrentPageAs<Step5ReviewAndCompletePage>();

            Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains("Manual"));
            Assert.IsTrue(step5.ShowPayments.SecondaryPaymentType.TextContent.Contains("Manual"));

			Assert.IsTrue(
				step5.ShowPricePlan.PricePlanText.TextContent.Contains("Your price plan and savings will reflect your details displayed above."));
		}

        [Test]
        public async Task HandlesUserPath_MANUAL_ThenNEWDD_USE_INDIVIDUAL_DD_COMPLETE_ShowsInputScreen()
        {
            Sut.UseNewSingleSetupForAllAccountsFromManualConfirmDialog.IsChecked = false;
            await Sut.SelectNewDirectDebitFromDialog();
            var inputPage = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			AssertDirectDebitInputFieldsAreBlank(inputPage);
			Assert.IsNotNull(inputPage.SkipElement);
            var gasAccountInfo = UserConfig.Accounts.ElementAt(0);
            var electricityAccountInfo = UserConfig.Accounts.ElementAt(1);
            Assert.AreEqual($"{electricityAccountInfo.ClientAccountType} ({electricityAccountInfo.AccountNumber})",
                inputPage.HeaderElement.TextContent);
            var ibanValue2 = "IE07BOFI90159756578872";
            inputPage.InputFormValues(ibanValue2, AccountName2);
            await inputPage.Complete();

            inputPage = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			AssertDirectDebitInputFieldsAreBlank(inputPage);

			Assert.AreEqual($"{gasAccountInfo.ClientAccountType} ({gasAccountInfo.AccountNumber})",
                inputPage.HeaderElement.TextContent);

            await inputPage.Skip();

            var step5 = App.CurrentPageAs<Step5ReviewAndCompletePage>();

            AssertStep5Review(step5);

            Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains("Direct Debit"));

            Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains(ibanValue2.Substring(ibanValue2.Length - 4)));

            Assert.IsTrue(step5.ShowPayments.SecondaryPaymentType.TextContent.Contains("Manual"));

			Assert.IsTrue(
				step5.ShowPricePlan.PricePlanText.TextContent.Contains("Your price plan and savings will reflect your details displayed above."));
		}

        [Test]
        public async Task HandlesUserPath_MANUAL_ThenNEWDD_USE_SAME_DD_COMPLETE_ShowsInputScreen()
        {
            Sut.UseNewSingleSetupForAllAccountsFromManualConfirmDialog.IsChecked = true;
            await Sut.SelectNewDirectDebitFromDialog();
            var inputPage = App.CurrentPageAs<InputDirectDebitDetailsWhenMovingHomePage>();
			AssertDirectDebitInputFieldsAreBlank(inputPage);
			Assert.IsNotNull(inputPage.SkipElement);
            var gasAccountInfo = UserConfig.Accounts.ElementAt(0);
            var electricityAccountInfo = UserConfig.Accounts.ElementAt(1);
            Assert.AreEqual(
                $"{electricityAccountInfo.ClientAccountType} ({electricityAccountInfo.AccountNumber}) & {gasAccountInfo.ClientAccountType} ({gasAccountInfo.AccountNumber})",
                inputPage.HeaderElement.TextContent);
            Assert.IsNotNull(inputPage.SkipElement);
            var ibanValue2 = "IE07BOFI90159756578872";
            inputPage.InputFormValues(ibanValue2, AccountName2);
            await inputPage.Complete();

            var step5 = App.CurrentPageAs<Step5ReviewAndCompletePage>();

            AssertStep5Review(step5);

            Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains("Direct Debit"));

            Assert.IsTrue(step5.ShowPayments.PrimaryPaymentType.TextContent.Contains(ibanValue2.Substring(ibanValue2.Length - 4)));

			Assert.IsTrue(step5.ShowPayments.SecondaryPaymentType.TextContent.Contains("Direct Debit"));

			Assert.IsTrue(step5.ShowPayments.SecondaryPaymentType.TextContent.Contains(ibanValue2.Substring(ibanValue2.Length - 4)));

			Assert.IsTrue(
				step5.ShowPricePlan.PricePlanText.TextContent.Contains("Your price plan and savings will reflect your details displayed above."));
		}

        [Ignore("NotApplicable")]
        public override async Task HandlesUserPath_USE_EXISTING_USE_INDIVIDUAL_DD_ShowsStep5WithCorrectInfo()
        {
        }

        [Ignore("NotApplicable")]
        public override async Task HandlesUserPath_USE_EXISTING_USE_SAME_DD_ShowsStep5WithCorrectInfo()
        {
        }

        [Ignore("NotApplicable")]
        public override async Task
            HandlesUserPath_NEWDD_Use_Individual_DD_Complete_Cancel_New_Complete_Complete_ShowsStep5WithCorrectInfo()
        {
        }

        [Ignore("NotApplicable")]
        [Test]
        public override async Task
            HandlesUserPath_NEWDD_Use_Same_CancelSetup_Use_Individual_DD_Complete_ShowsCorrectHeaders()
        {
        }
    }
}