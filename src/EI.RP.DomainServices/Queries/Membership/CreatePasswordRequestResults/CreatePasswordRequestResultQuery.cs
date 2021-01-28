using System;
using System.Collections.Generic;
using System.Linq;
using EI.RP.CoreServices.Cqrs.Queries;

namespace EI.RP.DomainServices.Queries.Membership.CreatePasswordRequestResults
{
	public class CreatePasswordRequestResultQuery : IQueryModel, IEquatable<CreatePasswordRequestResultQuery>
	{
		public QueryCacheResultsType CacheResults { get; } = QueryCacheResultsType.UserSpecific;
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
		public bool Equals(CreatePasswordRequestResultQuery other)
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
			return Equals((CreatePasswordRequestResultQuery) obj);
		}

		public override int GetHashCode()
		{
			return (RequestId != null ? RequestId.GetHashCode() : 0);
		}

		public static bool operator ==(CreatePasswordRequestResultQuery left, CreatePasswordRequestResultQuery right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(CreatePasswordRequestResultQuery left, CreatePasswordRequestResultQuery right)
		{
			return !Equals(left, right);
		}

		public override string ToString()
		{
			return $"{nameof(RequestId)}: {RequestId}";
		}
	}
}