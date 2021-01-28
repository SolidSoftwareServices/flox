namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils
{
	public static class XPathSelectors
	{
		public static class NavMenu
		{
			public static string help = "//*[@id='helpMenuItem']/a";
		}

		public static class ContactUsPage
		{
			public static string contactUsConfirmationHeader = "/html/body/div[2]/div/main/div/div/div/h2";
		}

		public static class PaymentsPage
		{
			internal static readonly string PAYGBox = "/html/body/div[2]/div[2]/main/div/div/div[4]/div/div/p[1]";
			public static string ElevonName = "//*[@id='pas_ccnum']";
			public static string PaymentValue = "/html/body/div[2]/div[2]/main/div/div/div[1]/div[1]/p[2]";
			public static string moreAboutEqual = "//*[@id='AccountsPaymentConfiguration-']/button";
		}

		public static class MeterReadingResultPage
		{
			public static string rejectHeader = "/html/body/main/div/div/div[1]/h2";
			public static string rejectReason = "/html/body/div[2]/div[2]/main/div/div/div[1]/p";
			public static string backToMyAccounts = "/html/body/main/div/div/div[1]/a";
			internal static string acceptReason = "/html/body/div[2]/div[2]/main/div/div/div[1]/p";
		}

		public static class PlanPage
		{
			public static string No = "//*[@id='paperless-confirmation']/div/div/div[3]/button[1]";
			public static string Yes = "//*[@id='paperless-confirmation']/div/div/div[3]/button[2]";
		}

		public static class MyAccountsPage
		{
			public static string FirstAccount = "//*[@id='Accounts-']/button";

		}


		public static class UsagePage
		{
			public static string compareYears =
				"/html/body/div[2]/div[2]/main/div/div/div[1]/div/div[2]/div[3]/div/div/div/label[2]";

			public static string KWH = "//html/body/div[2]/div[2]/main/div/div/div[1]/div/div[2]/div[1]/div[2]/label";
			public static string prevYear = "//*[@id='pills-year']/div/button[1]";
			public static string nextYear = "//*[@id='pills-year']/div/button[2]";

			public static string amount =
				"/html/body/div[2]/div[2]/main/div/div/div[1]/div/div[2]/div[2]/div[4]/div[3]/span";

			public static string year = "//*[@id='pills-year']/div/a[2]";
			public static string year1 = "//*[@id='pills-year']/div/a[1]";
			public static string year2 = "//*[@id='pills-year']/div/a[2]";
			public static string chart = "//*[@id='highcharts-bh1l2ty-0']/svg/g[7]";

			public static string compFirst =
				"//html/body/div[2]/div[2]/main/div/div/div[1]/div/div[2]/div[2]/div[2]/div[2]/span";

			public static string compSecond =
				"//html/body/div[2]/div[2]/main/div/div/div[1]/div/div[2]/div[2]/div[4]/div[3]/span";

		}

		public static class SubmitMeterReadingPage
		{
			public static string histMeterType = "/html/body/main/div/div/table[1]/thead/tr/th[1]";
			public static string histFromDate = "/html/body/main/div/div/table[1]/thead/tr/th[2]";
			public static string histToDate = "/html/body/main/div/div/table[1]/thead/tr/th[3]";
			public static string histReading = "/html/body/main/div/div/table[1]/thead/tr/th[4]";
			public static string histType = "/html/body/main/div/div/table[1]/thead/tr/th[5]";
			public static string histConsumption = "/html/body/main/div/div/table[1]/thead/tr/th[6]";
			public static string meterID = "/html/body/div/main/div/div/div[1]/p[1]";
			public static string mprnInfo = "/html/body/div/main/div/div/div[1]/p[2]";
			public static string submitHeader = "//*[@id='MeterReadings-']/div/h2";
			public static string elecMeterInputLabel = "//*[@id='MeterReadings-']/div/div[1]/label";
			public static string gasMeterInputLabel = "//*[@id='MeterReadings-']/div/div[1]/label";
			public static string howDoIReadMyMeterBtnLabel = "//*[@id='MeterReadings-']/div/div[1]/p";
			public static string howDoIReadMyMeterBtn = "//*[@id='MeterReadings-']/div/div[1]/p/span";
			public static string kWh = "//*[@id='MeterReadings-']/div/div[1]/div[1]/div/span";
			public static string dNKWH1 = "//*[@id='MeterReadings-']/div/div[1]/div[1]/div/span";
			public static string dNKWH2 = "//*[@id='MeterReadings-']/div/div[2]/div[1]/div/span";
			public static string submitBtn = "//*[@id='MeterReadings-']/div/div[2]/button";
			public static string modalElecMeterHeader = "//*[@id='modalMeterReadingTooltip24h']/div/div/div[1]/h1";
			public static string modalElecP1 = "//*[@id='modalMeterReadingTooltip24h']/div/div/div[2]/div/p[1]";
			public static string modalElecP2 = "//*[@id='modalMeterReadingTooltip24h']/div/div/div[2]/div/p[2]";
			public static string modalfaqLink = "//*[@id='modalMeterReadingTooltip24h']/div/div/div[3]/a";
			public static string modalClose = "//*[@id='modalMeterReadingTooltip24h']/div/div/div[1]/button";
			public static string modalElecImage = "//*[@id='modalMeterReadingTooltip24h']/div/div/div[2]/div/p[3]/img";
			public static string elecError = "//*[@id='MeterReadings-']/div/div[1]/div[2]";

			public static string error = "//*[@id='MeterReadings-']/div/div[1]/div[2]";

			public static string modalGasDigitalHeader =
				"//*[@id='modalMeterReadingTooltipGas']/div/div[1]/div[1]/div[1]/h1";

			public static string modalGasImage1 = "//*[@id='modalMeterReadingTooltip24h']/div/div/div[2]/div/p[3]/img";

			public static string modalGasImage2 =
				"//*[@id='modalMeterReadingTooltipGas']/div/div[1]/div[2]/div[2]/div/p[2]/img";

			public static string modalGasP1 =
				"//*[@id='modalMeterReadingTooltipGas']/div/div[1]/div[1]/div[2]/div/p[1]";

			public static string modalGasP2 =
				"//*[@id='modalMeterReadingTooltipGas']/div/div[1]/div[1]/div[2]/div/p[2]";

			public static string modalGasReadingHeader =
				"//*[@id='modalMeterReadingTooltipGas']/div/div[1]/div[2]/div[1]/h1";

			public static string modalGasReadingP1 =
				"//*[@id='modalMeterReadingTooltipGas']/div/div[1]/div[2]/div[2]/div/p[1]";

			public static string dayMeterLabel = "//*[@id='MeterReadings-']/div/div[1]/label";
			public static string nightMeterLabel = "//*[@id='MeterReadings-']/div/div[2]/label";

			public static string modalDayNightMeterHeader =
				"//*[@id='modalMeterReadingTooltipDayNight']/div/div/div[1]/div[1]/div[1]/h1";

			public static string modalDayNightP1 =
				"//*[@id='modalMeterReadingTooltipDayNight']/div/div/div[1]/div[1]/div[2]/div/p[1]";

			public static string modalDayNightP2 =
				"//*[@id='modalMeterReadingTooltipDayNight']/div/div/div[1]/div[1]/div[2]/div/p[2]";

			public static string modalDayNightMeterImg =
				"//*[@id='modalMeterReadingTooltipDayNight']/div/div/div[1]/div[1]/div[2]/div/p[3]/img";

			public static string modalDayNightMeterFaq = "//*[@id='modalMeterReadingTooltipDayNight']/div/div/div[2]/a";

			public static string paginationRight = "//*[@id='meter-reading-history-pagination']/ul/li[6]/a/span";
			public static string paginationLeft = "//*[@id='meter-reading-history-pagination']/ul/li[6]/a/span";
			public static string paginationOne = "//*[@id='meter-reading-history-pagination']/ul/li[6]/a/span";
			public static string paginationTwo = "//*[@id='meter-reading-history-pagination']/ul/li[6]/a/span";
		}

		public static class RegisterPage
		{
			public static string goBackBtn = "//*[@id='modalMPRN']/div/div/div[3]/button";
		}
}
}