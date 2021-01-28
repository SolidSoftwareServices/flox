using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration.
	EqualizerMonthlyPayments
{
	internal class SetupEqualizerDirectDebitPage : MyAccountElectricityAndGasPage
	{
		public SetupEqualizerDirectDebitPage(ResidentialPortalApp app) : base(app)
		{
		}

		public IHtmlElement Page =>
			(IHtmlElement) Document.QuerySelector("[data-page='equaliser-direct-debit']");

		public IHtmlParagraphElement PaymentDate =>
			(IHtmlParagraphElement) Page.QuerySelector("[data-testid='equaliser-direct-debit-payment-date']");

		public IHtmlInputElement Iban =>
			(IHtmlInputElement) Page.QuerySelector("#iban");

		public IHtmlInputElement CustomerName =>
			(IHtmlInputElement) Page.QuerySelector("#customer-name");

		public IHtmlInputElement CheckBox =>
			(IHtmlInputElement) Page.QuerySelector("#terms");

		public IHtmlButtonElement CompleteDirectDebitButton =>
			(IHtmlButtonElement) Page.QuerySelector("[data-testid='equaliser-direct-debit-complete-setup']");

		protected override bool IsInPage()
		{
			var isInPage = base.IsInPage() && Page != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("Equal Monthly Payments"));
			}

			return isInPage;
		}
	}
}