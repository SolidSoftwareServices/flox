using System;
using System.Collections.Generic;
using System.Linq;
using EI.RP.CoreServices.Cqrs.Queries;

namespace EI.RP.DomainServices.Queries.Billing.LatestBill
{
	public class LatestBillQuery : IQueryModel, IEquatable<LatestBillQuery>
	{
		public string AccountNumber { get; set; }
		public bool IsValidQuery(out string[] notValidReasons)
		{
			var result = new List<string>();
			if (string.IsNullOrWhiteSpace(AccountNumber))
			{
				result.Add($"Must specify {nameof(AccountNumber)}");
			}


			notValidReasons = result.ToArray();
			return !result.Any();
		}
		public QueryCacheResultsType CacheResults { get; } = QueryCacheResultsType.UserSpecific;
		public bool Equals(LatestBillQuery other)
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
			return Equals((LatestBillQuery) obj);
		}

		public override int GetHashCode()
		{
			return (AccountNumber != null ? AccountNumber.GetHashCode() : 0);
		}

		public static bool operator ==(LatestBillQuery left, LatestBillQuery right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(LatestBillQuery left, LatestBillQuery right)
		{
			return !Equals(left, right);
		}

		public override string ToString()
		{
			return $"{nameof(AccountNumber)}: {AccountNumber}";
		}
	}
}