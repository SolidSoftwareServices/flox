using System;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Selenium;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects
{
    class SmartHubPage
    {
        protected IWebDriver driver { get; set; }
        internal SmartHubPage(ResidentialPortalWebDriver driver0) : this(driver0.Instance) { }
		internal SmartHubPage(IWebDriver driver0)
        {
            driver = driver0;
        }
        WebDriverWait wait;
        internal void ClickSignUpForSmartServices()
        {
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            IWebElement signUpForSmartServicesLink = wait.Until<IWebElement>(d => d.FindElementEx(By.XPath("//a[contains(text(),'Sign Up for Smart Services')]")));
            signUpForSmartServicesLink.Click();
        }
        internal void ClickLogIntoSmartServices()
        {
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            IWebElement logIntoSmartServicesLink = wait.Until<IWebElement>(d => d.FindElementEx(By.XPath("//a[contains(text(),'Log Into Smart Services')]")));
            logIntoSmartServicesLink.Click();
        }
        internal void AssertRegisterPage()
        {
            SharedPageFunctions page = new SharedPageFunctions(driver);
            page.IsElementPresent(By.XPath("//h1[contains(text(),'Register')]"));
        }
        internal void AssertSignInPage()
        {
            SharedPageFunctions page = new SharedPageFunctions(driver);
            page.IsElementPresent(By.XPath("//h1[contains(text(),'Account Login')]"));
        }
    }
}
