using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;
using OpenQA.Selenium;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects
{
    class PlanPage
    {
        protected IWebDriver Driver { get; set; }

        protected SharedPageFunctions Page => new SharedPageFunctions(Driver); 
        internal PlanPage(ResidentialPortalWebDriver driver0) : this(driver0.Instance) { }

		internal PlanPage(IWebDriver driver0)
        {
            Driver = driver0;
        }
        internal void AssertPlanPage()
        {
	        Page.IsElementPresent(Page.ByDataAttribute("page", "plan"));
        }

        internal void ClickAddGasCTA()
        {
	        Page.ClickElement(Page.ByDataAttribute(val: "add-gas-flow"));
        }

        internal void ClickEditDirectDebit()
        {
	        Page.ClickElement(Page.ByDataAttribute(val: IdentifierSelector.PlanPage.editDirectDebit));
        }

        internal void AssertPaymentMethodDisplayed()
        {
	        Page.IsElementPresent(Page.ByDataAttribute(val: "payment-method-label"));
	        Page.IsElementPresent(Page.ByDataAttribute(val: "payment-method"));
        }

        internal void AssertPaperlessBillingDisplayed()
		{
			Page.IsElementPresent(Page.ByDataAttribute(val: "paperless-billing-heading"));
			Page.IsElementPresent(Page.ByDataAttribute(val: "paperless-billing-toggle"));
			Page.IsElementPresent(Page.ByDataAttribute(val: "paperless-billing-text"));
			Page.IsElementPresent(Page.ByDataAttribute(val: "paperless-billing-link"));
		}
    }
}
