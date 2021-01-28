using OpenQA.Selenium;
using System;
using EI.RP.WebApp.RegressionTests.PageObjects;
using System.Threading;
using IDictionary = System.Collections.Generic.IDictionary<string, string>;
using static EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils.SharedVariables;
using System.Collections;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects;
using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.StepDefinitions;
using EI.RP.WebApp.AcceptanceTests.Infrastructure;

namespace EI.RP.WebApp.UITestAutomation
{
    public class SubmitRefundSteps : BaseStep
    {
        public SubmitRefundSteps(SingleTestContext shared) : base(shared)
        {
        }
        private MyAccountsPage MyAccountsPage => new MyAccountsPage(shared.Driver.Value);

        private EI.RP.WebApp.RegressionTests.PageObjects.RequestRefundPage RequestRefundPage => new EI.RP.WebApp.RegressionTests.PageObjects.RequestRefundPage(shared.Driver.Value);

        public void WhenClickSubmitRefundRequestButton(IDictionary dict)
        {
            MyAccountsPage.ClickRequestRefundSpecificAccount(dict);
            RequestRefundPage.AssertRequestRefundForm();
        }

        public void WhenEnterAdditionalInformation(string s)
        {
            RequestRefundPage.EnterAdditionalInformation(s);
        }

        public void WhenClickSubmitOnRefundForm()
        {
            RequestRefundPage.ClickSubmitBtn();
        }

        public void ThenShouldBeSentToRefundConfirmationPage()
        {
            RequestRefundPage.AssertRequestRefundConfirmationPage();
        }

        internal void ThenErrorTooLong()
        {
            RequestRefundPage.AssertErrorTooLong();
        }

        internal void ThenAccountCreditShouldBe(string v)
        {
            RequestRefundPage.AssertAccountCredit(v);
        }
    }
}
