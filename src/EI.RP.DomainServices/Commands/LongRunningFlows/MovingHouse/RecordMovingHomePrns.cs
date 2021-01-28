using System;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.CoreServices.Cqrs.Commands;
using Ei.Rp.DomainModels.Contracts.Accounts;

namespace EI.RP.DomainServices.Commands.LongRunningFlows.MovingHouse
{
	public sealed class RecordMovingHomePrns : RecordMovingHomeProgress, IDomainCommand, IEquatable<RecordMovingHomePrns>
	{
		public static bool operator ==(RecordMovingHomePrns left, RecordMovingHomePrns right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(RecordMovingHomePrns left, RecordMovingHomePrns right)
		{
			return !Equals(left, right);
		}

		public ElectricityPointReferenceNumber NewMprn { get; }
		public GasPointReferenceNumber NewGprn { get; }

		public RecordMovingHomePrns(MovingHouseType moveType,AccountInfo initiatedFromAccount, AccountInfo otherAccount, ElectricityPointReferenceNumber newMprn, GasPointReferenceNumber newGprn):base(moveType,initiatedFromAccount,otherAccount)
		{
			NewMprn = newMprn;
			NewGprn = newGprn;
		}

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
		public bool Equals(RecordMovingHomePrns other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return base.Equals(other) && Equals(NewMprn, other.NewMprn) && Equals(NewGprn, other.NewGprn);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((RecordMovingHomePrns) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = base.GetHashCode();
				hashCode = (hashCode * 397) ^ (NewMprn != null ? NewMprn.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (NewGprn != null ? NewGprn.GetHashCode() : 0);
				return hashCode;
			}
		}
	}
}