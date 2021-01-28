using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;
using OpenQA.Selenium;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects
{
    class RequestRefundPage
    {
        protected IWebDriver Driver { get; set; }
        protected SharedPageFunctions Page { get; set; }
        internal RequestRefundPage(ResidentialPortalWebDriver driver0) : this(driver0.Instance) { }

		internal RequestRefundPage(IWebDriver driver0)
        {
            Driver = driver0;
            Page = new SharedPageFunctions(Driver);
        }

        internal const string AccountInputLabelId = "lblAccount";
        internal const string AccountInputFieldId = "txtAccount";
        internal const string AccountCreditLabelId = "lblAccountCredit";
        internal const string AccountCreditFieldId = "txtAccountCredit";
        internal const string AddInfoInputLabelId = "lblComments";
        internal const string AddInfoInputFieldId = "txtComments";
        internal const string RequestRefundBtnId = "btnRequestRefund";

        internal void AssertRequestRefundForm()
        {
            AssertRequestRefundBtnDisplayed();
            AssertAccountInputFieldDisplayed();
            AssertRequestPrivacyNoticeBoxDisplayed();
            AssertAdditionalInformationDisplayed();
            AssertRequestPrivacyNoticeBoxDisplayed();
            AssertAccountCreditDisplayed();          
        }

        internal void AssertRequestPrivacyNoticeBoxDisplayed()
        {
	        Page.IsElementPresent(Page.ByDataAttribute(val: "privacy-notice-message-component"));
        }

        internal void AssertAccountInputFieldDisplayed()
        {
	        Page.IsElementPresent(By.Id(AccountInputLabelId));
	        Page.IsElementPresent(By.Id(AccountInputFieldId));
        }

        internal void AssertAccountCreditDisplayed()
        {
	        Page.IsElementPresent(By.Id(AccountCreditLabelId));
	        Page.IsElementPresent(By.Id(AccountCreditFieldId));
        }

        internal void AssertAdditionalInformationDisplayed()
        {
	        Page.IsElementPresent(By.Id(AddInfoInputLabelId));
	        Page.IsElementPresent(By.Id(AddInfoInputFieldId));
        }

        internal void AssertRequestRefundBtnDisplayed()
        {
	        Page.IsElementPresent(By.Id(RequestRefundBtnId));
        }

        internal void AssertRequestRefundConfirmationPage()
        {
	        Page.IsElementPresent(Page.ByDataAttribute(val: "request-refund-confirmation-title"));
	        Page.IsElementPresent(Page.ByDataAttribute(val: "request-refund-confirmation-back-to-my-accounts"));
        }

        internal void EnterAdditionalInformation(string s)
        {
	        Page.SendElementKeys(By.Id(AddInfoInputFieldId), s);
        }

        internal void ClickSubmitBtn()
        {
	        Page.ClickElement(By.Id(RequestRefundBtnId));
        }
    }
}
