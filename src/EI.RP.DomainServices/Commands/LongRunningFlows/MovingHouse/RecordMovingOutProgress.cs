using System;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.Contracts.Accounts;

namespace EI.RP.DomainServices.Commands.LongRunningFlows.MovingHouse
{
	public class RecordMovingOutProgress : RecordMovingHomeProgress, IDomainCommand, IEquatable<RecordMovingOutProgress>
	{
		public RecordMovingOutProgress(
			MovingHouseType moveType,
			AccountInfo initiatedFromAccount,
			AccountInfo otherAccount,
			int electricityMeterReading24HrsOrDayValue,
			int electricityMeterReadingNightOrNshValue,
			int gasMeterReadingValue,
			bool informationCollectionAuthorized, 
			bool termsAndConditionsAccepted,
			bool incomingOccupant,
			string lettingAgentName, 
			string lettingPhoneNumber,
			bool occupierDetailsAccepted, 
			DateTime movingOutDate) : base(moveType, initiatedFromAccount, otherAccount)
		{
		
			ElectricityMeterReading24HrsOrDayValue = electricityMeterReading24HrsOrDayValue;
			ElectricityMeterReadingNightOrNshValue = electricityMeterReadingNightOrNshValue;			
			GasMeterReadingValue = gasMeterReadingValue;
			InformationCollectionAuthorized = informationCollectionAuthorized;
			TermsAndConditionsAccepted = termsAndConditionsAccepted;
			IncomingOccupant = incomingOccupant;
			LettingAgentName = lettingAgentName;
			LettingPhoneNumber = lettingPhoneNumber;
			OccupierDetailsAccepted = occupierDetailsAccepted;
			MovingOutDate = movingOutDate;
		}


		public virtual int ElectricityMeterReading24HrsOrDayValue { get; }
		public virtual int ElectricityMeterReadingNightOrNshValue { get; }		
		public virtual int GasMeterReadingValue { get; }


		public virtual bool InformationCollectionAuthorized { get; }
		public virtual bool TermsAndConditionsAccepted { get; }


		public virtual bool IncomingOccupant { get; }
		public virtual string LettingAgentName { get; }
		public virtual string LettingPhoneNumber { get; }
		public virtual bool OccupierDetailsAccepted { get; }

		public virtual DateTime MovingOutDate { get; }

		internal AccountInfo ElectricityAccount()
		{
			return ResolveAccount(ClientAccountType.Electricity);
		}

		internal AccountInfo GasAccount()
		{
			return ResolveAccount(ClientAccountType.Gas);
		}

		public bool Equals(RecordMovingOutProgress other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return base.Equals(other) && ElectricityMeterReading24HrsOrDayValue == other.ElectricityMeterReading24HrsOrDayValue && ElectricityMeterReadingNightOrNshValue == other.ElectricityMeterReadingNightOrNshValue && GasMeterReadingValue == other.GasMeterReadingValue && InformationCollectionAuthorized == other.InformationCollectionAuthorized && TermsAndConditionsAccepted == other.TermsAndConditionsAccepted && IncomingOccupant == other.IncomingOccupant && LettingAgentName == other.LettingAgentName && LettingPhoneNumber == other.LettingPhoneNumber && OccupierDetailsAccepted == other.OccupierDetailsAccepted && MovingOutDate.Equals(other.MovingOutDate);
		}

		private AccountInfo ResolveAccount(ClientAccountType clientAccountType)
		{
			if (InitiatedFromAccount.ClientAccountType == clientAccountType) return InitiatedFromAccount;
			if (OtherAccount != null && OtherAccount.ClientAccountType == clientAccountType)
				return OtherAccount;
			return null;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((RecordMovingOutProgress) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = base.GetHashCode();
				hashCode = (hashCode * 397) ^ ElectricityMeterReading24HrsOrDayValue;
				hashCode = (hashCode * 397) ^ ElectricityMeterReadingNightOrNshValue;
				hashCode = (hashCode * 397) ^ GasMeterReadingValue;
				hashCode = (hashCode * 397) ^ InformationCollectionAuthorized.GetHashCode();
				hashCode = (hashCode * 397) ^ TermsAndConditionsAccepted.GetHashCode();
				hashCode = (hashCode * 397) ^ IncomingOccupant.GetHashCode();
				hashCode = (hashCode * 397) ^ (LettingAgentName != null ? LettingAgentName.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (LettingPhoneNumber != null ? LettingPhoneNumber.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ OccupierDetailsAccepted.GetHashCode();
				hashCode = (hashCode * 397) ^ MovingOutDate.GetHashCode();
				return hashCode;
			}
		}

		public static bool operator ==(RecordMovingOutProgress left, RecordMovingOutProgress right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(RecordMovingOutProgress left, RecordMovingOutProgress right)
		{
			return !Equals(left, right);
		}
	}
}