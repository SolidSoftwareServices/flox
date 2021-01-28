using System;
using EI.RP.CoreServices.Cqrs.Queries;

namespace EI.RP.DomainServices.Queries.Geography.Country
{
    public class CountryDetailsQuery : IQueryModel, IEquatable<CountryDetailsQuery>
    {
	    public QueryCacheResultsType CacheResults { get; } = QueryCacheResultsType.UserSpecific;
		public string CountryId { get; set; }
        public bool HasCountry() => !string.IsNullOrWhiteSpace(CountryId);

        public bool IsValidQuery(out string[] notValidReasons)
        {
            notValidReasons = new string[0];
            return true;
        }

        public bool Equals(CountryDetailsQuery other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return CountryId == other.CountryId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CountryDetailsQuery) obj);
        }

        public override int GetHashCode()
        {
            return (CountryId != null ? CountryId.GetHashCode() : 0);
        }

        public static bool operator ==(CountryDetailsQuery left, CountryDetailsQuery right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CountryDetailsQuery left, CountryDetailsQuery right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"{nameof(CountryId)}: {CountryId}";
        }
    }
}
