using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration.
	EqualizerMonthlyPayments
{
	internal class SetupEqualizerMonthlyPaymentsPage : MyAccountElectricityAndGasPage
	{
		public SetupEqualizerMonthlyPaymentsPage(ResidentialPortalApp app) : base(app)
		{
		}

		public IHtmlSpanElement EqualMonthlyPaymentAmount =>
			(IHtmlSpanElement) Document.QuerySelector("[data-testid='equalizer-setup-monthly-payment-amount']");

		public IHtmlInputElement DatePickerInputElement =>
			(IHtmlInputElement) Document.QuerySelector("[data-testid='equalizer-setup-first-payment-date']");

		public IHtmlButtonElement SetDirectDebitButton =>
			(IHtmlButtonElement) Document.QuerySelector("[data-testid='equalizer-setup-set-up-direct-debit']");

		protected override bool IsInPage()
		{
			var isInPage = base.IsInPage() && Document.QuerySelector("[data-page='equalizer-setup']") != null;

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