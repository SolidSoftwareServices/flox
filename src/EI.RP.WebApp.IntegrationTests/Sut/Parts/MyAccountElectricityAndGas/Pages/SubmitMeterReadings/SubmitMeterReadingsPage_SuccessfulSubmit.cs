using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SubmitMeterReadings
{
	internal class SubmitMeterReadingsPage_SuccessfulSubmit : SubmitMeterReadingsPage
	{
		public SubmitMeterReadingsPage_SuccessfulSubmit(ResidentialPortalApp app) : base(app)
		{
		}

		protected override bool IsInPage()
		{
			var isInPage = Document.QuerySelector("[data-page='meter-reading-success']") != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("Meter Reading"));
			}

			return isInPage;
		}
	}
}