using System;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.CoreServices.Cqrs.Queries;

namespace EI.RP.DomainServices.Queries.MovingHouse.Validation
{
	public class MovingHouseValidationQuery : IQueryModel, IEquatable<MovingHouseValidationQuery>
	{
		public QueryCacheResultsType CacheResults { get; } = QueryCacheResultsType.NoCache;

		public string ElectricityAccountNumber { get; set; }
		public string GasAccountNumber { get; set; }
        public bool ValidateHasAccountDevices { get; set; }
        public bool ValidateNewPremise { get; set; }
        public bool ValidateNewPremisePodNotRegisteredToday { get; set; }
        public bool ValidateNewGprnPremiseAreNotDeregistered { get; set; }
		public bool ValidatePhoneNumber { get; set; }
        public string NewMPRN { get; set; }
        public string NewGPRN { get; set; }
        public string PhoneNumber { get; set; }
        public MovingHouseType MovingHouseType { get; set; }
        public bool ValidateMoveInDetails { get; set; }
        public DateTime? MoveInDate { get; set; }
        public bool ValidateElectricNewPremiseAddressInSwitch { get; set; }
        public bool IsNewAcquisitionElectricity { get; set; }
        public bool IsMPRNDeregistered { get; set; }
        public bool IsGPRNDeregistered { get; set; }
		public DateTime? MoveOutDate { get; set; }
        public bool ValidateCanCloseAccounts { get; set; }
        public bool IsMPRNAddressInSwitch { get; set; }
        public bool ValidateContractSaleChecks { get; set; }
        public string InitiatedFromAccountNumber { get; set; }

        public bool Equals(MovingHouseValidationQuery other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
            return Equals(ElectricityAccountNumber, other.ElectricityAccountNumber) &&
                   Equals(GasAccountNumber, other.GasAccountNumber) &&
                   Equals(ValidateHasAccountDevices, other.ValidateHasAccountDevices) &&
                   Equals(ValidateNewPremise, other.ValidateNewPremise) &&
                   Equals(ValidateNewPremisePodNotRegisteredToday, other.ValidateNewPremisePodNotRegisteredToday) &&
                   Equals(ValidateNewGprnPremiseAreNotDeregistered, other.ValidateNewGprnPremiseAreNotDeregistered) &&
				   Equals(ValidatePhoneNumber, other.ValidatePhoneNumber) &&
                   Equals(PhoneNumber, other.PhoneNumber) &&
                   Equals(NewMPRN, other.NewMPRN) &&
                   Equals(NewGPRN, other.NewGPRN) &&
                   Equals(MovingHouseType, other.MovingHouseType) &&
                   Equals(ValidateMoveInDetails, other.ValidateMoveInDetails) &&
                   Equals(MoveInDate, other.MoveInDate) &&
                   Equals(ValidateElectricNewPremiseAddressInSwitch, other.ValidateElectricNewPremiseAddressInSwitch) &&
                   Equals(IsNewAcquisitionElectricity, other.IsNewAcquisitionElectricity) &&
                   Equals(IsMPRNDeregistered, other.IsMPRNDeregistered) &&
                   Equals(IsGPRNDeregistered, other.IsGPRNDeregistered) &&
				   Equals(MoveOutDate, other.MoveOutDate) &&
                   Equals(ValidateCanCloseAccounts, other.ValidateCanCloseAccounts) &&
                   Equals(IsMPRNAddressInSwitch, other.IsMPRNAddressInSwitch) &&
                   Equals(ValidateContractSaleChecks, other.ValidateContractSaleChecks) &&
                   Equals(InitiatedFromAccountNumber, other.InitiatedFromAccountNumber);
		}

		public bool IsValidQuery(out string[] notValidReasons)
		{
			if (string.IsNullOrEmpty(ElectricityAccountNumber) && string.IsNullOrEmpty(GasAccountNumber))
			{
				notValidReasons = new[]
					{"Electricity and Gas Account Numbers are both null. Please provide at least one."};
				return false;
			}

			notValidReasons = null;
			return true;
		}

		public override string ToString()
		{
			return
				$"{nameof(ElectricityAccountNumber)}: {ElectricityAccountNumber}, {nameof(GasAccountNumber)}: {GasAccountNumber} " +
                $"{nameof(ValidateHasAccountDevices)}: {ValidateHasAccountDevices} {nameof(ValidateNewPremise)}: {ValidateNewPremise} {nameof(NewMPRN)}: {NewMPRN} {nameof(NewGPRN)}: {NewGPRN} {nameof(MovingHouseType)}: {MovingHouseType}" +
                $"{nameof(ValidateNewPremisePodNotRegisteredToday)}: {ValidateNewPremisePodNotRegisteredToday}, " +
				$"{nameof(ValidateNewGprnPremiseAreNotDeregistered)}: {ValidateNewGprnPremiseAreNotDeregistered}, " +
				$"{nameof(ValidateMoveInDetails)}: {ValidateMoveInDetails} {nameof(MoveInDate)}: {MoveInDate}, " +
                $"{nameof(ValidatePhoneNumber)}: {ValidatePhoneNumber}, {nameof(PhoneNumber)}: {PhoneNumber}, " +
                $"{nameof(ValidateElectricNewPremiseAddressInSwitch)}: {ValidateElectricNewPremiseAddressInSwitch}, " +
                $"{nameof(IsNewAcquisitionElectricity)}: {IsNewAcquisitionElectricity}, " +
                $"{nameof(IsMPRNDeregistered)}: {IsMPRNDeregistered}, " +
				$"{nameof(IsGPRNDeregistered)}: {IsGPRNDeregistered}, " +
				$"{nameof(MoveOutDate)}: {MoveOutDate}, " +
                $"{nameof(ValidateCanCloseAccounts)}: {ValidateCanCloseAccounts}, " +
                $"{nameof(IsMPRNAddressInSwitch)}: {IsMPRNAddressInSwitch}, " +
                $"{nameof(ValidateContractSaleChecks)}: {ValidateContractSaleChecks}, " +
                $"{nameof(InitiatedFromAccountNumber)}: {InitiatedFromAccountNumber}";
		}
	}
}