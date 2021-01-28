using System;
using System.Collections.Generic;
using System.Linq;
using EI.RP.CoreServices.Cqrs.Queries;

namespace EI.RP.DomainServices.Queries.Geography.Region
{
    public class UtilitiesRegionQuery : IQueryModel, IEquatable<UtilitiesRegionQuery>
    {
	    public QueryCacheResultsType CacheResults { get; } = QueryCacheResultsType.UserSpecific;
		public string CountryId { get; set; }

        public bool Equals(UtilitiesRegionQuery other)
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
            return Equals((UtilitiesRegionQuery)obj);
        }

        public override int GetHashCode()
        {
            return (CountryId != null ? CountryId.GetHashCode() : 0);
        }

        public static bool operator ==(UtilitiesRegionQuery left, UtilitiesRegionQuery right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(UtilitiesRegionQuery left, UtilitiesRegionQuery right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"{nameof(CountryId)}: {CountryId}";
        }

        public bool IsValidQuery(out string[] notValidReasons)
        {
            var result = new List<string>();
            if (string.IsNullOrWhiteSpace(CountryId))
            {
                result.Add($"Must specify {nameof(CountryId)}");
            }

            notValidReasons = result.ToArray();
            return !result.Any();
        }
    }
}
