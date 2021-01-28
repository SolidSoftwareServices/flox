using System;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils;
using EI.RP.WebApp.AcceptanceTests.Infrastructure;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;
using OpenQA.Selenium;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects
{
    class BillsAndPaymentsPage: ResidentialPortalBrowserFixture
    {
        protected IWebDriver driver { get; set; }
        protected SharedPageFunctions page => new SharedPageFunctions(driver);

        class Selectors
        {
            public string ChangeBillingPreferencesTestId { get; set; }
            public string EditDirectDebitTestId { get; set; }
            public string HistoryComponentTestId { get; set; }
            public string PaginationClass { get; set; }
            public string BillClass { get; set; }
        }

        readonly Selectors selectors = new Selectors()
        {
            ChangeBillingPreferencesTestId = "payments-history-change-billing-preferences-link",
            EditDirectDebitTestId = "payments-history-edit-direct-debit-link",
            HistoryComponentTestId = "bills-and-payments-component",
            PaginationClass = ".pagination",
            BillClass = ".download-bill"
        };

        internal BillsAndPaymentsPage(ResidentialPortalWebDriver driver0) : this(driver0.Instance) { }
        internal BillsAndPaymentsPage(IWebDriver driver0)
        {
            driver = driver0;
        }
        internal void AssertBillingAndPaymentOptionsBtnDisplayed()
        {
            page.IsElementPresent(page.ByDataAttribute(val: selectors.ChangeBillingPreferencesTestId));
        }

        internal void ClickPayNow()
        {
            page.ClickElement(page.ByDataAttribute(val: IdentifierSelector.PlanPage.payNow));
        }

        internal void AssertIsInPage()
        {
            page.IsElementPresent(page.ByDataAttribute("page", "payments-history"));
        }
        internal void AssertBillsAndPaymentsOverviewDisplayed()
        {
            AssertBillsAndPaymentsTablePresent();
        }
        internal void ClickBillingPreferencesBtn()
        {
            page.ClickElement(page.ByDataAttribute(val: selectors.ChangeBillingPreferencesTestId));
        }
        internal void AssertBillsAndPaymentsTablePresent()
        {
            page.IsElementPresent(page.ByDataAttribute(val: selectors.HistoryComponentTestId));
        }
        internal void AssertPaginationDisplayed()
        {
            page.IsElementPresent(By.CssSelector(selectors.PaginationClass)); ;
        }
        internal void AssertViewBillDownloadDisplayed()
        {
            throw new NotImplementedException();
        }
        internal void AssertBillsAndPaymentsPage()
        {
            AssertIsInPage();
            AssertBillingAndPaymentOptionsBtnDisplayed();
            AssertBillsAndPaymentsOverviewDisplayed();
            //AssertPaginationDisplayed();
        }
    }
}
