using System;
using System.Collections.Generic;
using System.Linq;
using EI.RP.CoreServices.Cqrs.Queries;

namespace EI.RP.DomainServices.Queries.Contracts.CanCloseAccount
{
	public class CanCloseAccountQuery : IQueryModel, IEquatable<CanCloseAccountQuery>
	{
		public QueryCacheResultsType CacheResults { get; } = QueryCacheResultsType.UserSpecific;
		public bool IsValidQuery(out string[] notValidReasons)
		{
			var result = new List<string>();
			if (string.IsNullOrWhiteSpace(ElectricityAccountNumber)
			    && string.IsNullOrWhiteSpace(GasAccountNumber))
			{
				result.Add("Must specify an account");
			}

			if (ClosingDate < DateTime.Today.Subtract(TimeSpan.FromDays(365 * 2)) ||
			    ClosingDate > DateTime.Today.Add(TimeSpan.FromDays(365 * 2)))
			{
				//TODO: constrain range according to SAP... not documented
				result.Add("Date provided is not valid");
			}

			notValidReasons = result.ToArray();
			return !notValidReasons.Any();
		}

		public string ElectricityAccountNumber { get; set; }
		public string GasAccountNumber { get; set; }
		public DateTime ClosingDate { get; set; }
		public bool HasElectricityAccount()
		{
			return !string.IsNullOrEmpty(ElectricityAccountNumber);
		}
		public bool HasGasAccount()
		{
			return !string.IsNullOrEmpty(GasAccountNumber);
		}
		public bool Equals(CanCloseAccountQuery other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return ElectricityAccountNumber == other.ElectricityAccountNumber && GasAccountNumber == other.GasAccountNumber && ClosingDate.Equals(other.ClosingDate);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((CanCloseAccountQuery) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (ElectricityAccountNumber != null ? ElectricityAccountNumber.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (GasAccountNumber != null ? GasAccountNumber.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ ClosingDate.GetHashCode();
				return hashCode;
			}
		}

		public static bool operator ==(CanCloseAccountQuery left, CanCloseAccountQuery right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(CanCloseAccountQuery left, CanCloseAccountQuery right)
		{
			return !Equals(left, right);
		}

		public override string ToString()
		{
			return $"{nameof(ElectricityAccountNumber)}: {ElectricityAccountNumber}, {nameof(GasAccountNumber)}: {GasAccountNumber}, {nameof(ClosingDate)}: {ClosingDate}";
		}

		
	}
}