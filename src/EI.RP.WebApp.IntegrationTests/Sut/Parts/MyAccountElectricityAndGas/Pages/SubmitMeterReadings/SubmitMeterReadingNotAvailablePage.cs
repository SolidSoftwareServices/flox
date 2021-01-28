using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SubmitMeterReadings
{
	internal class SubmitMeterReadingNotAvailablePage : MyAccountElectricityAndGasPage
	{
		public SubmitMeterReadingNotAvailablePage(ResidentialPortalApp app) : base(app)
		{
		}
		protected override bool IsInPage()
		{
			var isInPage = Page != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("Meter Reading"));
			}

			return isInPage;
		}
		public IHtmlElement Page => Document.QuerySelector("[data-page='meter-reading-not-present']") as IHtmlElement;
		public IHtmlHeadingElement Heading => Page.QuerySelector("[data-testid='meter-reading-title']") as IHtmlHeadingElement;
	}
}