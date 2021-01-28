using System;
using System.Collections.Generic;
using System.Linq;
using EI.RP.CoreServices.Cqrs.Queries;

namespace EI.RP.DomainServices.Queries.Metering.MeterReadings
{
	public class MeterReadingsQuery : IQueryModel, IEquatable<MeterReadingsQuery>
	{
		public QueryCacheResultsType CacheResults { get; } = QueryCacheResultsType.UserSpecific;
		public string AccountNumber { get; set; }
		public MeterReadingsPeriod MeterReadingsPeriodFrom { get; set; }

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
		public bool Equals(MeterReadingsQuery other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return AccountNumber == other.AccountNumber &&
			       MeterReadingsPeriodFrom == other.MeterReadingsPeriodFrom;
		}
		
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((MeterReadingsQuery) obj);
		}

		public override int GetHashCode()
		{
			return AccountNumber != null ? AccountNumber.GetHashCode() : 0;
		}

		public static bool operator ==(MeterReadingsQuery left, MeterReadingsQuery right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(MeterReadingsQuery left, MeterReadingsQuery right)
		{
			return !Equals(left, right);
		}

		public override string ToString()
		{
			return $"{nameof(AccountNumber)}: {AccountNumber}";
		}
	}

	public class MeterReadingsPeriod : IEquatable<MeterReadingsPeriod>
	{
		public int FromPeriodsAgo { get; set; }
		public PeriodUnit PeriodUnit { get; set; }

		public bool Equals(MeterReadingsPeriod other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return FromPeriodsAgo == other.FromPeriodsAgo && PeriodUnit == other.PeriodUnit;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((MeterReadingsPeriod) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (FromPeriodsAgo * 397) ^ (int) PeriodUnit;
			}
		}
	}

	public enum PeriodUnit
	{
		Months
	}
}