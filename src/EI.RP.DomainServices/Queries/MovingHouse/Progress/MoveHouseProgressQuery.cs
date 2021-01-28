using System;
using System.Collections.Generic;
using System.Linq;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;

namespace EI.RP.DomainServices.Queries.MovingHouse.Progress
{
	public class MoveHouseProgressQuery : IQueryModel, IEquatable<MoveHouseProgressQuery>
	{
		public QueryCacheResultsType CacheResults { get; } = QueryCacheResultsType.UserSpecific;
		public MovingHouseType MoveType { get; set; }
		public AccountInfo InitiatedFromAccount { get; set; }

		public AccountInfo OtherAccount { get; set; }

		public bool Equals(MoveHouseProgressQuery other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(MoveType, other.MoveType) && Equals(InitiatedFromAccount, other.InitiatedFromAccount) &&
			       Equals(OtherAccount, other.OtherAccount);
		}

		public bool IsValidQuery(out string[] notValidReasons)
		{
			var result = new List<string>();
			if (InitiatedFromAccount == null) result.Add($"Must specify {nameof(InitiatedFromAccount)}");
			if (MoveType == null) result.Add($"Must specify {nameof(MoveType)}");

			notValidReasons = result.ToArray();
			return !result.Any();
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((MoveHouseProgressQuery) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = MoveType != null ? MoveType.GetHashCode() : 0;
				hashCode = (hashCode * 397) ^ (InitiatedFromAccount != null ? InitiatedFromAccount.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (OtherAccount != null ? OtherAccount.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(MoveHouseProgressQuery left, MoveHouseProgressQuery right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(MoveHouseProgressQuery left, MoveHouseProgressQuery right)
		{
			return !Equals(left, right);
		}

		public override string ToString()
		{
			return
				$"{nameof(InitiatedFromAccount)}: {InitiatedFromAccount?.AccountNumber}, {nameof(OtherAccount)}: {OtherAccount?.AccountNumber}";
		}
	}
}