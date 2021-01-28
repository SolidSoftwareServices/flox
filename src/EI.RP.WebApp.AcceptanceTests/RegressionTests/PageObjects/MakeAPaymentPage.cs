using OpenQA.Selenium;
using System;
using System.Net.Mime;
using System.Threading;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Selenium;

namespace EI.RP.WebApp.RegressionTests.PageObjects
{
    class MakeAPaymentPage
    {
        protected IWebDriver driver { get; set; }
        protected SharedPageFunctions page => new SharedPageFunctions(driver);

        internal string makeAPaymentHeaderID = "makeAMakeAPaymentPageHeader",
            payDiffAmountBtnID = "payDiffAmount",
            payDiffAmountTxtFieldID = "text-field-1",
            cardHolderNameTxtFieldID = "pas_ccname",
            cardExpiryTxtFieldID = "pas_expiry",
            currentBalanceID = "currentBalance",
            cardNumTxtFieldID = "pas_ccnum",
            cardSecurityTxtFieldID = "pas_cccvc",
            saveCardDetailsChckID = "hppRealVaultCheck",
            paymentDueDateValID = "dueDate",
            billIssueDateValID = "lastBillDate",
            nextBillApproxValID = "nextBillDate",
            directDebitCustBoxID = "directDebitDiv",
            PAYGBoxHeaderID = "electicityPAYGCustomerHeader",
            pawWithSavedCardHeaderID = "linkPayWithSavedCard",
            payWithAnotherCardLinkID = "linkPayWithNewCard",
            payNowBtnID = "rxp-primary-btn",
            payDiffAmountSubmitBtnID = "payAmount";


        class Selectors
        {
            public string PayDifferentAmountTestId { get; set; }
            public string LastPaymentCurrentBalanceAmount { get; set; }
            public string MakeAPaymentBillIssueDate { get; set; }
            public string MakeAPaymentPaymentDueDate { get; set; }
            public string MakeAPaymentNextBillDate { get; set; }
            public string PayDifferentAmountSubmitTestId { get; set; }
            public string PayDifferentAmountCancelTestId { get; set; }
            public string PayDifferentAmountInputTestId { get; set; }
            public string PrivacyNoticeMessageTestId { get; set; }
            public string MakeAPaymentTitleTestId { get; set; }
            public string ChangePaymentAmountPage { get; set; }
        }

        readonly Selectors selectors = new Selectors()
        {
            MakeAPaymentTitleTestId = "last-payment-title",
            LastPaymentCurrentBalanceAmount = "last-payment-current-balance-amount",
            MakeAPaymentBillIssueDate = "make-a-payment-bill-issue-date",
            MakeAPaymentPaymentDueDate = "make-a-payment-payment-due-date",
            MakeAPaymentNextBillDate = "make-a-payment-next-bill-date",
            PayDifferentAmountTestId = "last-payment-pay-different-amount",
            PayDifferentAmountSubmitTestId = "change-payment-amount-submit",
            PayDifferentAmountCancelTestId = "change-payment-amount-cancel",
            PayDifferentAmountInputTestId = "change-payment-amount-amount-input",
            PrivacyNoticeMessageTestId = "privacy-notice-message-component",
            ChangePaymentAmountPage = "change-payment-amount"
        };
        internal MakeAPaymentPage(ResidentialPortalWebDriver driver0) : this(driver0.Instance) { }
		internal MakeAPaymentPage(IWebDriver driver0)
        {
            driver = driver0;
        }

        internal void AssertPaymentAmountChanged(string payAmount)
        {
            page.IsElementPresent(By.XPath(XPathSelectors.PaymentsPage.PaymentValue),e=>e.Text=="€"+payAmount);
        }


        internal void ClickSubmitBtnPayDiffAmount()
        {
            page.ClickElement(page.ByDataAttribute(val: selectors.PayDifferentAmountSubmitTestId));
        }

        internal void EnterNewPayAmount(string p0)
        {
            page.SendElementKeys(page.ByDataAttribute(val: selectors.PayDifferentAmountInputTestId), p0);
        }

        internal void ClickPayDifferentAmount()
        {
            driver.ClickElementEx(page.ByDataAttribute(val: selectors.PayDifferentAmountTestId));
            AssertChangePaymentAmountPage();
        }

        internal void AssertPaymentConfirmationScreen()
        {
            throw new NotImplementedException();
        }

        internal void ClickPayWithAnotherCardBtn()
        {
            page.ClickElement(By.Id(payWithAnotherCardLinkID));
        }

        internal void ClickSaveCardDetails()
        {
            page.ClickElement(By.Id(saveCardDetailsChckID));
        }

        internal void EnterCardHolderName(string p0)
        {
            page.SendElementKeys(By.Id(cardHolderNameTxtFieldID), p0);
        }

        internal void EnterCardNumber(string p0)
        {
            driver.SwitchTo().Frame("paymentFrame");
            page.SendElementKeys(By.Id(cardNumTxtFieldID), p0);
        }

        internal void EnterSecurityCode(string p0)
        {
            page.SendElementKeys(By.Id(cardSecurityTxtFieldID), p0);
        }

        internal void EnterCardExpiry(string p0)
        {
            page.SendElementKeys(By.Id(cardExpiryTxtFieldID), p0);
        }

        internal void AssertMakeAMakeAPaymentPage()
        {
            AssertAll();
        }

        internal void AssertErrorAlphabetical()
        {
            page.IsElementTextPresent(TextMatch.PaymentsPage.payNowErrorAlphabetical);
        }


        internal void AssertChangePaymentAmountPage()
        {
            page.IsElementPresent(page.ByDataAttribute("page", selectors.ChangePaymentAmountPage));
        }

        private void AssertPrivacyNotice()
        {
            page.IsElementPresent(page.ByDataAttribute(val: selectors.PrivacyNoticeMessageTestId));
        }
        internal void AssertErrorMinus()
        {
            page.IsElementTextPresent(TextMatch.PaymentsPage.payNowErrorMinus);
        }
        internal void AssertErrorHigh()
        {
            page.IsElementTextPresent(TextMatch.PaymentsPage.payNowErrorHigh);
        }

        private void AssertAll()
        {
            AssertPrivacyNotice();
            AssertMakeAPaymentHeaderDisplayed();
            AssertAmountDueDisplayed();
            AssertBillIssuedDisplayed();
            AssertPaymentDueDisplayed();
            AssertNextBillDisplayed();
            //AssertPayAsYouGoNotifier();
        }

        private void AssertNextBillDisplayed()
        {
            page.IsElementPresent(page.ByDataAttribute(val: selectors.MakeAPaymentNextBillDate));
        }

        private void AssertPaymentDueDisplayed()
        {
            page.IsElementPresent(page.ByDataAttribute(val: selectors.MakeAPaymentPaymentDueDate));
        }
        private void AssertBillIssuedDisplayed()
        {
            page.IsElementPresent(page.ByDataAttribute(val: selectors.MakeAPaymentBillIssueDate));
        }
        private void AssertAmountDueDisplayed()
        {
            page.IsElementPresent(page.ByDataAttribute(val: selectors.LastPaymentCurrentBalanceAmount));
        }
        private void AssertMakeAPaymentHeaderDisplayed()
        {
            page.IsElementPresent(page.ByDataAttribute(val: selectors.MakeAPaymentTitleTestId));
        }

        internal void AssertTooLowNotifier()
        {
            page.IsElementTextPresent(TextMatch.PaymentsPage.AmountDueIsTooLow);
        }

        internal void AsserElevonPanel()
        {
            page.IsElementTextNotPresent(TextMatch.PaymentsPage.AmountDueIsTooLow);
        }
    }
}
