using EI.RP.WebApp.AcceptanceTests.AcceptanceTests.PageObjects;
using EI.RP.WebApp.AcceptanceTests.Infrastructure;
using IDictionary = System.Collections.Generic.IDictionary<string, string>;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.StepDefinitions
{
    public class RefundSteps : BaseStep
    {
        public RefundSteps(SingleTestContext shared) : base(shared)
        {
        }

        private MyAccountsPage MyAccountsPage => new MyAccountsPage(shared.Driver.Value);

        private RequestRefundPage RequestRefundPage => new RequestRefundPage(shared.Driver.Value);

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

    }
}
