using OpenQA.Selenium;
using System;
using System.Threading;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;
using IDictionary = System.Collections.Generic.IDictionary<string, string>;
using OpenQA.Selenium.Support.UI;

namespace EI.RP.WebApp.RegressionTests.PageObjects
{
    class RequestRefundPage
    {
        protected IWebDriver driver { get; set; }
        SharedPageFunctions page => new SharedPageFunctions(driver); internal RequestRefundPage(ResidentialPortalWebDriver driver0) : this(driver0.Instance) { }
		internal RequestRefundPage(IWebDriver driver0)
        {
            driver = driver0;
        }


        internal void AssertErrorTooLong()
        {
            page.IsElementTextPresent(TextMatch.RefundRequestPage.ErrorTooLong);
        }

        internal void AssertAccountCredit(string v)
        {
            page.IsElementTextPresent(v);
        }

        internal void EnterAdditionalInformation(string s)
        {

            page.SendElementKeys(By.Id(IdentifierSelector.RefundRequestPage.addInfoInputFieldID), s);
        }
        internal void ClickSubmitBtn()
        {

            page.ClickElement(By.Id(IdentifierSelector.RefundRequestPage.requestRefundBtnID));
        }
        internal void AssertRequestRefundForm()
        {
            AssertRequestRefundBtnDisplayed();
            AssertRequestPrivacyNoticeBoxDisplayed();
            AssertAccountCreditDisplayed();
        }
        internal void AssertRequestRefundConfirmationPage()
        {
            page.IsElementPresent(page.ByDataAttribute(val: IdentifierSelector.RefundRequestPage.confirmationTitle));
            page.IsElementPresent(page.ByDataAttribute(val: IdentifierSelector.RefundRequestPage.backToAccOverviewBtnID));
        }
        private void AssertRequestRefundBtnDisplayed()
        {
            page.IsElementPresent(By.Id(IdentifierSelector.RefundRequestPage.requestRefundBtnID));
        }
        private void AssertAccountCreditDisplayed()
        {

            page.IsElementPresent(By.Id(IdentifierSelector.RefundRequestPage.accountCreditLabelID));
        }
        private void AssertRequestPrivacyNoticeBoxDisplayed()
        {

            page.IsElementPresent(page.ByDataAttribute(val: IdentifierSelector.RefundRequestPage.gdprTextID));
        }

    }
}
