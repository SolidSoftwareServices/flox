using System;
using System.Linq;
using System.Threading;
using EI.RP.WebApp.AcceptanceTests.Infrastructure;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Selenium;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using IDictionary = System.Collections.Generic.IDictionary<string, string>;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects
{
    public class SignInPage
    {
        protected IWebDriver Driver { get; set; }
        private readonly SharedPageFunctions _page;
        internal SignInPage(ResidentialPortalWebDriver driver0) : this(driver0.Instance) { }
		public SignInPage(IWebDriver driver0)
        {
            Driver = driver0;
            _page = new SharedPageFunctions(Driver);
        }

        WebDriverWait wait;

        internal string overrideName = "overridelink";

        internal string emailID = "txtEmail",
			userNameId="txtUserName",
            passwordID = "txtPassword",
            loginBtnID = "btnLogin",
            errorEmailID = "emailErrMsg",
            createAccountBtnID = "createAccountLink",
            forgotPasswordLinkID = "forgotPasswordLink",
            eIlogoID = "logoImg",
            headerSubtitleID = "loginSubtitle",
            paygTaglineID = "paygTagline",
            paygLinkID = "paygLink",
            createAccountTaglineID = "createAccountTagline";

        internal string validEmail = "duelfuel@esb.ie",
	        validPassword = "Test3333";

        internal void NavigateToSignInPageURL(SingleTestContext shared)
        {
	        Driver.Navigate().GoToUrl( TestSettings.Default.PublicTargetUrl);
		}

        internal void EnterEmail(string email)
        {
	        _page.SendElementKeys(By.Id(emailID), email);
        }

        internal void EnterPassword(string password)
        {
	        _page.SendElementKeys(By.Id(passwordID), password);
        }

        internal void ValidSignIn()
        { 
            EnterEmail(validEmail);
            EnterPassword(validPassword);
            ClickLogin();
        }

        internal void SignIn(IDictionary dictionary, string defaultPageUrl)
        {
            if (LoggedIn())
			{
				var spf = new SharedPageFunctions(Driver);

                var legacyLogouts = Driver.FindElements(By.XPath("/html/body/header/div/div/nav/div/ul[2]/li[2]/a"));
                var smartLogouts = Driver.FindElements(spf.ByDataAttribute(val: "main-navigation-log-out-link-desktop"));

                if (legacyLogouts.Any())
                {
                    Driver.ClickElementEx(legacyLogouts.First());
                }
                else
                {
                    Driver.ClickElementEx(spf.ByDataAttribute(val: "main-navigation-open-settings"));
                    Driver.ClickElementEx(smartLogouts.First());
                }

                Thread.Sleep(TimeSpan.FromSeconds(2));
			}
            else
            {
                Driver.Navigate().GoToUrl(defaultPageUrl);
            }
			string email = dictionary["Email"];
            string password = dictionary["Password"];
            EnterEmail(email);
            EnterPassword(password);
			WebDriverWait wait1 = new WebDriverWait(Driver, TimeSpan.FromSeconds(2));
			IWebElement loginBtn = wait1.Until(ExpectedConditions.ElementIsVisible(By.Id(loginBtnID))); 
			if (loginBtn.Enabled &&
				Driver.FindElementEx(By.Id(emailID)).GetAttribute("value").Contains(email) &&
				Driver.FindElementEx(By.Id(passwordID)).GetAttribute("value").Contains(password))
			{
				ClickLogin();
			}
			else
			{
				Thread.Sleep(TimeSpan.FromSeconds(2));
				EnterEmail(email);
				EnterPassword(password);
				try
				{
					ClickLogin();
				}
				catch
				{
					SignIn(dictionary, defaultPageUrl);
				}
			}
        }

        internal void AssertLoginPage(bool assertPublic=true)
        {
	        AssertSignInPageLogoDisplayed();
	        AssertSignPageHeaderDisplayed();
	        AssertHeaderSubtitleDisplayed();
	        AssertEmailFieldDisplayed();
	        AssertPasswordFieldDisplayed();
	        if (assertPublic)
	        {
		        AssertForgotPasswordLinkDisplayed();
		        AssertLoginBtnDisplayed();
		        AssertPAYGTaglineDisplayed();
		        AssertCreateAccountTaglineDisplayed();
		        AssertCreateAccountLinkDisplayed();
	        }
        }

        internal void ClickLogin()
        {
            var loginBtn = Driver.FindElementEx(By.Id(loginBtnID));
            wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
            wait.Until(driver => loginBtn.Displayed);
            loginBtn.Click();
        }

        internal void ClickCreateAccount()
        {
	        _page.ClickElement(By.Id(createAccountBtnID));
        }

        internal void ClickPAYG()
        {
            var PAYGLink = Driver.FindElementEx(By.Id(paygLinkID));
            wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
            wait.Until(driver => PAYGLink.Displayed);
            PAYGLink.Click();
        }

        internal void ClickForgotPassword()
        {
            var forgotPasswordLink = Driver.FindElementEx(By.Id(forgotPasswordLinkID));
            wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
            wait.Until(driver => forgotPasswordLink.Displayed);
            forgotPasswordLink.Click();
        }

        internal void AssertErrorEmail()
        {
	        _page.IsElementPresent(By.Id(errorEmailID));
        }

        internal void AssertForgotPasswordPage()
        {
            _page.IsElementPresent(By.XPath("//h1[contains(text(),'Password Reset')]"));
        }

        internal void AssertPAYGPage()
        {
	        _page.IsElementPresent(By.XPath("//a[contains(text(),'PAYG Home')]"));
        }

        internal void EnterSignInEmail(string email)
        {
	        _page.SendElementKeys(By.Id(emailID), email);
        }

        internal void AssertSignInPageLogoDisplayed()
        {
	        _page.IsElementPresent(By.Id(eIlogoID));
        }

        internal void AssertSignPageHeaderDisplayed()
        {
	        _page.IsElementPresent(By.XPath("//h1[contains(text(), 'Account Login') or contains(text(), 'Admin Login')]"));
        }

        internal void AssertHeaderSubtitleDisplayed()
        {
	        _page.IsElementPresent(By.Id(headerSubtitleID));
        }

        internal void AssertEmailFieldDisplayed()
        {
	        Assert.IsTrue(Driver.FindElementEx(By.Id(emailID), false, timeout: TimeSpan.FromSeconds(5)) != null
	                      || Driver.FindElementEx(By.Id(userNameId), false, timeout: TimeSpan.FromSeconds(5)) != null);
        }

        internal void AssertPasswordFieldDisplayed()
        {
	        _page.IsElementPresent(By.Id(passwordID));
        }

        internal void AssertForgotPasswordLinkDisplayed()
        {
	        _page.IsElementPresent(By.Id(forgotPasswordLinkID));
        }

        internal void AssertLoginBtnDisplayed()
        {
	        _page.IsElementPresent(By.Id(loginBtnID));
        }

        internal void AssertPAYGTaglineDisplayed()
        {
	        _page.IsElementPresent(By.Id(paygTaglineID));
        }

        internal void AssertCreateAccountTaglineDisplayed()
        {
	        _page.IsElementPresent(By.Id(createAccountTaglineID));
        }

        internal void AssertCreateAccountLinkDisplayed()
        {
	        _page.IsElementPresent(By.Id(createAccountTaglineID));
        }

		private bool LoggedIn()
		{
			return Driver.FindElements(By.CssSelector("[data-navigation]")).Count > 0;
		}
	}
}
