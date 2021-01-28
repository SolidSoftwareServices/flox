using System;
using System.Collections.Generic;
using System.Linq;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using EI.RP.CoreServices.Cqrs.Queries;

namespace EI.RP.DomainServices.Queries.Metering.Premises
{

	public class PremisesQuery : IQueryModel, IEquatable<PremisesQuery>
	{
		public QueryCacheResultsType CacheResults { get; } = QueryCacheResultsType.UserSpecific;
		public string PremiseId { get; set; }
		public PointReferenceNumber Prn { get; set; }

		public bool IsValidQuery(out string[] notValidReasons)
		{
			var result = new List<string>();
			if (string.IsNullOrWhiteSpace(PremiseId)&& Prn==null)
			{
				result.Add($"Must specify {nameof(PremiseId)} or {nameof(Prn)}");
			}

			if (!string.IsNullOrWhiteSpace(PremiseId) && Prn != null)
			{
				result.Add($"Must specify {nameof(PremiseId)} or {nameof(Prn)}");
			}

			notValidReasons = result.ToArray();
			return !result.Any();
		}
		public bool Equals(PremisesQuery other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return !string.IsNullOrEmpty(PremiseId) ? PremiseId == other.PremiseId : Prn == other.Prn ;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((PremisesQuery)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = PremiseId != null ? PremiseId.GetHashCode() : 0;
				return hashCode;
			}
		}

		public static bool operator ==(PremisesQuery left, PremisesQuery right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(PremisesQuery left, PremisesQuery right)
		{
			return !Equals(left, right);
		}

		public override string ToString()
		{
			return $"{nameof(PremiseId)}: {PremiseId}";
		}
	}
}