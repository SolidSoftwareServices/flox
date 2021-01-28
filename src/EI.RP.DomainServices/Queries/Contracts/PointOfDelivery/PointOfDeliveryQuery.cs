using EI.RP.CoreServices.Cqrs.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Newtonsoft.Json;

namespace EI.RP.DomainServices.Queries.Contracts.PointOfDelivery
{
    public class PointOfDeliveryQuery : IQueryModel, IEquatable<PointOfDeliveryQuery>
    {
        public PointReferenceNumber Prn { get; set; }

		public QueryCacheResultsType CacheResults { get; } = QueryCacheResultsType.UserSpecific;

		public bool IsValidQuery(out string[] notValidReasons)
        {
            var result = new List<string>();
            if (Prn == null)
            {
                result.Add($"Must specify {nameof(Prn)}");
            }


            notValidReasons = result.ToArray();
            return !result.Any();
        }
        public bool Equals(PointOfDeliveryQuery other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Prn == other.Prn;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((PointOfDeliveryQuery)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Prn != null ? Prn.GetHashCode() : 0;
                return hashCode;
            }
        }

        public static bool operator ==(PointOfDeliveryQuery left, PointOfDeliveryQuery right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PointOfDeliveryQuery left, PointOfDeliveryQuery right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"{nameof(Prn)}: {Prn}";
        }
    }
}
