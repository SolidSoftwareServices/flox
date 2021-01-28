using System;
using System.Collections.Generic;
using System.Linq;
using EI.RP.CoreServices.Cqrs.Queries;

namespace EI.RP.DomainServices.Queries.Banking.PaymentCards
{
	public class PaymentCardsQuery : IQueryModel, IEquatable<PaymentCardsQuery>
	{
		public QueryCacheResultsType CacheResults { get; } = QueryCacheResultsType.UserSpecific;
		public string Partner { get; set; }

		public bool IsValidQuery(out string[] notValidReasons)
		{
			var result = new List<string>();
			if (string.IsNullOrWhiteSpace(Partner))
			{
				result.Add($"Must specify {nameof(Partner)}");
			}

			notValidReasons = result.ToArray();
			return !result.Any();
		}

		public bool Equals(PaymentCardsQuery other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Partner == other.Partner;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((PaymentCardsQuery) obj);
		}

		public override int GetHashCode()
		{
			return Partner != null ? Partner.GetHashCode() : 0;
		}

		public static bool operator ==(PaymentCardsQuery left, PaymentCardsQuery right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(PaymentCardsQuery left, PaymentCardsQuery right)
		{
			return !Equals(left, right);
		}

		public override string ToString()
		{
			return $"{nameof(Partner)}: {Partner}";
		}
	}
}