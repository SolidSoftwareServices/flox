using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using EI.RP.CoreServices.System;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.EstimateYourCost
{
	internal class EstimateYourCostPage : MyAccountElectricityAndGasPage
	{
		public EstimateYourCostPage(ResidentialPortalApp app) : base(app)
		{
		}

		public IHtmlElement PageContainer =>
			(IHtmlElement) Document
				.QuerySelector("[data-page='estimate-your-costs']");

		public EuroMoney EstimatedAmount =>
			PageContainer
				.QuerySelector("[data-testid='estimate-your-costs-amount']")
				.TextContent;

		public IHtmlAnchorElement ButtonMakeAPayment =>
			(IHtmlAnchorElement) PageContainer
				.QuerySelector("[data-testid='estimate-your-costs-make-a-payment-link']");

		protected override bool IsInPage()
		{
			var isInPage = PageContainer != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("Estimate your costs"));
			}

			return isInPage;
		}

		public async Task<ResidentialPortalApp> ClickButtonMakeAPayment()
		{
			return (ResidentialPortalApp) await ClickOnElement(ButtonMakeAPayment);
		}
	}
}