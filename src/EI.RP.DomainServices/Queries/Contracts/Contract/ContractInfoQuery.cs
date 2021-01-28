using System;
using System.Collections.Generic;
using System.Linq;
using EI.RP.CoreServices.Cqrs.Queries;

namespace EI.RP.DomainServices.Queries.Contracts.Contract
{
	public class ContractInfoQuery : IQueryModel, IEquatable<ContractInfoQuery>
	{
		public QueryCacheResultsType CacheResults => QueryCacheResultsType.UserSpecific;
		
		public string AccountNumber { get; set; }
	
		public bool IsValidQuery(out string[] notValidReasons)
		{
			var result=new List<string>();

			if (string.IsNullOrWhiteSpace(AccountNumber))
			{
				result.Add("Must specify an account");
			}

			notValidReasons = result.ToArray();
			return !notValidReasons.Any();
		}

		public bool Equals(ContractInfoQuery other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return CacheResults == other.CacheResults && AccountNumber == other.AccountNumber;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((ContractInfoQuery) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = CacheResults.GetHashCode();
				hashCode = (hashCode * 397) ^ (AccountNumber != null ? AccountNumber.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(ContractInfoQuery left, ContractInfoQuery right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(ContractInfoQuery left, ContractInfoQuery right)
		{
			return !Equals(left, right);
		}

		public override string ToString()
		{
			return $"{nameof(CacheResults)}: {CacheResults}, {nameof(AccountNumber)}: {AccountNumber}";
		}		
	}
}