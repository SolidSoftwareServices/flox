using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SubmitMeterReadings
{
	internal class SubmitMeterReadingsPage_MeterInput1 : SubmitMeterReadingsPage
	{
		public SubmitMeterReadingsPage_MeterInput1(ResidentialPortalApp app) : base(app)
		{
		}

		public IHtmlInputElement MeterReadingInput1 =>
			Document.QuerySelector("[data-testid='reading-value-input-1']") as IHtmlInputElement;

		public IHtmlInputElement MeterReadingInput2 =>
			Document.QuerySelector("[data-testid='reading-value-input-2']") as IHtmlInputElement;

		public IHtmlInputElement MeterReadingInput3 =>
			Document.QuerySelector("[data-testid='reading-value-input-3']") as IHtmlInputElement;

		public IHtmlInputElement MeterReadingInput4 =>
			Document.QuerySelector("[data-testid='reading-value-input-4']") as IHtmlInputElement;

		public IHtmlInputElement MeterReadingInput5 =>
			Document.QuerySelector("[data-testid='reading-value-input-5']") as IHtmlInputElement;

		public IHtmlInputElement MeterReadingInput6 =>
			Document.QuerySelector("[data-testid='reading-value-input-6']") as IHtmlInputElement;

		public IHtmlElement MeterReadingHistoryTable =>
			Document.QuerySelector("[data-testid='meter-reading-history-table']") as IHtmlTableElement;

		public IHtmlElement MeterReadingHistoryTableHeading =>
			MeterReadingHistoryTable?.QuerySelector(
				"thead > tr:first-child [data-testid='meter-reading-history-table-heading']") as IHtmlElement;

		public IHtmlElement MeterReadingHistoryTableFirstReading =>
			MeterReadingHistoryTable?.QuerySelector(
				"tbody > tr:first-child [data-testid='meter-reading-history-table-reading']") as IHtmlElement;

		public IHtmlElement MeterReadingHistoryTableMobile =>
			Document.QuerySelector("[data-testid='meter-reading-history-table-mobile']") as IHtmlTableElement;

		public IHtmlElement MeterReadingHistoryTableMobileHeading =>
			MeterReadingHistoryTableMobile?.QuerySelector(
				"tr:first-child [data-testid='meter-reading-history-table-mobile-heading']") as IHtmlElement;

		public IHtmlElement MeterReadingHistoryTableMobileFirstReading =>
			MeterReadingHistoryTableMobile?.QuerySelector(
				"tr:first-child [data-testid='meter-reading-history-table-mobile-reading']") as IHtmlElement;

		public IHtmlElement MeterReadingInput1Error =>
			Document.QuerySelector("[data-valmsg-for='MeterReadings[0].ReadingValue']") as IHtmlElement;

		public IHtmlElement MeterReadingInput2Error =>
			Document.QuerySelector("[data-valmsg-for='MeterReadings[1].ReadingValue']") as IHtmlElement;

		public IHtmlElement MeterReadingInput3Error =>
			Document.QuerySelector("[data-valmsg-for='MeterReadings[2].ReadingValue']") as IHtmlElement;

		public IHtmlElement MeterReadingInput4Error =>
			Document.QuerySelector("[data-valmsg-for='MeterReadings[3].ReadingValue']") as IHtmlElement;

		public IHtmlElement MeterReadingInput5Error =>
			Document.QuerySelector("[data-valmsg-for='MeterReadings[4].ReadingValue']") as IHtmlElement;

		public IHtmlElement MeterReadingInput6Error =>
			Document.QuerySelector("[data-valmsg-for='MeterReadings[5].ReadingValue']") as IHtmlElement;


		public IHtmlElement HowDoIReadMessageLink =>
			Document.QuerySelector("[data-testid='meter-reading-how-do-i-read-message-link']") as IHtmlElement;

		public IHtmlAnchorElement ForOtherMetersLink => Document.QuerySelector("[data-test-id='for-other-meters-link']") as IHtmlAnchorElement;

        public IHtmlHeadingElement MeterReadingH2Header =>
            Document.QuerySelector("[data-testid='meter-reading-history-title']") as IHtmlHeadingElement;

        protected override bool IsInPage()
		{
			var isInPage = Document.QuerySelector("[data-page='meter-reading']") != null
			       && MeterReadingInput1 != null && MeterReadingInput2 == null && MeterReadingInput3 == null
			       && MeterReadingInput4 == null && MeterReadingInput5 == null && MeterReadingInput6 == null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("Meter Reading"));
			}

			return isInPage;
		}
	}
}