using OpenQA.Selenium;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils
{
	public static class IdentifierSelector
	{
		public static class NavMenu
		{
			public static string
				meterReadingTab = "nav-meter-reading-link",
				planTab = "nav-plan-link",
				detailsTab = "nav-details-link",
				usageTab = "nav-usage-link",
				billsAndPaymentsTab = "billsAndPaymentsTab",
				moveHouseTab = "nav-moving-house-link",
				makeAPaymentTab = "paymentTab",
				paymentsTab = "nav-payments-link",
				contactUsMenuItem = "main-navigation-contact-us-link",
				help = "main-navigation-help-link",
				productsAndServices = "main-navigation-products-and-services-link",
				myAccounts = "main-navigation-my-accounts-link";

		}

		public static class AgentSearchPage
		{
			public static string Street = "Street";
			public static string HouseNo = "House";
			public static string City = "City";
			public static string MaxNo = "MaxNo";
			public static string Username = "FirstName";
			public static string BP = "Partner";
			public static string errorEmpty = "errorMessage";
		}

		public static class MeterReadingPage
		{
			public static string TitleTestId = "meter-reading-title";
			public static string TitleSuccessTestId = "meter-reading-success-title";
			public static string TitleErrorTestId = "meter-reading-error-title";
			public static string MessageErrorTestId = "meter-reading-error-message";
			public static string InputValueATestId = "reading-value-input-1";
			public static string InputValueBTestId = "reading-value-input-2";
			public static string MessageHowDoIRead = "meter-reading-how-do-i-read-message";
			public static string LinkHowDoIRead = "meter-reading-how-do-i-read-message-link";
			public static string ModalHowDoIReadGas = "meter-reading-how-do-i-read-gas-modal";
			public static string ModalHowDoIReadDayNight = "meter-reading-how-do-i-read-day-night-modal";
			public static string ModalHowDoIRead24H = "meter-reading-how-do-i-read-24h-modal";
			public static string ButtonSubmitMeterReadingTestId = "submit-meter-reading-button";
			public static string TitleHistoryTestId = "meter-reading-history-title";
			public static string TableHistoryTestId = "meter-reading-history-table";
			public static string TableHistoryMobileTestId = "meter-reading-history-table-mobile";
			public static string ButtonBackTestId = "meter-reading-back";

			public static string meterReadingInputID = "MeterReadings_0__ReadingValue";
			public static string blueTickID = "confirmIcon";
			public static string redXID = "errorIcon";
			public static string sorryWeCantTakeYourMeterReadingID = "sorryWeCantTakeYourMeterReadingRightNow";
			public static string acceptedHeaderID = "confirmMessage";
			public static string meterReadingHeaderID = "meterReadingHeading";
			public static string meterReadingHistoryHeaderID = "meterReadingHistoryHeader";
			public static string howDoIReadMyMeterID = "btnMeterreadingHowToRead";
			public static string meterReadingInputDayID = "MeterReadings_0__ReadingValue";
			public static string meterReadingInputNightID = "MeterReadings_1__ReadingValue";
		}

		public static class PlanPage
		{
			public static string payNow = "payments-history-pay-now-button";
			public static string on = "toggle-on";
			public static string off = "toggle-off";
			public static string editDirectDebit = "edit-direct-debit-link";
			public static string equaliserSetup = "equaliser-link";
		}

		public static class EditDirectDebitPage
		{
			public static string gdpr = "privacy-notice-message-component-link";
			public static string IBANInputFieldID = "iban";
			public static string bankNameInputFieldID = "customer-name";
			public static string termsAndConditionsID = "terms";
			public static string updateDetailsBtnID = "dds_update_details_btn";
			public static string cancelID = "dds_cancel_btn";
			public static string mandateID = "dds_mandate";
			public static string downloadMandateID = "dds_mandate_download";
		}

		public static class PaymentsPage
		{
			public static string moreAboutEqual = "payments-history-equalizer-monthly-payments-link";
			public static string changeBllingPreferences = "payments-history-change-billing-preferences-link";
		}

		public static class AgentLoginPage
		{
			public static string usernameTxt = "txtUserName";
		}

		public static class GasSetupConfirmAddressPage
		{
			public static string helpID = "help-number";
			public static string noID = "btnNotMyAddress";
			public static string yesID = "btnConfirmAddress";
			public static string addressID = "address-details";
			public static string gprnID = "gprn";
			public static string confirmAddressTextID = "confirmAddressText";
			public static string confirmAddressHeaderID = "confirmAddressTitle";
		}

		public static class GasAccountPaymentPage
		{
			public static string gasAccPaymentHeaderID = "gasAccountPayment";
			public static string useExistingDDHeaderID = "useExistingDirectDebitHeader";
			public static string setUpNewDDHeaderID = "setUpNewDDHeader";
			public static string useExistingDDSubTextID = "useExistingDirectDebitHeader";
			public static string setUpNewDDSubTextID = "setUpNewDDSubText";
			public static string useExistingDDCheckBoxTextID = "continueDebit";
			public static string useExistingDirectDebitBtnID = "useExistingDirectDebit";
			public static string setUpNewDirectDebitBtnID = "setUpNewDirectDebit";
			public static string goBackBtnID = "btnGoBack";
			public static string skipCompleteSetUpBtnID = "btnSkipAndComplete";
		}

		public static class ContactUsPage
		{
			public static string accountFieldID = "accountList";
			public static string typeOfQueryFieldID = "queryType";
			public static string accountNumberFieldID = "accountNumber";
			public static string MPRNFieldID = "meterNumber";
			public static string subjectFieldID = "subject";
			public static string queryFieldID = "query";
			public static string submitBtnID = "btnSubmitQuery";
			public static string confirmation = "contact-us-confirmation";
		}

		public static class GasAccountSetupPage
		{
			public static string submitBtnID = "gasAccount";
			public static string priceTAndCsCheckboxID = "chkPricePlanTermsAndCondition";
			public static string GdprTextID = "gdprText";
			public static string debtFlagCheckboxID = "chkDebtFlagAndArrearsTermsAndConditions";
			public static string termsCheckboxID = "chkTermsAndConditionsAccepted";
			public static string detailsCheckboxID = "chkAuthorization";
			public static string gasMeterReadingTextFieldID = "GasMeterReading";
			public static string gprnTextFieldID = "inputGPRN";
			public static string cancelBtnID = "btnCancel";
		}

		public static class RegisterPage
		{
			public static string accountNumberInfoModalBtn = "accountNumberInfoModalBtn";
			public static string firstNameID = "txtFirstName";
			public static string lastNameID = "txtLastName";
			public static string emailID = "txtEmail";
			public static string accountNumberID = "txtAccountNumber";
			public static string phoneNumberID = "txtPhoneNumber";
			public static string mprnID = "txtMPRN";
			public static string dateOfBirthDayID = "txtDateofBirthDay";
			public static string dateOfBirthMonthID = "txtDateofBirthMonth";
			public static string dateOfBirthYearID = "txtDateofBirthYear";
			public static string termsAndConditionsID = "chkAcceptTC";
			public static string ownerID = "lblOwner";
			public static string privacyNoticeLinkID = "";
			public static string accountNumberInformationModalHeaderID = "MPRNlabel";
			public static string accountNumberInformationBtnID = "accountNumberInfoModalBtn";
			public static string mprnInformationBtnID = "mprnInfoBtn";
			public static string registerBtnID = "CreateAccount";
			public static string errorEmailID = "emailErrMsg";
			public static string errorAccountNumberID = "accountNumberErrMsg";
			public static string errorMPRNID = "mprnErrMsg";
			public static string errorPhoneNumberID = "phoneErrMsg";
			public static string errorPhoneNumberMinID = "phoneMinErrMsg";
			public static string errorFirstNameID = "firstNameErrMsg";
			public static string errorLastNameID = "lastNameErrMsg";
			public static string errorDOBID = "dobErrMsg";
			public static string errorOwnerID = "chkErrMsgOwner";
			public static string errorTandCID = "chkErrMsgAcceptTC";
			public static string modalMPRNCloseBtnID = "modalMPRNClose";
			public static string loginBtnID = "SignUp";
			public static string modalMPRNHeaderID = "";
			public static string logoID = "logoImg";
		}

		public static class UsagePage
		{
			public static string compareNow = "compareNow";
			public static string KWH = "value-kwh";
			public static string compareYears = "compareSwitch";
		}

		public static class RefundRequestPage
		{
			public static string AccountInputLabelId = "lblAccount";
			public static string AccountInputFieldId = "txtAccount";
			public static string AccountCreditLabelId = "lblAccountCredit";
			public static string AccountCreditFieldId = "txtAccountCredit";
			public static string AddInfoInputLabelId = "lblComments";
			public static string AddInfoInputFieldId = "txtComments";
			public static string RequestRefundBtnId = "btnRequestRefund";
			public static string confirmationTitle = "request-refund-confirmation-title";
			public static string requestRefundBtnID = "btnRequestRefund";
			public static string accountInputFieldID = "txtDisplayAccountName";
			public static string accountCreditLabelID = "lblAccountCredit";
			public static string accountCreditValID = "";
			public static string addInfoInputFieldID = "txtComments";
			public static string gdprTextID = "privacy-notice-message-component-link";
			public static string requestRefundHeaderID = "requestRefundHeader";
			public static string elecBillFaqID = "electricityBillFAQ";
			public static string changeMyNameFaqID = "changeMyNameFAQ";
			public static string meterReadingFaqID = "meterReadingFAQ";
			public static string directDebitFaqID = "directDebitFAQ";
			public static string closeAccFaqID = "closeAccountFAQ";
			public static string thankYouID = "r_confirm_msg";
			public static string blueTickID = "r_confirmation_tick";
			public static string backToAccOverviewBtnID = "request-refund-confirmation-back-to-my-accounts";
		}
	}
}