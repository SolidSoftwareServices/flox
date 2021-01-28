using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;
using OpenQA.Selenium;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects
{
    class GasAccountSetupPage
    {
        protected IWebDriver driver { get; set; }
        internal GasAccountSetupPage(ResidentialPortalWebDriver driver0) : this(driver0.Instance) { }
		SharedPageFunctions page => new SharedPageFunctions(driver);

        internal GasAccountSetupPage(IWebDriver driver0)
        {
            driver = driver0;
        }
        internal void AssertGasAccountSetupPage()
        {
            AssertGasAccountSetupHeader();
            AssertGasAccountSetupText();
            AssertGPRNFieldDisplayed();
            AssertGasMeterReadingFieldDisplayed();
            AssertConfirmDetailsCheckboxDisplayed();
            AssertTermsCheckboxDisplayed();
            AssertDebtFlaggingCheckboxDisplayed();
            AssertPriceTermsCheckboxDisplayed();
            AssertSubmitBtnDisplayed();
            AssertCancelBtnDisplayed();
        }
        internal void EnterGPRN(string v)
        {
            page.SendElementKeys(By.Id(IdentifierSelector.GasAccountSetupPage.gprnTextFieldID),v);
        }
        internal void EnterMeterReading(string v)
        { 
            page.SendElementKeys(By.Id(IdentifierSelector.GasAccountSetupPage.gasMeterReadingTextFieldID), v);
        }
        internal void ClickDetailsCheckbox(string v)
        {
            if (v == "True")
            {
                page.ClickElement(By.CssSelector(CSSLocator.GasAccountSetupPage.detailsCheckbox));
            }
        }
        private void AssertCancelBtnDisplayed()
        {
            page.IsElementPresent(By.Id(IdentifierSelector.GasAccountSetupPage.cancelBtnID));
        }
        internal void ClickGeneralTermsCheckbox(string v)
        {
            if (v == "True")
            {
                page.ClickElement(By.CssSelector(CSSLocator.GasAccountSetupPage.termsCheckbox));
            }
        }

        private void AssertSubmitBtnDisplayed()
        {
            page.IsElementPresent(By.Id(IdentifierSelector.GasAccountSetupPage.submitBtnID));
        }
        internal void ClickDebtFlagCheckbox(string v)
        {
            if (v == "True")
            {
                page.ClickElement(By.CssSelector(CSSLocator.GasAccountSetupPage.debtCheckbox));
            }
        }
        internal void ClickSubmitBtn()
        {
            page.ClickElement(By.Id(IdentifierSelector.GasAccountSetupPage.submitBtnID));
        }
        private void AssertPriceTermsCheckboxDisplayed()
        {
            page.IsElementPresent(By.Id(IdentifierSelector.GasAccountSetupPage.priceTAndCsCheckboxID));
        }
        internal void ClickPricePlanCheckbox(string v)
        {
            if (v == "True")
            {
                page.ClickElement(By.CssSelector(CSSLocator.GasAccountSetupPage.pricePlanCheckbox));
            }
        }
        private void AssertDebtFlaggingCheckboxDisplayed()
        {
            page.IsElementPresent(By.Id(IdentifierSelector.GasAccountSetupPage.debtFlagCheckboxID));
        }
        private void AssertTermsCheckboxDisplayed()
        {
            page.IsElementPresent(By.Id(IdentifierSelector.GasAccountSetupPage.termsCheckboxID));
        }
        private void AssertConfirmDetailsCheckboxDisplayed()
        {
            page.IsElementPresent(By.Id(IdentifierSelector.GasAccountSetupPage.detailsCheckboxID));
        }
        private void AssertGasMeterReadingFieldDisplayed()
        {
            page.IsElementPresent(By.Id(IdentifierSelector.GasAccountSetupPage.gasMeterReadingTextFieldID));
        }
        private void AssertGPRNFieldDisplayed()
        {
            page.IsElementPresent(By.Id(IdentifierSelector.GasAccountSetupPage.gprnTextFieldID));
        }
        private void AssertGasAccountSetupText()
        {
            page.IsElementPresent(page.ByDataAttribute(val: "add-gas-consumption-details-description"));
            page.IsElementPresent(page.ByDataAttribute(val: "privacy-notice-message-component-link"));
        }
        private void AssertGasAccountSetupHeader()
        {
            page.IsElementPresent(page.ByDataAttribute(val: "add-gas-consumption-details-title"));
        }
    }
}
