namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils
{
	public class TextMatch
	{
		public static class MeterReadingResultPage
		{
			public static string acceptReason = "Thanks for submitting your meter reading.";
			public static string rejectReason = "We've already received a reading for this meter today.";

			public static string rejectReason2 =
				"You cannot enter a meter reading at this time as your next bill is due to be issued soon.";

			public static string rejectReasonLessThan =
				"Sorry unfortunately we cannot accept your reading as it is less than the last ESB networks provided actual read. If you are sure your reading is correct please send a dated photo of your meter to meterreadingchanges@electricireland.ie. Please ensure your MPRN is in the subject line of email.";


			public static string rejectHeader = "Sorry, we can’t take your reading right now.";
			public static string backToMyAccounts = "Back to My Accounts";
		}

		public static class SubmitMeterReadingPage
		{
			public static string histMeterType = "Meter Type";
			public static string histFromDate = "From date";
			public static string histToDate = "To date";
			public static string histReading = "Reading";
			public static string histType = "Type";
			public static string histConsumption = "Consumption";
			public static string kWh = "kWh";
			public static string m3 = "m³";
			public static string submitHeader = "Submit Your Meter Reading";
			public static string elecMeterInputLabel = "24Hr Meter Reading";
			public static string gasMeterInputLabel = "Gas Meter Reading";
			public static string howDoIReadMyMeterBtnLabel = "How do I read my meter?";
			public static string submitBtn = "Submit Meter Reading";
			public static string modalElecMeterHeader = "Standard Meter";

			public static string modalElecP1 =
				"Make note of the number, reading from left to right. Please ignore in red or surrounded by a red box.";

			public static string modalElecP2 =
				"And remember, if you have night storage, you'll have two of these meters.";

			public static string modalfaqLink = "For other meters read our full FAQ";

			public static string modalGasDigitalHeader = "Digital Meter";

			public static string modalGasP1 =
				"Make note of the number, reading from left to right. Please ignore in red or surrounded by a red box.";

			public static string modalGasP2 =
				"And remember, if you have night storage, you'll have two of these meters.";

			public static string modalGasReadingHeader = "Reading a dial (clock) meter";

			public static string modalGasReadingP1 =
				"This meter displays a series of dials, as shown bellow. Read from left to right. If the pointer is halfway between two figures, use the lower figure. For example, if it is between 6 and 7, use 6. Or if the pointer is between 3 and 4, use 3. The reading on the image below is '1842'.";

			public static string emptyError = "Please enter a valid meter reading ";

			public static string modalDayNightMeterHeader = "Day & Night Meter";

			public static string modalDayNightP1 =
				"These meters may have numbers '1' and '2' or Roman numerals 'I' and 'II' referring to night and Day resepectively.";

			public static string modalDayNightP2 =
				"Read from left to right ignoring the figure in red surrounded by a red box.";

			public static string dayMeterLabel = "Day Meter Reading";
			public static string nightMeterLabel = "Night Meter Reading";
		}

		public static class PaymentsPage
		{
			public static string AmountDueIsTooLow = "Amount due is too low to make a payment. Please change amount.";
			public static string payNowErrorAlphabetical = "You must enter amount for payment  ";
			public static string payNowErrorMinus = "Please enter valid amount for payment";
			public static string payNowErrorHigh = "Payment amount must be within 0.02 to 9999.99";
		}

		public static class DirectDebitSettingsPage
		{
			public static string ibanError = "Please enter a valid IBAN";

			public static string termsAndConditionsError =
				"Please confirm that you have read and accept the Electric Ireland Terms and Conditions";

			public static string nameError = "Please enter a Bank Account name";
		}

		public static class ContactUsPage
		{
			internal static string errorQueryLonger1900 = "Please enter no more than 1900 characters";
			internal static string errorQueryEmpty = "Please enter your message here.";
			internal static string errorSubjectEmpty = "Please give a short description of your query. ";
			internal static string errorMPRNorGPRNEmpty = "Please enter a valid MPRN or GPRN";
			internal static string errorMPRNorGPRNShort = "Please enter a valid MPRN or GPRN ";
			internal static string errorMPRNorGPRNAlpha = "Please enter a valid MPRN or GPRN ";
			internal static string errorAccountNumberShort = "Please enter your Electric Ireland account number ";
			internal static string errorAccountNumberEmpty = "Please enter your Electric Ireland account number";
			internal static string errorAccountNumberAlpha = "Please enter your Electric Ireland account number ";
		}

		public static class GasAccountSetupPage
		{
			internal static string ErrorMeterReadingEmpty = "Please enter a valid meter reading";
			internal static string ErrorGPRNEmpty = "You must enter a valid GPRN with 7 digits";
			internal static string ErrorGPRNShort = "You must enter a valid GPRN with 7 digits";
			internal static string ErrorGPRNLong = "You must enter a valid GPRN with 7 digits";
			internal static string ErrorMeterReadingAlpha = "Please enter a valid meter reading";
			internal static string ErrorGPRNAlpha = "You must enter a valid GPRN with 7 digits";
			internal static string ErrorPricePlan = "You must check this box to proceed";
			internal static string ErrorDebt = "You must check this box to proceed";
			internal static string ErrorTerms = "You must accept the terms and conditions";
			internal static string ErrorDetails = "You must check this box to proceed";
			internal static string GasAccountSetupHeader = "Gas Account Set Up";
		}

		public static class RefundRequestPage
		{
			internal static string ErrorTooLong = "Please enter no more than 1900 characters";
		}
	}
}