using System;
using System.Collections.Generic;

namespace EI.RP.DomainServices.Queries.Metering.Consumption
{

	public partial class AccountConsumptionQuery : IEquatable<AccountConsumptionQuery>
	{
	
		public override string ToString()
		{
			return $"{nameof(CacheResults)}: {CacheResults}, {nameof(AccountNumber)}: {AccountNumber}, {nameof(Range)}: {Range}, {nameof(AggregationType)}: {AggregationType}, {nameof(RetrievalType)}: {RetrievalType}, {nameof(FillResultWithZeroes)}: {FillResultWithZeroes}";
		}

		public bool Equals(AccountConsumptionQuery other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return CacheResults == other.CacheResults && AccountNumber == other.AccountNumber && Equals(Range, other.Range) && Equals(AggregationType, other.AggregationType) && RetrievalType == other.RetrievalType && FillResultWithZeroes == other.FillResultWithZeroes;
		}

		private sealed class AccountConsumptionQueryEqualityComparer : IEqualityComparer<AccountConsumptionQuery>
		{
			public bool Equals(AccountConsumptionQuery x, AccountConsumptionQuery y)
			{
				if (ReferenceEquals(x, y)) return true;
				if (ReferenceEquals(x, null)) return false;
				if (ReferenceEquals(y, null)) return false;
				if (x.GetType() != y.GetType()) return false;
				return x.CacheResults == y.CacheResults && x.AccountNumber == y.AccountNumber && Equals(x.Range, y.Range) && Equals(x.AggregationType, y.AggregationType) && x.FillResultWithZeroes == y.FillResultWithZeroes;
			}

			public int GetHashCode(AccountConsumptionQuery obj)
			{
				unchecked
				{
					var hashCode = obj.CacheResults.GetHashCode();
					hashCode = (hashCode * 397) ^ (obj.AccountNumber != null ? obj.AccountNumber.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (obj.Range != null ? obj.Range.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (obj.AggregationType != null ? obj.AggregationType.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ obj.FillResultWithZeroes.GetHashCode();
					return hashCode;
				}
			}
		}

		public static IEqualityComparer<AccountConsumptionQuery> AccountConsumptionQueryComparer { get; } = new AccountConsumptionQueryEqualityComparer();

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((AccountConsumptionQuery) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = CacheResults.GetHashCode();
				hashCode = (hashCode * 397) ^ (AccountNumber != null ? AccountNumber.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Range != null ? Range.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (AggregationType != null ? AggregationType.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (int) RetrievalType;
				hashCode = (hashCode * 397) ^ FillResultWithZeroes.GetHashCode();
				return hashCode;
			}
		}

		public static bool operator ==(AccountConsumptionQuery left, AccountConsumptionQuery right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(AccountConsumptionQuery left, AccountConsumptionQuery right)
		{
			return !Equals(left, right);
		}
	}
}