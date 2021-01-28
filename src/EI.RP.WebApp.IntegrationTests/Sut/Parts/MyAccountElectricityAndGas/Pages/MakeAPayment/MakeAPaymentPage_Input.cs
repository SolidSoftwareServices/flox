using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MakeAPayment
{
	internal class MakeAPaymentPage_Input : MakeAPaymentPage
	{
		public MakeAPaymentPage_Input(ResidentialPortalApp app) : base(app)
		{
		}

		protected override bool IsInPage()
		{
			var isInPage = base.IsInPage();

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("Payments"));
			}

			return isInPage;
		}

		public IHtmlButtonElement PayDifferentAmountButton()
		{
			return Document.QuerySelector("[data-testid='last-payment-pay-different-amount']") as IHtmlButtonElement;
		}

		public async Task<ResidentialPortalApp> ClickPayDifferentAmountButton()
		{
			return (ResidentialPortalApp) await App.ClickOnElement(PayDifferentAmountButton());
		}
	}
}