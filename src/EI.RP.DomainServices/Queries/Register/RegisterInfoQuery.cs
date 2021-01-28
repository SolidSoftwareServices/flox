using System;
using System.Collections.Generic;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;

namespace EI.RP.DomainServices.Queries.Register
{
    public class RegisterInfoQuery : IQueryModel, IEquatable<RegisterInfoQuery>
    {
	    public QueryCacheResultsType CacheResults { get; } = QueryCacheResultsType.UserSpecific;
		public PointReferenceNumber Prn { get; set; }

        public bool Equals(RegisterInfoQuery other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Prn == other.Prn;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RegisterInfoQuery)obj);
        }

        public override int GetHashCode()
        {
            return (Prn != null ? Prn.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return $"{nameof(Prn)}: {Prn}";
        }

        public static bool operator ==(RegisterInfoQuery left, RegisterInfoQuery right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RegisterInfoQuery left, RegisterInfoQuery right)
        {
            return !Equals(left, right);
        }

        public bool IsValidQuery(out string[] notValidReasons)
        {
            notValidReasons = new string[0];
            return true;
        }
    }
}