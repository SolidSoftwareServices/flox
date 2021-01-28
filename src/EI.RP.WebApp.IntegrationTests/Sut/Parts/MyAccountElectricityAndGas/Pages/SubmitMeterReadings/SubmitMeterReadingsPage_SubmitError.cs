using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SubmitMeterReadings
{
	internal class SubmitMeterReadingsPage_SubmitError : SubmitMeterReadingsPage
	{
		public SubmitMeterReadingsPage_SubmitError(ResidentialPortalApp app) : base(app)
		{
		}

		public IHtmlElement MeterReadingErrorTitle =>
			Document.QuerySelector("[data-testid='meter-reading-error-title']") as IHtmlHeadingElement;

		public IHtmlElement MeterReadingErrorMessage =>
			Document.QuerySelector("[data-testid='meter-reading-error-message']") as IHtmlParagraphElement;

		protected override bool IsInPage()
		{
			var isInPage = Document.QuerySelector("[data-page='meter-reading-error']") != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("Meter Reading"));
			}

			return isInPage;
		}
	}
}