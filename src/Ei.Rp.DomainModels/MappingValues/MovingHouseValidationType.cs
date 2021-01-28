using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
	public class MovingHouseValidationType : TypedStringValue<MovingHouseValidationType>
	{
		[JsonConstructor]
		private MovingHouseValidationType()
		{
		}

		public MovingHouseValidationType(string value) : base(value)
		{

		}public static readonly MovingHouseValidationType QueryValidation = new MovingHouseValidationType(nameof(QueryValidation));
		
		public static readonly MovingHouseValidationType IsNotPAYGCustomer = new MovingHouseValidationType(nameof(IsNotPAYGCustomer));
		public static readonly MovingHouseValidationType IsNotLossInProgress = new MovingHouseValidationType(nameof(IsNotLossInProgress));
		public static readonly MovingHouseValidationType IsSapCheckMoveOutOk = new MovingHouseValidationType(nameof(IsSapCheckMoveOutOk));
		public static readonly MovingHouseValidationType HasSameAddress = new MovingHouseValidationType(nameof(HasSameAddress));
		public static readonly MovingHouseValidationType AccountContractStartedBeforeToday = new MovingHouseValidationType(nameof(AccountContractStartedBeforeToday));
		public static readonly MovingHouseValidationType ContractEndDateIsValid = new MovingHouseValidationType(nameof(ContractEndDateIsValid));
		public static readonly MovingHouseValidationType BusinessAgreementDoesNotHaveCollectiveParent = new MovingHouseValidationType(nameof(BusinessAgreementDoesNotHaveCollectiveParent));
		public static readonly MovingHouseValidationType DiscStatusIsNew = new MovingHouseValidationType(nameof(DiscStatusIsNew));
		public static readonly MovingHouseValidationType IsNotEqualizer = new MovingHouseValidationType(nameof(IsNotEqualizer));
		public static readonly MovingHouseValidationType IsResidentialCustomer = new MovingHouseValidationType(nameof(IsResidentialCustomer));
		public static readonly MovingHouseValidationType HasPremiseInstallations = new MovingHouseValidationType(nameof(HasPremiseInstallations));
		public static readonly MovingHouseValidationType NewPremisePointReferenceNumbersAreValid = new MovingHouseValidationType(nameof(NewPremisePointReferenceNumbersAreValid));		
		public static readonly MovingHouseValidationType MoveInDateMoreThan2daysInTheFuture = new MovingHouseValidationType(nameof(MoveInDateMoreThan2daysInTheFuture));
        public static readonly MovingHouseValidationType PhoneNumberIsValid = new MovingHouseValidationType(nameof(PhoneNumberIsValid));
        public static readonly MovingHouseValidationType HasAccountDevices = new MovingHouseValidationType(nameof(HasAccountDevices));      
        public static readonly MovingHouseValidationType HasElectricNewPremiseAccountDetailsInSwitch = new MovingHouseValidationType(nameof(HasElectricNewPremiseAccountDetailsInSwitch));
        public static readonly MovingHouseValidationType CanCloseAccounts = new MovingHouseValidationType(nameof(CanCloseAccounts));
        public static readonly MovingHouseValidationType IsContractSaleChecksOk = new MovingHouseValidationType(nameof(IsContractSaleChecksOk));
        public static readonly MovingHouseValidationType PodNotRegisteredEarlierToday = new MovingHouseValidationType(nameof(PodNotRegisteredEarlierToday));
		public static readonly MovingHouseValidationType IsNonSmartMoveOutDeviceSet = new MovingHouseValidationType(nameof(IsNonSmartMoveOutDeviceSet));
		public static readonly MovingHouseValidationType IsNonSmartMoveInDeviceSet = new MovingHouseValidationType(nameof(IsNonSmartMoveInDeviceSet));

	}
}