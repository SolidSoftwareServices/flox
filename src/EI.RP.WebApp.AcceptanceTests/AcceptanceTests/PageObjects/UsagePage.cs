using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;
using OpenQA.Selenium;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects
{
	class UsagePage
    {
		protected IWebDriver driver { get; set; }
		SharedPageFunctions page => new SharedPageFunctions(driver);

		internal UsagePage(ResidentialPortalWebDriver driver0):this(driver0.Instance) { }
        internal UsagePage(IWebDriver driver0)
        {
            driver = driver0;
        }

        internal void ClickCompareYears()
        {
            page.ClickElement(By.Id(IdentifierSelector.UsagePage.compareYears));
        }

        internal void ClickKWH()
        {
            page.ClickElement(By.Id(IdentifierSelector.UsagePage.KWH));
        }

        internal void ChartIsDisplayed()
        {
            //page.IsElementPresent(By.XPath(XPathSelectors.UsagePage.chart));
            page.IsElementPresent(By.XPath(XPathSelectors.UsagePage.year));
            //page.IsElementPresent(By.XPath(XPathSelectors.UsagePage.amount));
        }

        internal void ClickCompareNow()
        {
            page.ClickElement(By.Id(IdentifierSelector.UsagePage.compareNow));
            AssertComparison();
        }

        private void AssertComparison()
        {
            page.IsElementPresent(By.XPath(XPathSelectors.UsagePage.year1));
            page.IsElementPresent(By.XPath(XPathSelectors.UsagePage.year2));
            page.IsElementPresent(By.XPath(XPathSelectors.UsagePage.compFirst));
            page.IsElementPresent(By.XPath(XPathSelectors.UsagePage.compSecond));
        }

        internal void ClickNextYear()
        {
            page.ClickElement(By.XPath(XPathSelectors.UsagePage.nextYear));
        }

        internal void ClickPrevYear()
        {
            page.ClickElement(By.XPath(XPathSelectors.UsagePage.prevYear));
        }
    }
}
