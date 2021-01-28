using OpenQA.Selenium;
using System;
using System.Threading;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;

namespace EI.RP.WebApp.RegressionTests.PageObjects
{
    class PlanPage
    {
        protected IWebDriver driver { get; set; }
        protected SharedPageFunctions page => new SharedPageFunctions(driver);
        internal PlanPage(ResidentialPortalWebDriver driver0) : this(driver0.Instance) { }
		internal PlanPage(IWebDriver driver0)
        {
            driver = driver0;
        }

        class Selectors
        {
            public string ChangeBillingPreferencesTestId { get; set; }
            public string EditDirectDebitTestId { get; set; }
        }

        readonly Selectors selectors = new Selectors()
        {
            ChangeBillingPreferencesTestId = "payments-history-change-billing-preferences-link",
            EditDirectDebitTestId = "payments-history-edit-direct-debit-link"
        };

        internal void ClickEditDirectDebit()
        {
            page.ClickElement(page.ByDataAttribute(val: IdentifierSelector.PlanPage.editDirectDebit));
        }

        internal void AssertDirectDebitDisplayed()
        {
            AssertEditDirectDebitBtnDisplayed();
        }

        internal void AssertPaperlessBillingDisplayed()
        {
            AssertPaperlessBillingToggleDisplayed();
        }
        internal void AssertEditDirectDebitBtnDisplayed()
        {
            page.IsElementPresent(page.ByDataAttribute(val: "edit-direct-debit-link"));
        }

        internal void ClickNoPaperlessBilling()
        {
            page.ClickElement(By.XPath(XPathSelectors.PlanPage.No));
        }
        internal void ClickYesPaperlessBilling()
        {
            page.ClickElement(By.XPath(XPathSelectors.PlanPage.Yes));
        }

        internal void ClickPaperlessBillingOff()
        {
            page.ClickElement(By.Id(IdentifierSelector.PlanPage.off));
        }

        internal void ClickPaperlessBillingOn()
        {
            page.ClickElement(By.Id(IdentifierSelector.PlanPage.on));
        }

        internal void AssertPaperlessBillingToggleDisplayed()
        {
            page.IsElementPresent(By.XPath("//*[@id='Plan-paperless-form']/div/div/span"));         
        }
    }
}
