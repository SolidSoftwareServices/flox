using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MakeAPayment
{
	internal abstract class MakeAPaymentPage : MyAccountElectricityAndGasPage
	{
		protected MakeAPaymentPage(ResidentialPortalApp app) : base(app)
		{
		}

		public LastPayment ShowPaymentDetails => new LastPayment(Document);

		protected override bool IsInPage()
		{
			var isInPage = Document.QuerySelector("[data-page='make-a-payment']") != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("Payments"));
			}

			return isInPage;
		}
		public IHtmlElement HPPPostResponse => Document.QuerySelector("input[name='HPP_POST_RESPONSE']") as IHtmlElement;
		public class LastPayment
		{
			public LastPayment(IHtmlDocument document)
			{
				Document = document;
			}

			public IHtmlDocument Document { get; }

			public string AmountDue => Document.QuerySelector("[data-testid='last-payment-current-balance-amount']")
				.TextContent;


			public string BillIssueDate =>
				Document.QuerySelector("[data-testid='make-a-payment-bill-issue-date']").TextContent;

			public string PaymentDueDate =>
				Document.QuerySelector("[data-testid='make-a-payment-payment-due-date']").TextContent;

			public string NextBillDate =>
				Document.QuerySelector("[data-testid='make-a-payment-next-bill-date']").TextContent;

			//public IElement DirectDebitHeader => Document.QuerySelector("#directDebitDiv > h4");

			//public string ElectricityPAYGCustomerHeader => Document.QuerySelector("#electicityPAYGCustomerHeader").TextContent;

			//public string ElectricityPAYGCustomerContent => Document.QuerySelector("#electricityPAYGCustomerContent").TextContent;

			public string ShowPayDifferentAmountButton =>
				Document.QuerySelector("[data-testid='last-payment-pay-different-amount']").TextContent;
		}
	}
}