using System;
using System.Collections.Generic;
using System.Linq;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.Billing;

namespace EI.RP.DomainServices.Queries.Billing.Activity
{
	public class AccountBillingActivityQuery : IQueryModel, IEquatable<AccountBillingActivityQuery>
	{
		private DateTime _maxDate = SapDateTimes.SapDateTimeMax;

		private DateTime _minDate = DateTime.Today.AddYears(-2).ToSapFilterDateTime();
		public string AccountNumber { get; set; }
		public AccountBillingActivity.ActivitySource Source { get; set; } = AccountBillingActivity.ActivitySource.All;

		public DateTime MinDate
		{
			get => _minDate;
			set => _minDate = value.ToSapFilterDateTime();
		}

		public DateTime MaxDate
		{
			get => _maxDate;
			set => _maxDate = value.ToSapFilterDateTime();
		}
		public QueryCacheResultsType CacheResults { get; } = QueryCacheResultsType.UserSpecific;
		public bool Equals(AccountBillingActivityQuery other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return AccountNumber == other.AccountNumber && Source == other.Source &&
			       MinDate.Date.Equals(other.MinDate.Date) &&
			       MaxDate.Equals(other.MaxDate.Date);
		}

		public bool IsValidQuery(out string[] notValidReasons)
		{
			var result = new List<string>();
			if (string.IsNullOrWhiteSpace(AccountNumber)) result.Add($"Must specify {nameof(AccountNumber)}");
			if (MinDate > MaxDate) result.Add($"{nameof(MinDate)} must be smaller than {nameof(MaxDate)}");

			notValidReasons = result.ToArray();
			return !result.Any();
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((AccountBillingActivityQuery) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = AccountNumber != null ? AccountNumber.GetHashCode() : 0;
				hashCode = (hashCode * 397) ^ (int) Source;
				hashCode = (hashCode * 397) ^ MinDate.GetHashCode();
				hashCode = (hashCode * 397) ^ MaxDate.GetHashCode();
				return hashCode;
			}
		}

		public static bool operator ==(AccountBillingActivityQuery left, AccountBillingActivityQuery right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(AccountBillingActivityQuery left, AccountBillingActivityQuery right)
		{
			return !Equals(left, right);
		}

		public override string ToString()
		{
			return
				$"{nameof(AccountNumber)}: {AccountNumber}, {nameof(Source)}: {Source}, {nameof(MinDate)}: {MinDate}, {nameof(MaxDate)}: {MaxDate}";
		}
	}
}