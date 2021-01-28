using EI.RP.CoreServices.Cqrs.Queries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EI.RP.DomainServices.Queries.MovingHouse.CheckMoveOutRequestResults
{
	public class CheckMoveOutRequestResultQuery
		: IQueryModel, IEquatable<CheckMoveOutRequestResultQuery>
	{
		public QueryCacheResultsType CacheResults { get; } = QueryCacheResultsType.UserSpecific;

		public string ContractID { get; set; }

		public bool IsValidQuery(out string[] notValidReasons)
		{
			var result = new List<string>();
			if (string.IsNullOrEmpty(ContractID))
			{
				result.Add("Must supply ContractID");
			}

			notValidReasons = result.ToArray();
			return !result.Any();
		}

		public bool Equals(CheckMoveOutRequestResultQuery other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return ContractID == other.ContractID;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((CheckMoveOutRequestResultQuery)obj);
		}

		public override int GetHashCode()
		{
			return (ContractID != null ? ContractID.GetHashCode() : 0);
		}

		public static bool operator ==(CheckMoveOutRequestResultQuery left, CheckMoveOutRequestResultQuery right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(CheckMoveOutRequestResultQuery left, CheckMoveOutRequestResultQuery right)
		{
			return !Equals(left, right);
		}

		public override string ToString()
		{
			return $"{nameof(ContractID)}: {ContractID}";
		}
	}
}