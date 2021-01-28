using System;
using System.Collections.Generic;
using System.Linq;
using EI.RP.CoreServices.Cqrs.Queries;

namespace EI.RP.DomainServices.Queries.User.PhoneMetaData
{
    public class PhoneMetadataResolverQuery : IQueryModel, IEquatable<PhoneMetadataResolverQuery>
    {
	    public QueryCacheResultsType CacheResults { get; } = QueryCacheResultsType.UserSpecific;
		public string PhoneNumber { get; set; }

        public bool Equals(PhoneMetadataResolverQuery other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return PhoneNumber == other.PhoneNumber;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PhoneMetadataResolverQuery) obj);
        }

        public override int GetHashCode()
        {
            return (PhoneNumber != null ? PhoneNumber.GetHashCode() : 0);
        }

        public bool IsValidQuery(out string[] notValidReasons)
        {
            var result = new List<string>();
            if (string.IsNullOrWhiteSpace(PhoneNumber))
            {
                result.Add($"Must specify {nameof(PhoneNumber)}");
            }

            notValidReasons = result.ToArray();
            return !result.Any();
        }

        public static bool operator ==(PhoneMetadataResolverQuery left, PhoneMetadataResolverQuery right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PhoneMetadataResolverQuery left, PhoneMetadataResolverQuery right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"{nameof(PhoneNumber)}: {PhoneNumber}";
        }
    }
}
