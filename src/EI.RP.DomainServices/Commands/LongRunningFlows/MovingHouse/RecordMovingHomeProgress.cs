using System;
using System.Collections.Generic;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Queries.MovingHouse.Progress;

namespace EI.RP.DomainServices.Commands.LongRunningFlows.MovingHouse
{
	public abstract class RecordMovingHomeProgress:DomainCommand, IEquatable<RecordMovingHomeProgress>
	{
		protected RecordMovingHomeProgress(MovingHouseType moveType, AccountInfo initiatedFromAccount, AccountInfo otherAccount=null)
		{
			MoveType = moveType ?? throw new ArgumentNullException(nameof(moveType));
			InitiatedFromAccount = initiatedFromAccount;
			OtherAccount = otherAccount;
		}

		public MovingHouseType MoveType { get; }
		public AccountInfo InitiatedFromAccount { get; }
		public AccountInfo OtherAccount { get; }

		public override IEnumerable<IQueryModel> InvalidateQueriesOnSuccess =>((IQueryModel)new MoveHouseProgressQuery
		{
			MoveType = this.MoveType,InitiatedFromAccount = InitiatedFromAccount,OtherAccount = OtherAccount
		}).ToOneItemArray();

		public bool Equals(RecordMovingHomeProgress other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(MoveType, other.MoveType) && Equals(InitiatedFromAccount, other.InitiatedFromAccount) && Equals(OtherAccount, other.OtherAccount);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((RecordMovingHomeProgress) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (MoveType != null ? MoveType.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (InitiatedFromAccount != null ? InitiatedFromAccount.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (OtherAccount != null ? OtherAccount.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(RecordMovingHomeProgress left, RecordMovingHomeProgress right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(RecordMovingHomeProgress left, RecordMovingHomeProgress right)
		{
			return !Equals(left, right);
		}
	}
}