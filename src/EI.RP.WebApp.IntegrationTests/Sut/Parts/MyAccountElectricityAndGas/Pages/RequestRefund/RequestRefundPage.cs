using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.RequestRefund
{
    internal class RequestRefundPage : MyAccountElectricityAndGasPage
    {
        public RequestRefundPage(ResidentialPortalApp app) : base(app)
        {
        }

        protected override bool IsInPage()
        {
            var isInPage = Page != null;

            if (isInPage)
            {
	            AssertTitle(App.ResolveTitle("Request Refund"));
            }

            return isInPage;
        }

        public IHtmlElement Page => 
            Document.QuerySelector("[data-page='request-refund']") as IHtmlElement;

        public IHtmlElement AccountLabel => 
	        Document.QuerySelector("#lblAccount") as IHtmlElement;

        public IHtmlElement AccountText =>
	        Document.QuerySelector("#txtAccount") as IHtmlElement;

        public IHtmlElement CommentLabel => 
	        Document.QuerySelector("#lblComments") as IHtmlElement;

        public IHtmlElement CommentText => 
	        Document.QuerySelector("#txtComments") as IHtmlElement;

        public IHtmlElement AccountCreditLabel => 
	        Document.QuerySelector("#lblAccountCredit") as IHtmlElement;

        public IHtmlElement AccountCreditText => 
	        Document.QuerySelector("#txtAccountCredit") as IHtmlElement;

        public IHtmlElement SubmitRequestRefund => 
	        Document.QuerySelector("#btnRequestRefund") as IHtmlElement;
    }
}