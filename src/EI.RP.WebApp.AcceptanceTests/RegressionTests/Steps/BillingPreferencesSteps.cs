using System;
using System.Threading;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.StepDefinitions;
using EI.RP.WebApp.AcceptanceTests.Infrastructure;
using EI.RP.WebApp.RegressionTests.PageObjects;
using EI.RP.WebApp.UITestAutomation;
using types = EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils.AccountTypes.types;
namespace EI.RP.WebApp.UITestAutomation
{
    public class BillingPreferencesSteps : BaseStep
    {
        string validMeter = "1234";

        public BillingPreferencesSteps(SingleTestContext shared) : base(shared)
        { }

        PlanPage planPage => new PlanPage(shared.Driver.Value);

        private DirectDebitSettingsPage directDebitSettingsPage => new DirectDebitSettingsPage(shared.Driver.Value);
        public void WhenEnterIntoTheIBANField(string p0)
        {
            directDebitSettingsPage.EnterIBAN(p0);
        }        

        public void WhenEnterNameIntoTheNameOnBankAccountField(string p0)
        {
            directDebitSettingsPage.EnterNameBankAccount(p0);
        }
        
        
        public void ThenConfirmationScreenShouldBeDisplayed()
        {
            EditDirectDebitConfirmationPage editDirectDebitConfirmationPage = new EditDirectDebitConfirmationPage(shared.Driver.Value);
            editDirectDebitConfirmationPage.AssertConfirmationScreenSuccess();
        }
        
        public void WhenTickTermsAndConditionsOnDirectDebitSettingsPage()
        {
            directDebitSettingsPage.ClickTermsAndConditions();
        }
        
        public void WhenClickEditDirectDebitOnPlanPage()
        {
            planPage.ClickEditDirectDebit();
            Thread.Sleep(TimeSpan.FromSeconds(2));
            directDebitSettingsPage.AssertPageText();
            directDebitSettingsPage.AssertMandateInfoDisplayed();
            directDebitSettingsPage.AssertForm();
        }

        public void WhenClickUpdateDetailsBtn()
        {
            directDebitSettingsPage.ClickUpdateDetailsBtn();
        }

        internal void ThenErrorIBAN()
        {
            directDebitSettingsPage.AssertErrorIBAN();
        }

        internal void ThenErrorName()
        {
            directDebitSettingsPage.AssertErrorName();
        }

        internal void ThenErrorTerms()
        {
            directDebitSettingsPage.AssertErrorTerms();
        }
    }
}
