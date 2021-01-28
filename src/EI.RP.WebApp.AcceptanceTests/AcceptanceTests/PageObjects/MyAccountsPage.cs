using System;
using System.Linq;
using System.Threading;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;
using OpenQA.Selenium;
using IDictionary = System.Collections.Generic.IDictionary<string, string>;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects
{
	class MyAccountsPage
    {
        protected IWebDriver driver { get; set; }
        internal MyAccountsPage(ResidentialPortalWebDriver driver0) : this(driver0.Instance) { }
		internal MyAccountsPage(IWebDriver driver0)
        {
            driver = driver0;
        }
        SharedPageFunctions page => new SharedPageFunctions(driver);
        internal void AssertMyAccountsHeaderDisplayed()
        {
            if (page.GetCurrentLayoutType() == LayoutType.Smart && page.GetCurrentNavigationType() == LayoutType.Smart)
            {
                page.IsElementPresent(page.ByDataAttribute(val: "main-navigation-my-accounts-link"));
            }
            else
            {
                if (driver.FindElements(By.Id("btnMyAccounts")).Any())
                {
                    page.IsElementPresent(By.Id("btnMyAccounts"));
                }
            }
        }

        internal void ClickViewFullAccountDetailsSpecificAccount(IDictionary dict)
        {
            var account = dict["Account"];
            page.ClickElement(page.ByDataAttribute(val: $"account-card-view-this-account-{account}"));
        }

        internal void ClickViewFullAccountDetailsOnFirstAccount()
        {
            page.ClickElementFirst("View this Account");
        }

        internal void ClickRequestRefundSpecificAccount(IDictionary dict)
        {
	        Thread.Sleep(TimeSpan.FromSeconds(1));
	        var account = dict["Account"];
	        page.ClickElement(page.ByDataAttribute(val: $"account-card-submit-refund-request-{account}"));
        }
    }
}
