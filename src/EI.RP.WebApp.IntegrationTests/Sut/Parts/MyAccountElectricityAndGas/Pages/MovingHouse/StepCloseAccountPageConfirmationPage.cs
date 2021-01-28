using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse
{
	internal class StepCloseAccountPageConfirmationPage : MyAccountElectricityAndGasPage
	{
		public StepCloseAccountPageConfirmationPage(ResidentialPortalApp app) : base(app)
		{
		}

		public IHtmlElement Page =>
			Document.QuerySelector("[data-page='mimo-close-accounts-confirmation']") as IHtmlElement;

		public IHtmlAnchorElement BackToMyAccountsLink =>
			Page.QuerySelector("[data-id='dds_back_to_my_accounts']") as IHtmlAnchorElement;

		protected override bool IsInPage()
		{
			var isInPage = base.IsInPage() && Page != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("Confirmation | Close Account"));
			}

			return isInPage;
		}

		public IHtmlDivElement GetGasAccountInfo()
		{
			return Page.QuerySelector("#gasAccountInfo") as IHtmlDivElement;
		}

		public IHtmlDivElement GetElectricityAccountInfo()
		{
			return Page.QuerySelector("#electricityAccountInfo") as IHtmlDivElement;
		}

		public IHtmlParagraphElement GetElectricityPaymentInfo()
		{
			return Page.QuerySelector("#electricityPaymentInfo") as IHtmlParagraphElement;
		}

		public IHtmlParagraphElement GetGasPaymentInfo()
		{
			return Page.QuerySelector("#gasPaymentInfo") as IHtmlParagraphElement;
		}
	}
}