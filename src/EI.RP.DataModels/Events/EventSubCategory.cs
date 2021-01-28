namespace EI.RP.DataModels.Events
{
    public static class EventSubCategory
    {
        


        public const long LoginRequest = 1101;

        public const long RegistrationRequest = 1102;


        public const long CreatePasswordRequest = 1104;
        public const long ChangePasswordId = 1104;
        public const long ResetPassword = 1105;
        public const long ForgetPasswordCompleteId = 1106;

        public const long ViewBill = 1201;

        public const long EditUserContact = 1301;
        public const long MarketPreferences = 1302;
		public const long SignUpDirectDebit = 1303;
        public const long EditDirectDebit = 1304;
        public const long EqualiserSignUpId = 1305;
        public const long EqualiserEditDetails = 1306;
		public const long PaperBillOn=1307;
		public const long PaperBillOff = 1308;
		public const long EnterMeterReadingEventId = 1309;

		public const long PaymentResult = 1401;
		public const long CalculateEstimationForNextBill = 1402;

        public const long CloseElectricity = 1501;
        public const long CloseGas = 1502;
        public const long CloseElectricityAndGas = -1503;
	    public const long MovingHouseCloseElec = 1503;
	    public const long MovingHouseCloseGas = 1504;

		public const long AddAdditionalAccountQuery = 1601;
        public const long MeterReadQuery = 1602;
        public const long BillOrPaymentQuery = 1603;
        public const long OtherQuery = 1605;
		

        public const long RefundRequest = 1604;
        

        public const long BusinessPartnerDeregistration = 1801;

        public const long DismissSmartActivationNotification = 1310;
        public const long ShowSmartActivationNotificationToUser = 1204;
        public const long SmartActivationThroughNotification = 1531;
		public const long SmartMeterActivation = 1532;
		public const long MonthlyBillingPeriod = 1311; 
		public const long BiMonthlyBillingPeriod = 1312;
		public const long ChangeSmartPlanToStandard = 1313;
		public const long AddGasAccount = 1521;
		public const long AddGasAccountNewDirectDebit = 1522;
		public const long UseElectricityDirectDebit = 1523;
		public const long AddAdditionalAccount = 1541;

		public const long UpdatePremiseNotes = 1511;
		public const long AddAnAdditionalAccountMail = 1611;
		public const long BillOrPaymentQueryMail = 1612;
		public const long MeterReadQueryMail = 1613;
		public const long OtherMail = 1614;

		public const long RecordMovingHomePrns = 1513; 
		public const long RecordMovingInProgress = 1512; 
		public const long RecordMovingOutProgress = 1514;
		public const long UpdateEBiller = 1314;

		public const long CompetitionEntry = 1621;

	}
}