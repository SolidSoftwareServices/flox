using System;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects
{
    class PricePlanPage
    {
        protected IWebDriver driver { get; set; }
        internal PricePlanPage(ResidentialPortalWebDriver driver0) : this(driver0.Instance) { }
		internal PricePlanPage(IWebDriver driver0)
        {
            driver = driver0;
        }
        WebDriverWait wait;
        internal void ClickCancelBtn()
        {
            throw new NotImplementedException();
        }
    }
}
