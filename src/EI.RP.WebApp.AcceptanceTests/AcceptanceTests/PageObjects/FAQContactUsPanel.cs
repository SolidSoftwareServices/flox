using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;
using OpenQA.Selenium;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects
{
    class FAQContactUsPanel
    {
        protected IWebDriver driver { get; set; }
        internal FAQContactUsPanel(ResidentialPortalWebDriver driver0) : this(driver0.Instance) { }
		internal FAQContactUsPanel(IWebDriver driver0)
        {
            driver = driver0;
        }
        private string elecBillFaqID = "electricityBillFAQ";
        private string changeMyNameFaqID = "changeMyNameFAQ";
        private string meterReadingFaqID = "meterReadingFAQ";
        private string directDebitFaqID = "directDebitFAQ";
        private string closeAccFaqID = "closeAccountFAQ";
       
        
    
        internal void AssertFAQSectionDisplayed()
        {
            SharedPageFunctions page = new SharedPageFunctions(driver);
            page.IsElementPresent(By.Id(changeMyNameFaqID));
            page.IsElementPresent(By.Id(meterReadingFaqID));
            page.IsElementPresent(By.Id(directDebitFaqID));
            page.IsElementPresent(By.Id(closeAccFaqID));
            page.IsElementPresent(By.Id(elecBillFaqID));
        }
        internal void AssertContactSectionDisplayed()
        {
            SharedPageFunctions page = new SharedPageFunctions(driver);
            page.IsElementPresent(By.XPath("//*[@id='right-panel']/p[2]/a"));
            page.IsElementPresent(By.XPath("//*[@id='right-panel']/p[3]/a[1]"));
            page.IsElementPresent(By.XPath("//*[@id='right-panel']/p[3]/a[2]"));
            page.IsElementPresent(By.XPath("//*[@id='right-panel']/p[4]/a[1]"));
            page.IsElementPresent(By.XPath("//*[@id='right-panel']/p[4]/a[2]"));
        }
    }
}
