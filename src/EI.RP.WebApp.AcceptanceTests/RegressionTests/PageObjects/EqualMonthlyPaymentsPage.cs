using OpenQA.Selenium;
using System;
using System.Threading;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;

namespace EI.RP.WebApp.RegressionTests.PageObjects
{
    class EqualMonthlyPaymentsPage
    {
        protected IWebDriver driver { get; set; }
        protected SharedPageFunctions page => new SharedPageFunctions(driver);
        internal EqualMonthlyPaymentsPage(ResidentialPortalWebDriver driver0) : this(driver0.Instance) { }
		internal EqualMonthlyPaymentsPage(IWebDriver driver0)
        {
            driver = driver0;
        }
  
        internal void ClickSetUpEqualizer()
        {
            page.ClickElement(page.ByDataAttribute(val: "equalizer-landing-set-up"));
        }

        internal void ClickSetUpDirectDebit()
        {
            page.ClickElement(page.ByDataAttribute(val: "equalizer-setup-set-up-direct-debit"));
        }
    }
}
