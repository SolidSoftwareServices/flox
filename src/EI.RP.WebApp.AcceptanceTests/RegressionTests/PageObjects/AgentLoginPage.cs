using OpenQA.Selenium;
using System;
using System.Threading;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;
using IDictionary = System.Collections.Generic.IDictionary<string, string>;
using OpenQA.Selenium.Support.UI;

namespace EI.RP.WebApp.RegressionTests.PageObjects
{
    class AgentLoginPage
    {
        protected IWebDriver driver { get; set; }
        private SharedPageFunctions page => new SharedPageFunctions(driver);

        internal AgentLoginPage(ResidentialPortalWebDriver driver0):this(driver0.Instance) { }

		internal AgentLoginPage(IWebDriver driver0)
        {
            driver = driver0;
        }

        internal string eiLogoID = "logoImg",
            headerSubtitleID = "loginSubtitle",
            userNameFieldID = "txtUserName",
            passWordFieldID = "txtPassword",
            agentLoginBtnID = "btnLogin";
        internal Boolean AssertAgentLoginPage()
        {
            AssertAgentLoginEILogoDisplayed();
            AssertAgentLoginHeaderDisplayed();
            AssertAgentLoginSubtitleDisplayed();
            AssertAgentLoginUsernameFieldDisplayed();
            AssertAgentLoginPasswordFieldDisplayed();
            AssertAgentLoginLoginBtnDisplayed();
            return true;
        }

        internal void AssertErrorUsernameEmpty()
        {
            page.IsElementTextPresent("Please enter your user name");  
        }

        internal void AgentSignIn(IDictionary dict)
		{
			WebDriverWait wait1 = new WebDriverWait(driver, TimeSpan.FromSeconds(2));
			Thread.Sleep(TimeSpan.FromSeconds(2));

			string username = dict["Email"];
			string password = dict["Password"];
			EnterUsername(username);
			EnterPassword(password);
			IWebElement agentLoginBtnDisabled = wait1.Until(ExpectedConditions.ElementIsVisible(By.Id(agentLoginBtnID)));
			if (agentLoginBtnDisabled.Enabled)
			{
                ClickLogin();
            }
			else
			{
				Thread.Sleep(TimeSpan.FromSeconds(2));

				EnterUsername(username);
				EnterPassword(password);
				try
				{
                    ClickLogin();
                }
				catch
				{
					AgentSignIn(dict);
				}
			}
		}

        internal void AssertErrorPasswordEmpty()
        {
            page.IsElementTextPresent("Please enter your password");
        }

        internal void AssertErrorPasswordIncorrect()
        {
            page.IsElementTextPresent("Incorrect username or password. Please  try again or click Forgot Password above to reset it.");
        }

        internal bool OnPage()
        {
            return driver.FindElements(By.Id(IdentifierSelector.AgentLoginPage.usernameTxt)).Count > 0 ? true : false;
        }

        internal void ClickLogin()
        {
            page.ClickElement(By.Id(agentLoginBtnID));
        }

        internal void EnterUsername(string username)
        {
            page.SendElementKeys(By.Id(userNameFieldID), username);
        }
        internal void EnterPassword(string password)
        {
            page.SendElementKeys(By.Id(passWordFieldID), password);
        }
        internal void AssertAgentLoginEILogoDisplayed()
        {
            page.IsElementPresent(By.Id(eiLogoID));
        }
        internal void AssertAgentLoginHeaderDisplayed()
        {
            page.IsElementPresent(By.XPath("//h1[contains(text(), 'Admin Login')]"));
        }
        internal void AssertAgentLoginSubtitleDisplayed()
        {
            page.IsElementPresent(By.Id(headerSubtitleID));
        }
        internal void AssertAgentLoginUsernameFieldDisplayed()
        {
            page.IsElementPresent(By.Id(userNameFieldID));
        }
        internal void AssertAgentLoginPasswordFieldDisplayed()
        {
            page.IsElementPresent(By.Id(passWordFieldID));
        }
        internal void AssertAgentLoginLoginBtnDisplayed()
        {
            page.IsElementPresent(By.Id(agentLoginBtnID));
        }   
    }
}
