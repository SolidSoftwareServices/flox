using OpenQA.Selenium;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;
using static EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils.SharedVariables;

namespace EI.RP.WebApp.RegressionTests.PageObjects
{
    class GasAccountSetupPage
    {
        private string submitBtnID = "gasAccount";
        private string priceTAndCsCheckboxID = "chkPricePlanTermsAndCondition";
        private string GdprTextID = "privacy-notice-message-component-link";
        private string debtFlagCheckboxID = "chkDebtFlagAndArrearsTermsAndConditions";
        private string termsCheckboxID = "chkTermsAndConditionsAccepted";
        private string detailsCheckboxID = "chkAuthorization";
        private string gasMeterReadingTextFieldID = "GasMeterReading";
        private string gprnTextFieldID = "inputGPRN";
        private string cancelBtnID = "btnCancel";
        protected IWebDriver driver { get; set; }

        SharedPageFunctions page => new SharedPageFunctions(driver);
        internal GasAccountSetupPage(ResidentialPortalWebDriver driver0) : this(driver0.Instance) { }
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
            page.SendElementKeys(By.Id(gprnTextFieldID),v);
        }
        internal void EnterMeterReading(string v)
        {
            page.SendElementKeys(By.Id(gasMeterReadingTextFieldID), v);
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

        internal void ErrorDetails()
        {
            page.IsElementTextPresent(TextMatch.GasAccountSetupPage.ErrorDetails);
        }

        private void AssertSubmitBtnDisplayed()
        {
            page.IsElementPresent(By.Id(submitBtnID));
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
            page.ClickElement(By.Id(submitBtnID));
        }
        private void AssertPriceTermsCheckboxDisplayed()
        {
            page.IsElementPresent(By.Id(priceTAndCsCheckboxID));
        }

        internal void ErrorTerms()
        {
            page.IsElementTextPresent(TextMatch.GasAccountSetupPage.ErrorTerms);
        }

        internal void ClickPricePlanCheckbox(string v)
        {
            if (v == "True")
            {
                page.ClickElement(By.CssSelector(CSSLocator.GasAccountSetupPage.pricePlanCheckbox));
            }
        }

        internal void ErrorDebt()
        {
            page.IsElementTextPresent(TextMatch.GasAccountSetupPage.ErrorDebt);
        }

        internal void ErrorPricePlan()
        {
            page.IsElementTextPresent(TextMatch.GasAccountSetupPage.ErrorPricePlan);
        }

        private void AssertDebtFlaggingCheckboxDisplayed()
        { 
            page.IsElementPresent(By.Id(debtFlagCheckboxID));
        }

        internal void ErrorGPRNAlpha()
        {
            page.IsElementTextPresent(TextMatch.GasAccountSetupPage.ErrorGPRNAlpha);
        }

        private void AssertTermsCheckboxDisplayed()
        {
            page.IsElementPresent(By.Id(termsCheckboxID));
        }

        internal void ErrorMeterReadingAlpha()
        {
            page.IsElementTextPresent(TextMatch.GasAccountSetupPage.ErrorMeterReadingAlpha);
        }

        private void AssertConfirmDetailsCheckboxDisplayed()
        {
            page.IsElementPresent(By.Id(detailsCheckboxID));
        }

        internal void ErrorGPRNLong()
        {
            page.IsElementTextPresent(TextMatch.GasAccountSetupPage.ErrorGPRNLong);
        }

        private void AssertGasMeterReadingFieldDisplayed()
        {
            page.IsElementPresent(By.Id(gasMeterReadingTextFieldID));
        }

        internal void ErrorGPRNShort()
        {
            page.IsElementTextPresent(TextMatch.GasAccountSetupPage.ErrorGPRNShort);
        }

        private void AssertGPRNFieldDisplayed()
        {
            page.IsElementPresent(By.Id(gprnTextFieldID));
        }

        internal void ErrorGPRNEmpty()
        {
            page.IsElementTextPresent(TextMatch.GasAccountSetupPage.ErrorGPRNEmpty);
        }

        private void AssertGasAccountSetupText()
        {
            page.IsElementPresent(By.XPath("/html/body/div[3]/div[2]/main/div/div/p[1]"));
            page.IsElementPresent(By.XPath("/html/body/div[3]/div[2]/main/div/div/p[2]"));
            page.IsElementPresent(page.ByDataAttribute(val: GdprTextID));
        }

        internal void ErrorMeterReadingEmpty()
        {
            page.IsElementTextPresent(TextMatch.GasAccountSetupPage.ErrorMeterReadingEmpty);
        }

        private void AssertGasAccountSetupHeader()
        {
            page.IsElementTextPresent(TextMatch.GasAccountSetupPage.GasAccountSetupHeader);
        }
    }
}
