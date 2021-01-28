using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration
{
	internal class ShowPaymentsHistoryPage : MyAccountElectricityAndGasPage
    {
        public ShowPaymentsHistoryPage(ResidentialPortalApp app) : base(app)
        {
        }


        protected override bool IsInPage()
        {
            var result = false;

            if (ShowPaymentHistoryPage != null)
            {
                Overview = new OverviewStep(Document);
                AssertTitle(App.ResolveTitle("Bills & Payments"));
                result = true;
            }
            else if (Document.QuerySelector("[data-page='direct-debit-settings']") != null)
            {
                Overview = null;
                result = true;
            }

            return result;
        }

        public IHtmlElement ShowPaymentHistoryPage => Document.QuerySelector("[data-page='payments-history']") as IHtmlElement;
        public IHtmlElement PaymentOverDue => ShowPaymentHistoryPage.QuerySelector("[data-testid='payment-overdue']") as IHtmlElement;

		public async Task<ResidentialPortalApp> ClickButton(IHtmlButtonElement button)
        {
            return (ResidentialPortalApp)await App.ClickOnElement(button);
        }

        public OverviewStep Overview { get; set; }

        public class OverviewStep
        {
            public IHtmlDocument Document { get; }

            public OverviewStep(IHtmlDocument document)
            {
                Document = document;
            }

            public IHtmlAnchorElement BillAndPaymentOptionsLink => (IHtmlAnchorElement)Document.QuerySelector("[data-testid='payments-history-change-billing-preferences-link']");
            public IHtmlAnchorElement HowToReadLink => (IHtmlAnchorElement)Document.QuerySelector("[data-testid='how-to-read-bill-link']");
            public IHtmlElement NoPaymentAppliedMessage => (IHtmlElement)Document.QuerySelector("[data-testid='no-payment-applied-msg']");
            public IHtmlElement PaymentMessage => (IHtmlElement)Document.QuerySelector("[data-testid='payment-msg']");
        }
    }
}