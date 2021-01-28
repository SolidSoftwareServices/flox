using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration.
	EqualizerMonthlyPayments
{
	internal class EqualizerMonthlyPaymentsPage : MyAccountElectricityAndGasPage
	{
		public EqualizerMonthlyPaymentsPage(ResidentialPortalApp app) : base(app)
		{
		}

		public IHtmlElement EqualMonthlyPaymentAmount =>
			(IHtmlElement) Document.QuerySelector("[data-testid='equalizer-landing-monthly-payment']");

		public IHtmlElement HighestBill =>
			(IHtmlElement) Document.QuerySelector("[data-testid='equalizer-landing-highest-bill-amount']");

		public IHtmlElement HighestInvoiceDate =>
			(IHtmlElement) Document.QuerySelector("[data-testid='equalizer-landing-highest-bill-date']");

		public IHtmlElement LowestBill =>
			(IHtmlElement) Document.QuerySelector("[data-testid='equalizer-landing-lowest-bill-amount']");

		public IHtmlElement LowestInvoiceDate =>
			(IHtmlElement) Document.QuerySelector("[data-testid='equalizer-landing-lowest-bill-date']");

		public IHtmlButtonElement SetupEqualMonthlyPaymentsBtn =>
			(IHtmlButtonElement) Document.QuerySelector("[data-testid='equalizer-landing-set-up']");

		protected override bool IsInPage()
		{
			var isInPage = base.IsInPage() && Document.QuerySelector("[data-page='equalizer-landing']") != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("Equal Monthly Payments"));
			}

			return isInPage;
		}

		public async Task<ResidentialPortalApp> ClickButton(IHtmlButtonElement button)
		{
			return (ResidentialPortalApp) await App.ClickOnElement(button);
		}
	}
}