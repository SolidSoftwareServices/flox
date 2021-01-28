using OpenQA.Selenium;
using System;
using System.Threading;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;

namespace EI.RP.WebApp.RegressionTests.PageObjects
{
    class DirectDebitSettingsPage
    {
        protected IWebDriver driver { get; set; }
        internal DirectDebitSettingsPage(ResidentialPortalWebDriver driver0) : this(driver0.Instance) { }
		internal DirectDebitSettingsPage(IWebDriver driver0)
        {
            driver = driver0;
        }
        protected SharedPageFunctions page => new SharedPageFunctions(driver);

        internal void EnterIBAN(string p0)
        {
            page.SendElementKeys(By.Id(IdentifierSelector.EditDirectDebitPage.IBANInputFieldID), p0);
        }

        internal void AssertDirectDebitSettingsPage()
        {
            page.IsElementPresent(By.XPath("//h1[contains(text(),'Direct Debit Settings')]"));
        }


        internal void AssertPageText()
        {
            page.IsElementPresent(page.ByDataAttribute(val: IdentifierSelector.EditDirectDebitPage.gdpr));
        }

        internal void AssertMandateInfoDisplayed()
        {
            page.IsElementPresent(page.ByDataAttribute(val: "mandate-info-line-1"));
            page.IsElementPresent(page.ByDataAttribute(val: "mandate-info-line-2"));
        }

        internal void ClickUpdateDetailsBtn()
        {
            page.ClickElement(By.Id(IdentifierSelector.EditDirectDebitPage.updateDetailsBtnID));
        }
        internal void EnterNameBankAccount(string p0)
        {
            page.SendElementKeys(By.Id(IdentifierSelector.EditDirectDebitPage.bankNameInputFieldID), p0);
        }
        internal void AssertForm()
        {
            AssertIBANInputFieldDisplayed();
            AssertNameOnBankInputFieldDisplayed();
            AssertTermsAndConditionsDisplayed();
            AssertUpdateDetailsBtnDisplayed();
            AssertCancelDisplayed();
        }

        internal void AssertErrorTerms()
        {
            page.IsElementTextPresent(TextMatch.DirectDebitSettingsPage.termsAndConditionsError);
        }

        internal void AssertErrorName()
        {
            page.IsElementTextPresent(TextMatch.DirectDebitSettingsPage.nameError);
        }

        internal void AssertErrorIBAN()
        {
            page.IsElementTextPresent(TextMatch.DirectDebitSettingsPage.ibanError);
        }

        internal void ClickTermsAndConditions()
        {
            page.ClickElement(By.Id("terms-label"));
        }

        private void AssertCancelDisplayed()
        {
            page.IsElementPresent(By.Id(IdentifierSelector.EditDirectDebitPage.cancelID));
        }
        private void AssertUpdateDetailsBtnDisplayed()
        {
            page.IsElementPresent(By.Id(IdentifierSelector.EditDirectDebitPage.updateDetailsBtnID));
        }

        private void AssertTermsAndConditionsDisplayed()
        {
            page.IsElementPresent(By.Id(IdentifierSelector.EditDirectDebitPage.termsAndConditionsID));
        }

        private void AssertNameOnBankInputFieldDisplayed()
        {
            page.IsElementPresent(By.Id(IdentifierSelector.EditDirectDebitPage.bankNameInputFieldID));
        }

        private void AssertIBANInputFieldDisplayed()
        {
            page.IsElementPresent(By.Id(IdentifierSelector.EditDirectDebitPage.IBANInputFieldID));
        }
    }
}
