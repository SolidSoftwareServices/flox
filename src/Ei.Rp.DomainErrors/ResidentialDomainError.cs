using EI.RP.CoreServices.ErrorHandling;

namespace Ei.Rp.DomainErrors
{
    
    public class ResidentialDomainError:DomainError
    {

        private ResidentialDomainError(int errorCode, string defaultErrorMessage = "") : base(errorCode, defaultErrorMessage)
        {
        }
        #region Create Online Account

        public static readonly DomainError UserAlreadyExists=new ResidentialDomainError(100,"User already exists.");
        public static readonly DomainError Invalid_MPRN=new ResidentialDomainError(101, "Please enter a valid MPRN.");
        public static readonly DomainError Invalid_AccountNumber=new ResidentialDomainError(102,"Invalid Account Number.");
        public static readonly DomainError AccountAlreadyExists=new ResidentialDomainError(103, "The account already exists, contact us for details.");
        public static readonly DomainError BusinessAccount =new ResidentialDomainError(104, "The details you entered are for a business account. Please log in to Business Online to view your account.");
        public static readonly DomainError ActionCancelled = new ResidentialDomainError(105, "Action cancelled, User Request is already completed.");
        public static readonly DomainError NoDataFound = new ResidentialDomainError(106, "No data found for the Request ID");
        public static readonly DomainError CreateAccountTermsAndConditionsNotSelected = new ResidentialDomainError(107, "Terms and conditions not selected");
        public static readonly DomainError MeterReadingNextBillDue = new ResidentialDomainError(108, "You cannot enter a meter reading at this time as your next bill is due to be issued soon.");
        public static readonly DomainError MeterReadingAlreadyReceived = new ResidentialDomainError(109, " We've already received a reading for this meter today.");
        public static readonly DomainError MeterReadingReadyToIssue = new ResidentialDomainError(110, "We’re getting ready to issue your next bill so we can’t take your reading.");
        public static readonly DomainError UnableToProcessRequest = new ResidentialDomainError(111, "Unable to process your request.");
        public static readonly DomainError DataAlreadyReleased = new ResidentialDomainError(112, "It is not allowed to access data that have already been released.");
        public static readonly DomainError InvalidBusinessAgreement = new ResidentialDomainError(113, "Business Partner invalid for account.");
        public static readonly DomainError InvalidAccountId = new ResidentialDomainError(114, "Invalid Account ID.");
        public static readonly DomainError ContractAlreadyRegistered = new ResidentialDomainError(115, "Contract Account has already been registered.");
        public static readonly DomainError SorryUnableToProcessRequest = new ResidentialDomainError(116, "Sorry, Unable to process your request.");
        #endregion

        #region Reset Password
        public static readonly DomainError ResetPassword_InvalidEmail = new ResidentialDomainError(107, "Sorry, we don't recognise that email address. Please try another.");
        public static readonly DomainError ResetPassword_InvalidLink = new ResidentialDomainError(108, "Invalid reset password link.");
		#endregion

		#region Move In / Move Out
		#endregion

		public static readonly DomainError AuthenticationError = new ResidentialDomainError(200, "User authentication failed.");
        public static readonly DomainError AuthorizationError = new ResidentialDomainError(205, "User authorization failed.");
        //TODO: adD ALL THE DOMAIN ERRORS ON DEMAND HERE, INCREMENT IN HUNDREDS OR TENTHS

        public static readonly DomainError InvalidActivationLink = new ResidentialDomainError(210, "Invalid Activation Link");
        public static readonly DomainError MandatoryFieldMissing = new ResidentialDomainError(220, "Mandatory field is missing in SAP Request.");
        public static readonly DomainError NewPasswordMustBeDifferentThanTheLast5Passwords = new ResidentialDomainError(230, "Choose a password that is different from your last 5 passwords.");
        public static readonly DomainError IncorrectPasswordChangeError = new ResidentialDomainError(230, "This is not the correct password, please try again.");
        public static readonly DomainError PasswordChangeOnceError = new ResidentialDomainError(230, "You can change your password only once a day.");


        public static readonly DomainError ResourceNotFound = new ResidentialDomainError(404, "Resource not found");

        public static readonly DomainError IBANInvalidError = new ResidentialDomainError(440, "Please enter a valid IBAN.");
        public static readonly DomainError IbanInvalidBankForCountry = new ResidentialDomainError(445, "Please enter a valid IBAN.");
        

        public static readonly DomainError AccAlreadyRegisteredError = new ResidentialDomainError(450, "The account is already registered with the customer.");
        public static readonly DomainError FeeNotifier  = new ResidentialDomainError(460, "As your accounts are still in contract on your move out date you will incur an early exit fee of €50 for each account. This will be added to your final bill for each account.");
        public static readonly DomainError GasNoFutureDated  = new ResidentialDomainError(470, "Gas Move out cannot be future dated.");
        public static readonly DomainError SupplyGauranteeRemovedAlert  = new ResidentialDomainError(480, "The Guarantee of Supply will be removed");
        public static readonly DomainError OpenInstallment  = new ResidentialDomainError(490, "Open instalment plan exists on contract account");
        public static readonly DomainError ContractCollectiveBill  = new ResidentialDomainError(500, "Contract Account is Collective Bill Account");
        public static readonly DomainError SiteBeingDisconnected  = new ResidentialDomainError(510, "This site is currently being disconnected");
        public static readonly DomainError ContractEndTooEarly  = new ResidentialDomainError(520, "Enter a contract end date later than (Display Date here)");
        public static readonly DomainError TokenMeterExists  = new ResidentialDomainError(530, "A token meter exists. Please contact ESB support to continue.");
        public static readonly DomainError PoDLocked  = new ResidentialDomainError(540, "PoD (POD ID DISPLAYED HERE) is currently being locked");
        public static readonly DomainError AnnualBillsAlert  = new ResidentialDomainError(550, "Annual bill(s) detected. Please raise query");
        public static readonly DomainError AdjustmentReversalBillsAlert  = new ResidentialDomainError(560, "Adjustment Reversal Bill(s) detected.Please raise query to reverse");
        public static readonly DomainError ContractCollectiveBillQueryAlert  = new ResidentialDomainError(570, "Contract Account is Collective Bill Account. Raise query for bills.");
        public static readonly DomainError ExistingBillOutsideContract  = new ResidentialDomainError(580, "There are existing bill(s) outside of current contract.");
        public static readonly DomainError GasBillReversalNotPossible  = new ResidentialDomainError(590, "Gas bill reversal not possible – please raise query.");
        public static readonly DomainError MoreThanFourBillsReversed  = new ResidentialDomainError(600, "More than 4 bills need to be reversed – please raise query");
        public static readonly DomainError RevenueProtection  = new ResidentialDomainError(610, "Revenue protection query exists against the BP contract account");
        public static readonly DomainError TokenMeterExistsContactESB  = new ResidentialDomainError(620, "A token meter exists.Please contact ESB support to continue.");
        public static readonly DomainError CantProcessMoveInMoveOut  = new ResidentialDomainError(630, "We cannot process your move request at the moment. Please call 1850 372 372 to complete your move");
        public static readonly DomainError ThereHasBeenAnErrorPlsTryAgain = new ResidentialDomainError(640, "Sorry there has been an error. Please try again later.");
        public static readonly DomainError InstallmentPlanWillBeCancelled  = new ResidentialDomainError(650, "Please note: Your installment plan will be cancelled and any amount due on your final bill will need to be paid in full.");
        public static readonly DomainError PayAsYouGoNotSupported  = new ResidentialDomainError(660, "Pay- As - You - Go is currently not supported for online services.");
        public static readonly DomainError MoveOutThirtyDays  = new ResidentialDomainError(670, "Move-out for gas cannot be more than 30 days backdated.");
        public static readonly DomainError SiteSwitch  = new ResidentialDomainError(680, "This site is currently undergoing a switch.");
        public static readonly DomainError InstallationDiscStatusIsNotNew = new ResidentialDomainError(580, "Installation disc status is not new");

		public static readonly DomainError AccountPartOfCollectiveBill = new ResidentialDomainError(690, "Contract Account (ACCOUNT DISPLAYED) is part of Collective Bill Account.");

        public static readonly DomainError Invalid_AccountIsInCollectiveBill = new ResidentialDomainError(700, "Contract Account #acc# is Collective Bill Account");
        public static readonly DomainError ForAgentsDuringDeregistration = new ResidentialDomainError(710, "For agents during deregistration.");
        public static readonly DomainError AddressUpdatesNotPermitted = new ResidentialDomainError(720, "We are not permitting address updates so this should not occur.");
        public static readonly DomainError IncorrectPasswordError = new ResidentialDomainError(730, "This is not the correct password, please try again.");
        public static readonly DomainError BillDateReversedAlert = new ResidentialDomainError(740, "Bill(s) from #date# will be reversed.");
        public static readonly DomainError TermsAndCondError = new ResidentialDomainError(750, "Please accept the terms and conditions.");
        public static readonly DomainError AccountOwnerError = new ResidentialDomainError(760, "Please confirm you are the account owner.");
        public static readonly DomainError AccountCannotBeClosed = new ResidentialDomainError(765, "The account cannot be closed.");
		public static readonly DomainError ChangePasswordOncePerDayError = new ResidentialDomainError(780, "You can change your password only once a day.");
        public static readonly DomainError CouldNotChangePasswordError = new ResidentialDomainError(785, "Password unspecified error.");
		public static readonly DomainError EnterAPasswordError = new ResidentialDomainError(790, "Please enter a password.");
        public static readonly DomainError ContactDetailsUpToDate = new ResidentialDomainError(800, "Your contact details are up to date. Please note you can only change your marketing preferences once a day.");
        public static readonly DomainError DateOfBirthError = new ResidentialDomainError(810, "Please enter a valid date of birth.");
        public static readonly DomainError UnregisteredEmailAddressError = new ResidentialDomainError(820, "This is not a registered email address.Please remember that the email address you used to set up your online account remains your login email, even if you have changed your email address in the 'Edit My Profile' section of your account online.");
        public static readonly DomainError SendEmailFailed= new ResidentialDomainError(821, "Send email failed");
        public static readonly DomainError ShouldNotBeSeenByEndUser = new ResidentialDomainError(830, "Is this something seen by the end user? -No.");
        public static readonly DomainError ErrorWillNotOccur = new ResidentialDomainError(840, "We are not creating work items so this error will not occur.");
        public static readonly DomainError InvalidActivationLinkError = new ResidentialDomainError(850, "Activation link is invalid.");
        public static readonly DomainError TheInvoiceDoesNotHaveAFile = new ResidentialDomainError(860, "The invoice does not have a valid file");
        public static readonly DomainError ErrorGeneratingInvoiceFile = new ResidentialDomainError(870, "Error Generating invoice file.");
        public static readonly DomainError ThereHasBeenAnError = new ResidentialDomainError(880, "Sorry there has been an error.Please try again in a few moments.");
        public static readonly DomainError RequestNotProcessedError = new ResidentialDomainError(890, "Unfortunately your request could not be processed at this time. Apologies for any inconvenience this causes.Please try again later.");
        public static readonly DomainError ContactNumberInvalid = new ResidentialDomainError(892, "Contact Number provided is invalid.");
        public static readonly DomainError InvalidActiveContract = new ResidentialDomainError(891, "Invalid active contract...GPRN is not valid format");
		public static readonly DomainError MeterSubmitOutOfTolerance = new ResidentialDomainError(892, "Reading deemed implausible. Consumption is outside tolerance limits.");
		public static readonly DomainError MeterReadingLessThanActualNetwork = new ResidentialDomainError(893, "Cannot submit meter reading as it is less than the last ESB networks provided actual read.");
		public static readonly DomainError MeterReadingLessThanActualCustomer = new ResidentialDomainError(894, "Cannot submit meter reading as it is less than the last ESB customer provided actual read.");
		public static readonly DomainError CouldNotFindPointOfDelivery = new ResidentialDomainError(1000, "Could not find the point of delivery.");
        public static readonly DomainError ContractErrorPreventTheContactFromBeingSubmitted = new ResidentialDomainError(1010, "Contract error(s) prevent the contract from being submitted.");
		public static readonly DomainError ErrorCompletingRequestContactSomeone = new ResidentialDomainError(1020, "Cannot complete request. Contact callcenter.");
		public static readonly DomainError ErrorDataNotReplicated = new ResidentialDomainError(1030, "Data source replication failed or is slow.Try back again later.");

		public static readonly DomainError AccountsInBundleConfiguredOnDifferentAddresses =
	        new ResidentialDomainError(900, "Account bundles found with different addresses");


    }
}
