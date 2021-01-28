using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;
using OpenQA.Selenium;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects
{
    class EnergyServicesAccountPage
    {
        protected IWebDriver driver { get; set; }
        internal EnergyServicesAccountPage(ResidentialPortalWebDriver driver0) : this(driver0.Instance) { }
		internal EnergyServicesAccountPage(IWebDriver driver0)
        {
            driver = driver0;
        }

        internal string EnergyServicesHeaderID = "nrg_ao_header",
            smartHeatingControlsID = "",
            paymentMethodValID = "nrg_ao_pay_method",
            contactUsID = "",
            amountHeaderID = "nrg_ao_amount_header",
            amountValID = "nrg_ao_bill_amount",
            paymentDateValID = "nrg_ao_pay_date",
            accountTypeID = "accountTypeEnergy";

        internal void AssertContactUsDisplayed()
        {
            SharedPageFunctions page = new SharedPageFunctions(driver);
            page.IsElementPresent(page.ByDataAttribute(val: "energy-services-account-overview-contact-us"));
        }

        internal void AssertBillingComponentDisplayed()
        {
            SharedPageFunctions page = new SharedPageFunctions(driver);
            page.IsElementPresent(page.ByDataAttribute(val: "energy-service-billing-details-component"));
        }

        internal void AssertLastPaymentAmountDisplayed()
        {
            SharedPageFunctions page = new SharedPageFunctions(driver);
            page.IsElementPresent(page.ByDataAttribute(val: "energy-service-billing-details-component-amount"));
        }

        internal void AssertPaymentDateDisplayed()
        {
            SharedPageFunctions page = new SharedPageFunctions(driver);
            page.IsElementPresent(page.ByDataAttribute(val: "energy-service-billing-details-component-date"));
        }
    }
}
