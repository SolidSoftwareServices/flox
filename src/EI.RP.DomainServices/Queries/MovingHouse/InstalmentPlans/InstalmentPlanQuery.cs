using EI.RP.CoreServices.Cqrs.Queries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EI.RP.DomainServices.Queries.MovingHouse.InstalmentPlans
{
	public class InstalmentPlanQuery
        : IQueryModel, IEquatable<InstalmentPlanQuery>
	{
		public QueryCacheResultsType CacheResults { get; } = QueryCacheResultsType.UserSpecific;

        public string AccountNumber { get; set; }

        public bool IsValidQuery(out string[] notValidReasons)
		{
			var result = new List<string>();
			if (string.IsNullOrEmpty(AccountNumber))
			{
				result.Add("Must supply AccountNumber");
			}

			notValidReasons = result.ToArray();
			return !result.Any();
		}

		public bool Equals(InstalmentPlanQuery other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return AccountNumber == other.AccountNumber;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((InstalmentPlanQuery)obj);
		}

		public override int GetHashCode()
		{
			return (AccountNumber != null ? AccountNumber.GetHashCode() : 0);
		}

		public static bool operator ==(InstalmentPlanQuery left, InstalmentPlanQuery right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(InstalmentPlanQuery left, InstalmentPlanQuery right)
		{
			return !Equals(left, right);
		}

		public override string ToString()
		{
			return $"{nameof(AccountNumber)}: {AccountNumber}";
		}
	}
}