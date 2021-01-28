using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SubmitMeterReadings
{
	internal class SubmitMeterReadingPage_SmartMeter : SubmitMeterReadingsPage
	{
		public SubmitMeterReadingPage_SmartMeter(ResidentialPortalApp app) : base(app)
		{
		}

		protected override bool IsInPage()
		{
			var isInPage = Page != null && MeterReadingSection == null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("Meter Reading"));
			}

			return isInPage;
		}

		public IHtmlElement Page => Document.QuerySelector("[data-page='meter-reading']") as IHtmlElement;
		public IHtmlElement MeterReadingSection => Page.QuerySelector("[data-testid='submit-meter-reading']") as IHtmlElement;
	}
}
