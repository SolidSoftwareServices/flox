using System;
using System.Threading;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects;
using EI.RP.WebApp.AcceptanceTests.Infrastructure;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.StepDefinitions
{
    public class BillsAndPaymentsSteps : BaseStep
    {
        public BillsAndPaymentsSteps(SingleTestContext shared) : base(shared)
        {
        }
        private BillsAndPaymentsPage billsAndPaymentsPage => new BillsAndPaymentsPage(shared.Driver.Value);
        private PlanPage planPage => new PlanPage(shared.Driver.Value);
        private DirectDebitSettingsPage directDebitSettingsPage => new DirectDebitSettingsPage(shared.Driver.Value);
        private EditDirectDebitConfirmationPage editDirectDebitConfirmationPage => new EditDirectDebitConfirmationPage(shared.Driver.Value);
        public void WhenEnterIntoTheIBANField(string p0)
        {
            directDebitSettingsPage.EnterIBAN(p0);
        }

        public void WhenClickBillingPreferencesButton()
        {
            billsAndPaymentsPage.ClickBillingPreferencesBtn();
            planPage.AssertPaymentMethodDisplayed();
            planPage.AssertPaperlessBillingDisplayed();
        }
        public void WhenEnterNameIntoTheNameOnBankAccountField(string p0)
        {
            directDebitSettingsPage.EnterNameBankAccount(p0);
        }
        internal void WhenClickPayNow()
        {
            billsAndPaymentsPage.ClickPayNow();     
        }

        public void ThenConfirmationScreenShouldBeDisplayed()
        {
            editDirectDebitConfirmationPage.AssertConfirmationScreenSuccess();
        }

        public void WhenTickTermsAndConditionsOnDirectDebitSettingsPage()
        {
            directDebitSettingsPage.ClickTermsAndConditions();
        }

        public void WhenClickEditDirectDebitOnBillingAndPaymentsOptionsPage()
        {
            planPage.ClickEditDirectDebit();

            directDebitSettingsPage.AssertPageHeaderDisplayed();
            directDebitSettingsPage.AssertPageText();
            directDebitSettingsPage.AssertMandateInfoDisplayed();
            directDebitSettingsPage.AssertForm();
        }

        public void WhenClickUpdateDetailsBtn()
        {
            directDebitSettingsPage.ClickUpdateDetailsBtn();
        }
    }
}
