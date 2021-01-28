using OpenQA.Selenium;
using System;
using System.Threading;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;

namespace EI.RP.WebApp.RegressionTests.PageObjects
{
    class PaymentsPage
    {
        protected IWebDriver driver { get; set; }
        protected SharedPageFunctions page => new SharedPageFunctions(driver);

        class Selectors
        {
            public string HistoryComponentTestId { get; set; }
            public string PaginationClass { get; set; }
        }

        readonly Selectors selectors = new Selectors()
        {
            HistoryComponentTestId = "bills-and-payments-component",
            PaginationClass = ".pagination"
        };
        internal PaymentsPage(ResidentialPortalWebDriver driver0) : this(driver0.Instance) { }
		internal PaymentsPage(IWebDriver driver0)
        {
            driver = driver0;
        }

        internal void ClickMoreAboutEqualMonthlyPayments()
        {
            page.ClickElement(page.ByDataAttribute(val: IdentifierSelector.PlanPage.equaliserSetup));
        }

        internal void AssertBillingAndPaymentOptionsBtnDisplayed()
        {
            page.IsElementPresent(page.ByDataAttribute(val: IdentifierSelector.PaymentsPage.changeBllingPreferences));
        }

        internal void AssertIsInPage()
        {
            page.IsElementPresent(page.ByDataAttribute("page", "payments-history"));
        }
        internal void AssertBillsAndPaymentsOverviewDisplayed()
        {
            AssertBillsAndPaymentsTablePresent();
        }
        internal void ClickBillingAndPaymentOptionsBtn()
        {
            page.ClickElement(page.ByDataAttribute(val: IdentifierSelector.PaymentsPage.changeBllingPreferences));
        }
        internal void AssertBillsAndPaymentsTablePresent()
        {
            page.IsElementPresent(page.ByDataAttribute(val: selectors.HistoryComponentTestId));
        }
        internal void AssertPaginationDisplayed()
        {
            page.IsElementPresent(By.CssSelector(selectors.PaginationClass)); ;
        }
        internal void AssertBillsAndPaymentsPage()
        {
            AssertIsInPage();
            AssertBillingAndPaymentOptionsBtnDisplayed();
            AssertBillsAndPaymentsOverviewDisplayed();
        }
    }
}
