using System;
using EI.RP.CoreServices.Cqrs.Commands;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.Metering;

namespace EI.RP.DomainServices.Commands.Contracts.CloseAccounts
{


	public partial class CloseAccountsCommand: IEquatable<CloseAccountsCommand>
	{

		public bool Equals(CloseAccountsCommand other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(ContractAccountType, other.ContractAccountType)
			       && Equals(MeterReadingElectricityAccount, other.MeterReadingElectricityAccount)
			       && Equals(MeterReadingGasAccount, other.MeterReadingGasAccount)
			       && Equals(AddressInfo, other.AddressInfo)
			       && Equals(MoveOutIncommingOccupant, other.MoveOutIncommingOccupant)
			       && MoveOutDate.Equals(other.MoveOutDate);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((CloseAccountsCommand) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (ContractAccountType != null ? ContractAccountType.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (MeterReadingElectricityAccount != null ? MeterReadingElectricityAccount.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (MeterReadingGasAccount != null ? MeterReadingGasAccount.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (AddressInfo != null ? AddressInfo.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (MoveOutIncommingOccupant != null ? MoveOutIncommingOccupant.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ MoveOutDate.GetHashCode();
				return hashCode;
			}
		}

		public static bool operator ==(CloseAccountsCommand left, CloseAccountsCommand right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(CloseAccountsCommand left, CloseAccountsCommand right)
		{
			return !Equals(left, right);
		}
	}

}