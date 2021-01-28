﻿using OpenQA.Selenium;
using System;
using System.Threading;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;

namespace EI.RP.WebApp.RegressionTests.PageObjects
{
    class EditDirectDebitConfirmationPage
    {
        protected IWebDriver driver { get; set; }
        protected SharedPageFunctions _page { get; set; }
        internal EditDirectDebitConfirmationPage(ResidentialPortalWebDriver driver0) : this(driver0.Instance) { }
		internal EditDirectDebitConfirmationPage(IWebDriver driver0)
        {
            driver = driver0;
            _page = new SharedPageFunctions(driver);
        }

        internal void AssertConfirmationScreenSuccess()
        {
            AssertThankYouDisplayed();
            AssertBackToMyAccountsBtnDisplayed();
        }

        private void AssertThankYouDisplayed()
        {
            _page.IsElementPresent(_page.ByDataAttribute(val: "direct-debit-confirmation-thank-you"));
        }

        private void AssertBackToMyAccountsBtnDisplayed()
        {
            _page.IsElementPresent(_page.ByDataAttribute(val: "direct-debit-confirmation-back-to-my-accounts"));
        }
    }
}
