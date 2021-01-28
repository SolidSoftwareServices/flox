using System.Linq;
using System.Threading;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Selenium;
using OpenQA.Selenium;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects
{
    class DashBoardPage
    {
        protected IWebDriver driver { get; set; }
        internal DashBoardPage(ResidentialPortalWebDriver driver0) : this(driver0.Instance) { }
		internal DashBoardPage(IWebDriver driver0)
        {
            driver = driver0;
        }
		
		SharedPageFunctions page => new SharedPageFunctions(driver);

		internal string overrideName = "overridelink";

        internal void ClickMakeAPaymentNavBtn()
        {            
            page.ClickElement(By.Id(IdentifierSelector.NavMenu.makeAPaymentTab));
            MakeAPaymentPage makeAPaymentPage = new MakeAPaymentPage(driver);
            makeAPaymentPage.AssertMakeAPaymentPage();
        }
        internal void ClickUsageNavBtn()
        {
            page.ClickElement(page.ByDataAttribute(val: IdentifierSelector.NavMenu.usageTab));
        }
        internal void ClickBillsAndPaymentsNavBtn()
        {
            page.ClickElement(page.ByDataAttribute(val: IdentifierSelector.NavMenu.paymentsTab));
            BillsAndPaymentsPage billsAndPaymentsPage = new BillsAndPaymentsPage(driver);
            billsAndPaymentsPage.AssertBillsAndPaymentsPage();
        }
        internal void ClickMeterReadingNavBtn()
        {
            page.ClickElement(page.ByDataAttribute(val: IdentifierSelector.NavMenu.meterReadingTab));
            SubmitMeterReadingPage submitMeterReadingPage = new SubmitMeterReadingPage(driver);
            submitMeterReadingPage.AssertMeterReadingPage();
        }
        internal void ClickContactUsForm()
        {
            page.ClickElement(page.ByDataAttribute(val: IdentifierSelector.NavMenu.contactUsMenuItem));
            ContactUsFormPage contactUsFormPage = new ContactUsFormPage(driver);
            contactUsFormPage.AssertContactUsFormBeforeSelectQueryType();
        }
        internal void ClickMoveHouseNavBtn()
        {
            page.ClickElement(page.ByDataAttribute(val: IdentifierSelector.NavMenu.moveHouseTab));
            MoveHouseLandingPage _moveHouseLandingPage = new MoveHouseLandingPage(driver);
            _moveHouseLandingPage.AssertMoveHouseLandingPage();
        }
        internal void ClickMyAccountsNavBtn()
        {
            if (page.GetCurrentLayoutType() == LayoutType.Smart && page.GetCurrentNavigationType() == LayoutType.Smart)
            {
                driver.ClickElementEx(page.ByDataAttribute(val: "main-navigation-my-accounts-link"));
            }
            else
            {
	            driver.ClickElementEx(By.Id("btnMyAccounts"),throwIfNotFoundAfterTimeout:false);
                
	            driver.ClickElementEx(By.Id("dds_back_to_my_accounts"));
            }
        }

        public void ClickHelpNavBtn()
		{
			driver.ClickElementEx(By.XPath(XPathSelectors.NavMenu.help));
		}

        internal void ClickPlanNavBtn()
        {   
	        driver.ClickElementEx(page.ByDataAttribute(val: "nav-plan-link"));
	        var planPage = new PlanPage(driver);
	        planPage.AssertPlanPage();
        }
	}
}
