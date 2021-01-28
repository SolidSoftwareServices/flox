using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SubmitMeterReadings
{
	internal class SubmitMeterReadingsPage_SubmitErrorLessThanActual : SubmitMeterReadingsPage
	{
		public SubmitMeterReadingsPage_SubmitErrorLessThanActual(ResidentialPortalApp app) : base(app)
		{
		}

		public IHtmlElement Page =>
			(IHtmlElement) Document.QuerySelector("[data-page='meter-reading-error-submitted-less-than-actual']");

		public IHtmlElement MeterReadingErrorTitle =>
			Page.QuerySelector("[data-testid='meter-reading-error-title']") as IHtmlHeadingElement;

		public IHtmlElement MeterReadingErrorMessage =>
			Page.QuerySelector("[data-testid='meter-reading-error-message']") as IHtmlParagraphElement;

		protected override bool IsInPage()
		{
			var isInPage = Page != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("Meter Reading"));
			}

			return isInPage;
		}
	}
}