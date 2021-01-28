using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;

namespace EI.RP.WebApp.RegressionTests.PageObjects
{
    class GasAccountConfirmAddressPage
    {
        protected IWebDriver driver { get; set; }
        WebDriverWait wait;
        internal GasAccountConfirmAddressPage(ResidentialPortalWebDriver driver0) : this(driver0.Instance) { }
		protected SharedPageFunctions page => new SharedPageFunctions(driver);

        internal GasAccountConfirmAddressPage(IWebDriver driver0)
        {
            driver = driver0;
        }
        internal void AssertGasAccountConfirmAddressPage()
        {
            AssertConfirmAddressHeaderDisplayed();
            AssertConfirmAddressTextDisplayed();
            AssertGPRNDisplayed();
            AssertAddressDisplayed();
            AssertYesBtnDisplayed();
            AssertNoBtnDisplayed();
            AssertHelpDisplayed();
        }
        private void AssertHelpDisplayed()
        {
            SharedPageFunctions page = new SharedPageFunctions(driver);
            page.IsElementPresent(By.Id(IdentifierSelector.GasSetupConfirmAddressPage.helpID));
        }
        private void AssertNoBtnDisplayed()
        {
            SharedPageFunctions page = new SharedPageFunctions(driver);
            page.IsElementPresent(By.Id(IdentifierSelector.GasSetupConfirmAddressPage.noID));
        }
        private void AssertYesBtnDisplayed()
        {
            SharedPageFunctions page = new SharedPageFunctions(driver);
            page.IsElementPresent(By.Id(IdentifierSelector.GasSetupConfirmAddressPage.yesID));
        }
        private void AssertAddressDisplayed()
        {
            SharedPageFunctions page = new SharedPageFunctions(driver);
            page.IsElementPresent(By.Id(IdentifierSelector.GasSetupConfirmAddressPage.addressID));
        }
        private void AssertGPRNDisplayed()
        {
            SharedPageFunctions page = new SharedPageFunctions(driver);
            page.IsElementPresent(By.Id(IdentifierSelector.GasSetupConfirmAddressPage.gprnID));
        }
        private void AssertConfirmAddressTextDisplayed()
        {
            SharedPageFunctions page = new SharedPageFunctions(driver);
            page.IsElementPresent(By.XPath("//*[@id='address_container']/p[1]"));
        }
        private void AssertConfirmAddressHeaderDisplayed()
        {
            SharedPageFunctions page = new SharedPageFunctions(driver);
            page.IsElementPresent(By.Id(IdentifierSelector.GasSetupConfirmAddressPage.confirmAddressHeaderID));
        }
        internal void ClickYesBtn()
        {
            SharedPageFunctions page = new SharedPageFunctions(driver);
            page.ClickElement(By.Id(IdentifierSelector.GasSetupConfirmAddressPage.yesID));
        }
    }
}
