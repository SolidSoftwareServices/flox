using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SubmitMeterReadings
{
	internal abstract class SubmitMeterReadingsPage : MyAccountElectricityAndGasPage
	{
		protected SubmitMeterReadingsPage(ResidentialPortalApp app) : base(app)
		{
		}

		public IHtmlButtonElement SubmitButton =>
			Document.QuerySelector("[data-testid='submit-meter-reading-button']") as IHtmlButtonElement;

		protected override bool IsInPage()
		{
			var isInPage = Page != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("Meter Reading"));
			}

			return isInPage;
		}

		public IHtmlElement Page => (IHtmlElement) Document.QuerySelector("[data-page='meter-reading']");
	}
}