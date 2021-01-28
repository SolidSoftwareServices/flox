using OpenQA.Selenium;
using System;
using System.Threading;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;
using IDictionary = System.Collections.Generic.IDictionary<string, string>;
using OpenQA.Selenium.Support.UI;

namespace EI.RP.WebApp.RegressionTests.PageObjects
{
    class NavigationBar
    {
        protected IWebDriver driver { get; set; }
        private SharedPageFunctions page => new SharedPageFunctions(driver); internal NavigationBar(ResidentialPortalWebDriver driver0) : this(driver0.Instance) { }
		internal NavigationBar(IWebDriver driver0)
        {
            driver = driver0;
        }

        internal void AssertNoMoveHouseTab()
        {
            page.IsElementTextNotPresent("Moving House");
        }
    }
}
