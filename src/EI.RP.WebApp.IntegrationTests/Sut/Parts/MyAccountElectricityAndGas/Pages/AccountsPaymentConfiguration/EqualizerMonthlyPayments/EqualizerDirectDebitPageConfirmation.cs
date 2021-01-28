using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration.
	EqualizerMonthlyPayments
{
	internal class EqualizerDirectDebitPageConfirmation : MyAccountElectricityAndGasPage
	{
		public EqualizerDirectDebitPageConfirmation(ResidentialPortalApp app) : base(app)
		{
		}

		public IHtmlElement Page =>
			(IHtmlElement) Document.QuerySelector("[data-page='direct-debit-confirmation-equalizer']");

		public IHtmlHeadingElement SectionHeader =>
			(IHtmlHeadingElement) Page.QuerySelector("[data-testid='direct-debit-confirmation-thank-you']");

		public IHtmlAnchorElement BackToEqualizerDirectDebitEditLink =>
			(IHtmlAnchorElement) Page.QuerySelector(
				"[data-testid='direct-debit-confirmation-back-to-edit-direct-debit']");

		public IHtmlAnchorElement BackToAccountsLink =>
			(IHtmlAnchorElement) Page.QuerySelector("[data-testid='direct-debit-confirmation-back-to-my-accounts']");

		protected override bool IsInPage()
		{
			var isInPage = base.IsInPage()
			               && Page != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("Equal Monthly Payments"));
			}

			return isInPage;
		}
	}
}