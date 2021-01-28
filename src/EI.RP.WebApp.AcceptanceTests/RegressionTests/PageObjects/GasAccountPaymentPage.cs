using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;

namespace EI.RP.WebApp.RegressionTests.PageObjects
{
    class GasAccountPaymentPage
    {
        protected IWebDriver driver { get; set; }
        WebDriverWait wait;

        protected SharedPageFunctions page => new SharedPageFunctions(driver);
        internal GasAccountPaymentPage(ResidentialPortalWebDriver driver0) : this(driver0.Instance) { }
		internal GasAccountPaymentPage(IWebDriver driver0)
        {
            driver = driver0;
        }

        internal void AssertGasAccountPaymentPage()
        {
            AssertGasAccountPaymentHeader();
            AssertUseExistingDDHeader();
            AssertSetUpNewDDHeader();
            AssertUseExistingDDSubText();
            AssertSetUpNewDDSubText();
            AssertUseExistingDDCheckBox();
            AssertUseExistingDirectDebitBtn();
            AssertSetUpNewDirectDebitBtn();
            AssertSkipCompleteSetupBtn();
        }
        private void AssertGasAccountPaymentHeader()
        {
            page.IsElementPresent(By.Id(IdentifierSelector.GasAccountPaymentPage.gasAccPaymentHeaderID));
        }
        private void AssertUseExistingDDHeader()
        {
            page.IsElementPresent(By.Id(IdentifierSelector.GasAccountPaymentPage.useExistingDDHeaderID));
        }
        private void AssertSetUpNewDDHeader()
        {
            page.IsElementPresent(By.Id(IdentifierSelector.GasAccountPaymentPage.setUpNewDDHeaderID));
        }
        private void AssertUseExistingDDSubText()
        {
            page.IsElementPresent(By.Id(IdentifierSelector.GasAccountPaymentPage.useExistingDDSubTextID));
        }
        private void AssertSetUpNewDDSubText()
        {
            page.IsElementPresent(By.Id(IdentifierSelector.GasAccountPaymentPage.setUpNewDDSubTextID));
        }
        private void AssertUseExistingDDCheckBox()
        {
            page.IsElementPresent(By.Id(IdentifierSelector.GasAccountPaymentPage.useExistingDDCheckBoxTextID));
        }
        private void AssertUseExistingDirectDebitBtn()
        {
            page.IsElementPresent(By.Id(IdentifierSelector.GasAccountPaymentPage.useExistingDirectDebitBtnID));
        }

        private void AssertSetUpNewDirectDebitBtn()
        {
            page.IsElementPresent(By.Id(IdentifierSelector.GasAccountPaymentPage.setUpNewDirectDebitBtnID));
        }
        private void AssertSkipCompleteSetupBtn()
        {
            page.IsElementPresent(By.Id(IdentifierSelector.GasAccountPaymentPage.skipCompleteSetUpBtnID));
        }
        internal void GasAccPaymentTickUseExistingDDCheckbox()
        {
            page.ClickElement(By.Id(IdentifierSelector.GasAccountPaymentPage.useExistingDDCheckBoxTextID));
        }
        internal void GasAccPaymentClickGoBackBtn()
        {
            page.ClickElement(By.Id(IdentifierSelector.GasAccountPaymentPage.goBackBtnID));
        }
    }
}
