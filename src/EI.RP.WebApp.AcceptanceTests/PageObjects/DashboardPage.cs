using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using EI.RP.WebApp.AcceptanceTests.StepDefinitions;
using TechTalk.SpecFlow;
using EI.RP.WebApp.AcceptanceTests.Utils;
using IDictionary = System.Collections.Generic.IDictionary<string, string>;
using System.Linq;

namespace EI.RP.WebApp.AcceptanceTests.PageObjects
{
    class DashBoardPage
    {
        protected IWebDriver Driver { get; set; }
        private readonly SharedPageFunctions _sharedPageFunc = null;

        internal DashBoardPage(IWebDriver driver0)
        {
            Driver = driver0;
            _sharedPageFunc = new SharedPageFunctions(Driver);
        }

        WebDriverWait wait;
        internal string overrideName = "overridelink";
        internal string goToMyAccountID = "goToAccountBtn",
            CancelID = "messagePaygCancelBtn",
            clickHereID = "clickHereHyperlink",
            messagePAYGCloseID = "messagePaygClose",
            competitionCloseID = "boxclose",
            promoCloseID = "promoBoxClose",
            promoID = "imgPromotionBanner",
            competitionID = "imgcmpetitionentry",
            payzoneHeaderID = "",
            dashboardHeaderID = "",
            messageHeaderID = "",
            myClosedAccountsID = "lnkAcclistClosedAccounts",
            meterReadingTabID = "meterReadingTab",
            accountOverviewTabID = "accountOverviewTab",
            billsAndPaymentsTabID = "billsAndPaymentsTab",
            makeAPaymentTabID = "paymentTab",
            contactUsMenuItemID = "contactUsMenuItem";

        internal void NavigateToClosedAccounts()
        {
            _sharedPageFunc.ClickElement(By.Id(myClosedAccountsID));
        }
        internal void NavigateToTypeAccounts(string type)
        {
            _sharedPageFunc.ClickElement(By.XPath("//span[contains(text(),'" + type + "')]"));
        }
        internal void ClickViewFullAccountDetailsSpecificAccount(string defaultPageUrl, Table table)
        {
            var dictionary = TableExtensions.ToDictionary(table);
            var account = dictionary["Account"];

            _sharedPageFunc.ClickElement(_sharedPageFunc.ByDataAttribute(val: $"account-card-view-this-account-{account}"));
        }
        internal void AssertMessage()
        {
            _sharedPageFunc.ClickElement(By.Id(messageHeaderID));
        }
        internal void NavigateToDashboardSpecificUser(string defaultPageUrl, IDictionary dict)
        {
            var signInPage = new SignInPage(Driver);
			Driver.Navigate().GoToUrl(defaultPageUrl);
        }
        internal void ClickPAYGMessageClose()
        {
            _sharedPageFunc.ClickElement(By.Id(messagePAYGCloseID));
        }
        internal void ClickPromoBoxClose()
        {
            _sharedPageFunc.ClickElement(By.Id(promoCloseID));
        }
        internal void ClickCompetitionBoxClose()
        {
            _sharedPageFunc.ClickElement(By.Id(competitionCloseID));
        }
        internal void AssertHeader(string headerText)
        {
            _sharedPageFunc.IsElementPresent(By.XPath("//h1[contains(text(),'" + headerText + "')]"));
        }
        internal void AssertBody(string bodyText)
        {
            _sharedPageFunc.IsElementPresent(By.XPath("//p[contains(text(),'" + bodyText + "')]"));
        }
        internal void AssertMessageText(string messageText)
        {
            _sharedPageFunc.IsElementPresent(By.XPath("//h1[contains(text(),'" + messageText + "')]"));
        }
        internal void ClickGoToMyAccount()
        {
            _sharedPageFunc.ClickElement(By.Id(goToMyAccountID));
        }
        internal void AssertDashboard()
        {
            _sharedPageFunc.IsElementPresent(By.Id(dashboardHeaderID));
        }
        internal void ClickClickHereLink()
        {
            _sharedPageFunc.ClickElement(By.Id(clickHereID));
        }
        internal void AssertPayZonePage()
        {
            throw new System.InvalidOperationException(SharedThings.IDsFlag);
            _sharedPageFunc.IsElementPresent(By.Id(payzoneHeaderID));
        }
        internal void ClickCancel()
        {
            _sharedPageFunc.ClickElement(By.Id(CancelID));
        }
        internal void ClickMakeAPaymentNavBtn()
        {
            _sharedPageFunc.ClickElement(By.Id(makeAPaymentTabID));
            var makeAPaymentPage = new MakeAPaymentPage(Driver);
            makeAPaymentPage.AssertMakeAPaymentPage();
        }
        internal void ClickAccountOverviewNavBtn()
        {
            _sharedPageFunc.ClickElement(By.Id(accountOverviewTabID));
        }
        internal void ClickBillsAndPaymentsNavBtn()
        {
            _sharedPageFunc.ClickElement(By.Id(billsAndPaymentsTabID));
            var billsAndPaymentsPage = new BillsAndPaymentsPage(Driver);
            billsAndPaymentsPage.AssertBillsAndPaymentsPage();
        }
        internal void ClickMeterReadingNavBtn()
        {
            _sharedPageFunc.ClickElement(By.Id(meterReadingTabID));
            var submitMeterReadingPage = new SubmitMeterReadingPage(Driver);
            submitMeterReadingPage.AssertMeterReadingPage();
        }
        internal void ClickContactUsForm()
        {
            _sharedPageFunc.ClickElement(By.Id(contactUsMenuItemID));
            var contactUsFormPage = new ContactUsFormPage(Driver);
            contactUsFormPage.AssertContactUsFormBeforeSelectQueryType();
        }
		internal void ClickMyAccountsNavBtn()
		{
            if (_sharedPageFunc.GetCurrentLayoutType() == LayoutType.Smart && _sharedPageFunc.GetCurrentNavigationType() == LayoutType.Smart)
            {
                _sharedPageFunc.ClickElement(_sharedPageFunc.ByDataAttribute(val: "main-navigation-my-accounts-link"));
            }
            else
            {
                if (Driver.FindElements(By.Id("btnMyAccounts")).Any())
                {
                    _sharedPageFunc.ClickElement(Driver.FindElement(By.Id("btnMyAccounts")));
                }
                _sharedPageFunc.ClickElement(By.Id("dds_back_to_my_accounts"));               
            }
		}
        
	}
}
