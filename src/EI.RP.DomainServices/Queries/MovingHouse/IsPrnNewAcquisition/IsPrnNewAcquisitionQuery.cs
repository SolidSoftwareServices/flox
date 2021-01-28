using System;
using System.Collections.Generic;
using System.Linq;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using EI.RP.CoreServices.Cqrs.Queries;

namespace EI.RP.DomainServices.Queries.MovingHouse.IsPrnNewAcquisition
{
	public class IsPrnNewAcquisitionQuery
		: IQueryModel, IEquatable<IsPrnNewAcquisitionQuery>
	{
		public QueryCacheResultsType CacheResults { get; } = QueryCacheResultsType.UserSpecific;

		public PointReferenceNumber Prn { get; set; }
		public bool IsPODNewAcquisition { get; set; }

		public bool IsValidQuery(out string[] notValidReasons)
		{
			var result = new List<string>();
			if (Prn == null)
			{
				result.Add("Must supply Prn");
			}

			notValidReasons = result.ToArray();
			return !result.Any();
		}

		public bool Equals(IsPrnNewAcquisitionQuery other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Prn, other.Prn) &&
				   Equals(IsPODNewAcquisition, other.IsPODNewAcquisition);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((IsPrnNewAcquisitionQuery)obj);
		}

		public override int GetHashCode()
		{
			return (Prn != null ? Prn.GetHashCode() : 0);
		}

		public static bool operator ==(IsPrnNewAcquisitionQuery left, IsPrnNewAcquisitionQuery right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(IsPrnNewAcquisitionQuery left, IsPrnNewAcquisitionQuery right)
		{
			return !Equals(left, right);
		}

		public override string ToString()
		{
			return $"{nameof(Prn)}: {Prn}, {nameof(IsPODNewAcquisition)}: {IsPODNewAcquisition}";
		}
	}
}