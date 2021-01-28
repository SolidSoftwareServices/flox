using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration.
	DirectDebit
{
	internal class DirectDebitPageConfirmation : MyAccountElectricityAndGasPage
	{
		public DirectDebitPageConfirmation(ResidentialPortalApp app) : base(app)
		{
		}

		public IHtmlElement Page =>
			(IHtmlElement) Document.QuerySelector("[data-page='direct-debit-confirmation']");

		public IHtmlHeadingElement SectionHeader =>
			(IHtmlHeadingElement) Page.QuerySelector("[data-testid='direct-debit-confirmation-thank-you']");

		public IHtmlAnchorElement BackToBillAndPaymentOptionsLink =>
			(IHtmlAnchorElement) Page.QuerySelector(
				"[data-testid='direct-debit-confirmation-back-to-billing-and-payment']");

		public IHtmlAnchorElement BackToAccountsLink =>
			(IHtmlAnchorElement) Page.QuerySelector("[data-testid='direct-debit-confirmation-back-to-my-accounts']");

		protected override bool IsInPage()
		{
			return base.IsInPage()
			       && Page != null;
		}
	}
}