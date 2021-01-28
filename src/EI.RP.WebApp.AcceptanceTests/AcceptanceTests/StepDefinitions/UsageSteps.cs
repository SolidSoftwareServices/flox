using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects;
using EI.RP.WebApp.AcceptanceTests.Infrastructure;
using IDictionary = System.Collections.Generic.IDictionary<string,string>;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.StepDefinitions
{
    public class UsageSteps : BaseStep
    {
        public UsageSteps(SingleTestContext shared) : base(shared)
        {
        }
        private UsagePage usagePage => new UsagePage(shared.Driver.Value);

        public void GivenOnUsagePageOfAccount(IDictionary dict)
        {
            MyAccountsPage myAccountsPage = new MyAccountsPage(shared.Driver.Value);
            myAccountsPage.ClickViewFullAccountDetailsSpecificAccount(dict);
        }
        internal void ThenUsageShouldBeDisplayed()
        {
            usagePage.ChartIsDisplayed();
        }

        internal void WhenClickCompareYears()
        {
            usagePage.ClickCompareYears();
        }
        internal void WhenClickCompareNow()
        {
            usagePage.ClickCompareNow();
        }

        internal void WhenClickKWH()
        {
            usagePage.ClickKWH();
        }

        internal void WhenClickNextYear()
        {
            usagePage.ClickNextYear();
        }

        internal void WhenClickPrevYear()
        {
            usagePage.ClickPrevYear();
        }

        internal void WhenClickYear()
        {
            usagePage.ClickKWH();
        }
    }
}
