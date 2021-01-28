using System;
using EI.RP.CoreServices.Cqrs.Queries;

namespace EI.RP.DomainServices.Queries.Contracts.BusinessPartners
{
	public class BusinessPartnerQuery : IQueryModel, IEquatable<BusinessPartnerQuery>
	{
		public QueryCacheResultsType CacheResults { get; } = QueryCacheResultsType.UserSpecific;
		public string LastName { get; set; } = string.Empty;
		public string UserName { get; set; } = string.Empty;
		public string HouseNum { get; set; } = string.Empty;
		public string Street { get; set; } = string.Empty;
		public string City { get; set; } = string.Empty;
		public string PartnerNum { get; set; } = string.Empty;

		public int NumberOfRows { get; set; } = 100;

		public bool IsValidQuery(out string[] notValidReasons)
		{
			notValidReasons = new string[0];
			return true;
		}

		public bool Equals(BusinessPartnerQuery other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return LastName == other.LastName && UserName == other.UserName && HouseNum == other.HouseNum &&
			       Street == other.Street && City == other.City && PartnerNum == other.PartnerNum;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (!obj.GetType().IsInstanceOfType(this)) return false;
			return Equals((BusinessPartnerQuery)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (LastName != null ? LastName.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (UserName != null ? UserName.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (HouseNum != null ? HouseNum.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Street != null ? Street.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (City != null ? City.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (PartnerNum != null ? PartnerNum.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(BusinessPartnerQuery left, BusinessPartnerQuery right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(BusinessPartnerQuery left, BusinessPartnerQuery right)
		{
			return !Equals(left, right);
		}

		public override string ToString()
		{
			return
				$"{nameof(LastName)}: {LastName}, {nameof(UserName)}: {UserName}, {nameof(HouseNum)}: {HouseNum}, {nameof(Street)}: {Street}, {nameof(City)}: {City}, {nameof(PartnerNum)}: {PartnerNum}";
		}
	}
}