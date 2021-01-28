using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SubmitMeterReadings
{
	internal class SubmitMeterReadingsPage_MeterInput6 : SubmitMeterReadingsPage_MeterInput1
	{
		public SubmitMeterReadingsPage_MeterInput6(ResidentialPortalApp app) : base(app)
		{
		}

		protected override bool IsInPage()
		{
			var isInPage = Document.QuerySelector("[data-page='meter-reading']") != null
			       && MeterReadingInput1 != null && MeterReadingInput2 != null && MeterReadingInput3 != null
			       && MeterReadingInput4 != null && MeterReadingInput5 != null && MeterReadingInput6 != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("Meter Reading"));
			}

			return isInPage;
		}
	}
}