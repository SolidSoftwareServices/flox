using System;
using System.Collections.Generic;
using System.Linq;
using EI.RP.CoreServices.Cqrs.Queries;

namespace EI.RP.DomainServices.Queries.Membership.ForgotPasswordRequestResults
{
	public class ForgotPasswordRequestResultQuery : IQueryModel, IEquatable<ForgotPasswordRequestResultQuery>
	{
		public QueryCacheResultsType CacheResults { get; } = QueryCacheResultsType.NoCache;
		public string RequestId { get; set; }
		public bool IsValidQuery(out string[] notValidReasons)
		{
			var result = new List<string>();
			if (string.IsNullOrWhiteSpace(RequestId))
			{
				result.Add($"Must specify {nameof(RequestId)}");
			}


			notValidReasons = result.ToArray();
			return !result.Any();
		}
		public bool Equals(ForgotPasswordRequestResultQuery other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return RequestId == other.RequestId;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((ForgotPasswordRequestResultQuery) obj);
		}

		public override int GetHashCode()
		{
			return (RequestId != null ? RequestId.GetHashCode() : 0);
		}

		public static bool operator ==(ForgotPasswordRequestResultQuery left, ForgotPasswordRequestResultQuery right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(ForgotPasswordRequestResultQuery left, ForgotPasswordRequestResultQuery right)
		{
			return !Equals(left, right);
		}

		public override string ToString()
		{
			return $"{nameof(RequestId)}: {RequestId}";
		}
	}
}