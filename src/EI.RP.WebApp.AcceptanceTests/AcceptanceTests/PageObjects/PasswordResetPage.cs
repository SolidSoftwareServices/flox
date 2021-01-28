using System;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Selenium;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects
{
    class PasswordResetPage
    {
        protected IWebDriver driver { get; set; }
        internal PasswordResetPage(ResidentialPortalWebDriver driver0) : this(driver0.Instance) { }
		internal PasswordResetPage(IWebDriver driver0)
        {
            driver = driver0;
        }
        WebDriverWait wait;
        internal string overrideName = "overridelink";
        internal string emailID = "txtEmail",
            resetPasswordBtnID = "btnResetPassword",
            errorEmailID = "emailErrorMsg";
        internal string errorMsgEmail = "";
        internal void EnterEmail(string email)
        {
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            IWebElement fieldEmail = wait.Until<IWebElement>(d => d.FindElementEx(By.Id(emailID)));
            fieldEmail.SendKeys(email);
        }
        internal void AssertPasswordResetBtnGreyedOut()
        {
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            IWebElement resetPasswordBtn = wait.Until<IWebElement>(d => d.FindElementEx(By.Id(resetPasswordBtnID)));
            wait.Until(driver => resetPasswordBtn.Displayed);
            Assert.IsFalse(resetPasswordBtn.Enabled);
        }
        internal void AssertErrorInvalidEmailAddress()
        {
            SharedPageFunctions page = new SharedPageFunctions(driver);
            page.IsElementPresent(By.Id(errorEmailID));
        }
        internal void AssertPasswordResetPage()
        {
            SharedPageFunctions page = new SharedPageFunctions(driver);
            page.IsElementPresent(By.XPath("//h1[contains(text(),'Password Reset')]"));
        }
        internal void ClickResetPassword()
        {
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            IWebElement loginBtn = wait.Until<IWebElement>(d => d.FindElementEx(By.Id(resetPasswordBtnID)));
            loginBtn.Click();
        }  
    }
}
