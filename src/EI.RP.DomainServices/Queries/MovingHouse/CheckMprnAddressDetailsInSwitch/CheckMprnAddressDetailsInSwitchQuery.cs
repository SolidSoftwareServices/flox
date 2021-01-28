using EI.RP.CoreServices.Cqrs.Queries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EI.RP.DomainServices.Queries.MovingHouse.CheckMprnAddressDetailsInSwitch
{
	public class CheckMprnAddressDetailsInSwitchQuery
        : IQueryModel, IEquatable<CheckMprnAddressDetailsInSwitchQuery>
	{
		public QueryCacheResultsType CacheResults { get; } = QueryCacheResultsType.UserSpecific;

        public string MPRN { get; set; }

        public bool IsValidQuery(out string[] notValidReasons)
		{
			var result = new List<string>();

            if (string.IsNullOrEmpty(MPRN))
            {
                result.Add($"Must supply {nameof(MPRN)}");
            }

            notValidReasons = result.ToArray();
			return !result.Any();
		}

		public bool Equals(CheckMprnAddressDetailsInSwitchQuery other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(MPRN, other.MPRN);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((CheckMprnAddressDetailsInSwitchQuery)obj);
		}

		public override int GetHashCode()
		{
            unchecked
            {
                var hashCode = (MPRN != null ? MPRN.GetHashCode() : 0);
                return hashCode;
            }
        }

		public static bool operator ==(CheckMprnAddressDetailsInSwitchQuery left, CheckMprnAddressDetailsInSwitchQuery right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(CheckMprnAddressDetailsInSwitchQuery left, CheckMprnAddressDetailsInSwitchQuery right)
		{
			return !Equals(left, right);
		}

		public override string ToString()
		{
			return $"{nameof(MPRN)}: {MPRN}";
		}
	}
}