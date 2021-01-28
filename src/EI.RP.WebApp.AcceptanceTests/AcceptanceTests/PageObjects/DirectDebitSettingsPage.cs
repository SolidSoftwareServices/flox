using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Selenium;
using OpenQA.Selenium;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects
{
    class DirectDebitSettingsPage
    {
        protected IWebDriver driver { get; set; }
        internal DirectDebitSettingsPage(ResidentialPortalWebDriver driver0) : this(driver0.Instance) { }
		internal DirectDebitSettingsPage(IWebDriver driver0)
        {
            driver = driver0;
        }

        internal SharedPageFunctions page => new SharedPageFunctions(driver);

        internal string IBANInputFieldID = "iban",
                   bankNameInputFieldID = "customer-name",
                   termsAndConditionsID = "terms",
                   updateDetailsBtnID = "dds_update_details_btn",
                   cancelID = "dds_cancel_btn",
                   mandateID = "dds_mandate",
                   downloadMandateID = "dds_mandate_download";

        internal void EnterIBAN(string p0)
        {
            driver.FindElementEx(By.Id(IBANInputFieldID)).Clear();
            page.SendElementKeys(By.Id(IBANInputFieldID), p0);
        }

        internal void AssertDirectDebitSettingsPage()
        {
            page.IsElementPresent(By.XPath("//h2[contains(text(),'Direct Debit Settings')]"));
        }

        internal void AssertPageHeaderDisplayed()
        {
            page.IsElementPresent(page.ByDataAttribute(val: "dd-settings-title"));
        }

        internal void AssertPageText()
        {
            page.IsElementPresent(page.ByDataAttribute(val: "dd-settings-description"));
            page.IsElementPresent(page.ByDataAttribute(val: "privacy-notice-message-component-link"));
        }

        internal void AssertMandateInfoDisplayed()
        {
            page.IsElementPresent(page.ByDataAttribute(val: "mandate-info-line-1"));
            page.IsElementPresent(page.ByDataAttribute(val: "mandate-info-line-2"));
        }

        internal void ClickUpdateDetailsBtn()
        {
            page.ClickElement(By.Id(updateDetailsBtnID));
        }
        internal void EnterNameBankAccount(string p0)
        {
            page.SendElementKeys(By.Id(bankNameInputFieldID), p0);
        }
        internal void AssertForm()
        {
            AssertIBANInputFieldDisplayed();
            AssertNameOnBankInputFieldDisplayed();
            AssertTermsAndConditionsDisplayed();
            AssertUpdateDetailsBtnDisplayed();
            AssertCancelDisplayed();
        }

        internal void ClickTermsAndConditions()
        {
            page.ClickElement(By.Id("terms-label"));
        }

        private void AssertCancelDisplayed()
        {
            page.IsElementPresent(page.ByDataAttribute(val: "dd-settings-secondary-button"));
        }
        private void AssertUpdateDetailsBtnDisplayed()
        {
            page.IsElementPresent(page.ByDataAttribute(val: "dd-settings-primary-button"));
        }

        private void AssertTermsAndConditionsDisplayed()
        {
            page.IsElementPresent(By.Id(termsAndConditionsID));
        }

        private void AssertNameOnBankInputFieldDisplayed()
        {
            page.IsElementPresent(By.Id(bankNameInputFieldID));
        }

        private void AssertIBANInputFieldDisplayed()
        {
            page.IsElementPresent(By.Id(IBANInputFieldID));
        }
    }
}
