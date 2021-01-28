using System;
using System.Collections.Generic;
using System.Linq;
using EI.RP.CoreServices.Cqrs.Queries;

namespace EI.RP.DomainServices.Queries.Billing.InvoiceFiles
{
	public class InvoiceFileQuery : IQueryModel, IEquatable<InvoiceFileQuery>
	{
		public string ReferenceDocNumber { get; set; }
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
		public bool Equals(InvoiceFileQuery other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return ReferenceDocNumber == other.ReferenceDocNumber && AccountNumber == other.AccountNumber;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((InvoiceFileQuery) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((ReferenceDocNumber != null ? ReferenceDocNumber.GetHashCode() : 0) * 397) ^ (AccountNumber != null ? AccountNumber.GetHashCode() : 0);
			}
		}

		public static bool operator ==(InvoiceFileQuery left, InvoiceFileQuery right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(InvoiceFileQuery left, InvoiceFileQuery right)
		{
			return !Equals(left, right);
		}

		public override string ToString()
		{
			return $"{nameof(ReferenceDocNumber)}: {ReferenceDocNumber}, {nameof(AccountNumber)}: {AccountNumber}";
		}
	}
}