using System;
using System.Collections.Generic;
using System.Linq;
using EI.RP.CoreServices.Cqrs.Queries;

namespace EI.RP.DomainServices.Queries.User.UserContact
{
    public class UserContactDetailsQuery: IQueryModel, IEquatable<UserContactDetailsQuery>
    {
	    public QueryCacheResultsType CacheResults { get; } = QueryCacheResultsType.UserSpecific;
		public string AccountNumber { get; set; }

        public bool Equals(UserContactDetailsQuery other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return AccountNumber == other.AccountNumber;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UserContactDetailsQuery)obj);
        }

        public override int GetHashCode()
        {
            return (AccountNumber != null ? AccountNumber.GetHashCode() : 0);
        }

        public static bool operator ==(UserContactDetailsQuery left, UserContactDetailsQuery right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(UserContactDetailsQuery left, UserContactDetailsQuery right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"{nameof(AccountNumber)}: {AccountNumber}";
        }

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
    }
}
