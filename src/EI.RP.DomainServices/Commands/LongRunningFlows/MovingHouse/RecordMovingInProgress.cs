using System;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.Contracts.Accounts;

namespace EI.RP.DomainServices.Commands.LongRunningFlows.MovingHouse
{
	public class RecordMovingInProgress : RecordMovingHomeProgress, IDomainCommand, IEquatable<RecordMovingInProgress>
	{
		public static bool operator ==(RecordMovingInProgress left, RecordMovingInProgress right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(RecordMovingInProgress left, RecordMovingInProgress right)
		{
			return !Equals(left, right);
		}

		public RecordMovingInProgress(MovingHouseType moveType,AccountInfo initiatedFromAccount, AccountInfo otherAccount,
			int electricityMeterReading24HrsOrDayValue, int electricityMeterReadingNightValueOrNshValue, int gasMeterReadingValue,
			DateTime movingInDate, string contactNumber, bool hasConfirmedAuthority, bool hasConfirmedTermsAndConditions):base(moveType, initiatedFromAccount, otherAccount)
		{
		
			ElectricityMeterReading24HrsOrDayValue = electricityMeterReading24HrsOrDayValue;
			ElectricityMeterReadingNightValueOrNshValue = electricityMeterReadingNightValueOrNshValue;
			GasMeterReadingValue = gasMeterReadingValue;
		
			MovingInDate = movingInDate;
			ContactNumber = contactNumber;
			HasConfirmedAuthority = hasConfirmedAuthority;
			HasConfirmedTermsAndConditions = hasConfirmedTermsAndConditions;
		}

		public virtual int ElectricityMeterReading24HrsOrDayValue { get; }
		public virtual int ElectricityMeterReadingNightValueOrNshValue { get; }

		public virtual int GasMeterReadingValue { get; }



		public virtual DateTime MovingInDate { get; }
		public string ContactNumber { get;}
		public bool HasConfirmedAuthority { get; }
		public bool HasConfirmedTermsAndConditions { get; }

		internal AccountInfo ElectricityAccount()
		{
			return ResolveAccount(ClientAccountType.Electricity);
		}

		internal AccountInfo GasAccount()
		{
			return ResolveAccount(ClientAccountType.Gas);
		}

		private AccountInfo ResolveAccount(ClientAccountType clientAccountType)
		{
			if (InitiatedFromAccount.ClientAccountType == clientAccountType) return InitiatedFromAccount;
			if (OtherAccount != null && OtherAccount.ClientAccountType == clientAccountType)
				return OtherAccount;
			return null;
		}

		public bool Equals(RecordMovingInProgress other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return base.Equals(other) && ElectricityMeterReading24HrsOrDayValue == other.ElectricityMeterReading24HrsOrDayValue && ElectricityMeterReadingNightValueOrNshValue == other.ElectricityMeterReadingNightValueOrNshValue && GasMeterReadingValue == other.GasMeterReadingValue && MovingInDate.Equals(other.MovingInDate) && ContactNumber == other.ContactNumber && HasConfirmedAuthority == other.HasConfirmedAuthority && HasConfirmedTermsAndConditions == other.HasConfirmedTermsAndConditions;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((RecordMovingInProgress) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = base.GetHashCode();
				hashCode = (hashCode * 397) ^ ElectricityMeterReading24HrsOrDayValue;
				hashCode = (hashCode * 397) ^ ElectricityMeterReadingNightValueOrNshValue;
				hashCode = (hashCode * 397) ^ GasMeterReadingValue;
				hashCode = (hashCode * 397) ^ MovingInDate.GetHashCode();
				hashCode = (hashCode * 397) ^ (ContactNumber != null ? ContactNumber.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ HasConfirmedAuthority.GetHashCode();
				hashCode = (hashCode * 397) ^ HasConfirmedTermsAndConditions.GetHashCode();
				return hashCode;
			}
		}
	}
}