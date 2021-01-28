using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse
{
	internal class ShowMovingHouseValidationErrorPage : MovingHomePage
	{
		public ShowMovingHouseValidationErrorPage(ResidentialPortalApp app) : base(app)
		{
		}

		protected override bool IsInPage()
		{
			var isInPage = base.IsInPage() && Page != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("Moving House"));
			}

			return isInPage;
		}

		public IHtmlElement Page => Document.QuerySelector("[data-page='moving_house_validation_error_page']") as IHtmlElement;

		public IHtmlElement ErrorMessageBody => Document.QuerySelector("[data-testid='error_message_body']") as IHtmlElement;

		public IHtmlElement ErrorMessageTitle => Document.QuerySelector("[data-testid='error_message_title']") as IHtmlElement;
	}
}