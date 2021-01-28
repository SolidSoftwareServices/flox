using System;
using System.Collections.Generic;
using System.Linq;
using EI.RP.CoreServices.Cqrs.Queries;

namespace EI.RP.DomainServices.Queries.Competitions
{
	public class CompetitionQuery : IQueryModel, IEquatable<CompetitionQuery>
	{
		private string _userEmail;
		public QueryCacheResultsType CacheResults { get; } = QueryCacheResultsType.UserSpecific;

		public string UserEmail
		{
			get => _userEmail;
			set => _userEmail = value?.ToLowerInvariant();
		}

		public bool IsValidQuery(out string[] notValidReasons)
		{
			var result = new List<string>();
			if (string.IsNullOrWhiteSpace(UserEmail))
			{
				result.Add($"Must specify {nameof(UserEmail)}");
			}

			notValidReasons = result.ToArray();
			return !result.Any();
		}
		public bool Equals(CompetitionQuery other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return UserEmail == other.UserEmail;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((CompetitionQuery) obj);
		}

		public override int GetHashCode()
		{
			return UserEmail != null ? UserEmail.GetHashCode() : 0;
		}

		public static bool operator ==(CompetitionQuery left, CompetitionQuery right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(CompetitionQuery left, CompetitionQuery right)
		{
			return !Equals(left, right);
		}

		public override string ToString()
		{
			return $"{nameof(UserEmail)}: {UserEmail}";
		}
	}
}