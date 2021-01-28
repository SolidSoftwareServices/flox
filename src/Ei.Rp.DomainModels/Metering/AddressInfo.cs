using System;
using EI.RP.CoreServices.Cqrs.Queries;

using Ei.Rp.DomainModels.MappingValues;

namespace Ei.Rp.DomainModels.Metering
{
	public class AddressInfo : IQueryResult, IEquatable<AddressInfo>
	{
      
		public bool Equals(AddressInfo other)
        {
	        if (ReferenceEquals(null, other)) return false;
	        if (ReferenceEquals(this, other)) return true;
	        return PostalCode == other.PostalCode && Street == other.Street && Street2 == other.Street2 && Street3 == other.Street3 && Street4 == other.Street4 && Street5 == other.Street5 && DuosGroup == other.DuosGroup && HouseNo == other.HouseNo && HouseNo2 == other.HouseNo2 && CareOf == other.CareOf && City == other.City && Country == other.Country && ShortForm == other.ShortForm && Region == other.Region && District == other.District && POBoxPostalCode == other.POBoxPostalCode && POBox == other.POBox && Building == other.Building && Floor == other.Floor && RoomNo == other.RoomNo && CountryID == other.CountryID && CountryName == other.CountryName && RegionName == other.RegionName && TimeZone == other.TimeZone && TaxJurisdictionCode == other.TaxJurisdictionCode && LanguageID == other.LanguageID && HouseSupplement == other.HouseSupplement && AddressLine1 == other.AddressLine1 && AddressLine2 == other.AddressLine2 && AddressLine4 == other.AddressLine4 && AddressLine5 == other.AddressLine5 && Equals(AddressType, other.AddressType);
        }

        public override bool Equals(object obj)
        {
	        if (ReferenceEquals(null, obj)) return false;
	        if (ReferenceEquals(this, obj)) return true;
	        if (obj.GetType() != this.GetType()) return false;
	        return Equals((AddressInfo) obj);
        }

        public override int GetHashCode()
        {
	        unchecked
	        {
		        var hashCode = (PostalCode != null ? PostalCode.GetHashCode() : 0);
		        hashCode = (hashCode * 397) ^ (Street != null ? Street.GetHashCode() : 0);
		        hashCode = (hashCode * 397) ^ (Street2 != null ? Street2.GetHashCode() : 0);
		        hashCode = (hashCode * 397) ^ (Street3 != null ? Street3.GetHashCode() : 0);
		        hashCode = (hashCode * 397) ^ (Street4 != null ? Street4.GetHashCode() : 0);
		        hashCode = (hashCode * 397) ^ (Street5 != null ? Street5.GetHashCode() : 0);
		        hashCode = (hashCode * 397) ^ (DuosGroup != null ? DuosGroup.GetHashCode() : 0);
		        hashCode = (hashCode * 397) ^ (HouseNo != null ? HouseNo.GetHashCode() : 0);
		        hashCode = (hashCode * 397) ^ (HouseNo2 != null ? HouseNo2.GetHashCode() : 0);
		        hashCode = (hashCode * 397) ^ (CareOf != null ? CareOf.GetHashCode() : 0);
		        hashCode = (hashCode * 397) ^ (City != null ? City.GetHashCode() : 0);
		        hashCode = (hashCode * 397) ^ (Country != null ? Country.GetHashCode() : 0);
		        hashCode = (hashCode * 397) ^ (ShortForm != null ? ShortForm.GetHashCode() : 0);
		        hashCode = (hashCode * 397) ^ (Region != null ? Region.GetHashCode() : 0);
		        hashCode = (hashCode * 397) ^ (District != null ? District.GetHashCode() : 0);
		        hashCode = (hashCode * 397) ^ (POBoxPostalCode != null ? POBoxPostalCode.GetHashCode() : 0);
		        hashCode = (hashCode * 397) ^ (POBox != null ? POBox.GetHashCode() : 0);
		        hashCode = (hashCode * 397) ^ (Building != null ? Building.GetHashCode() : 0);
		        hashCode = (hashCode * 397) ^ (Floor != null ? Floor.GetHashCode() : 0);
		        hashCode = (hashCode * 397) ^ (RoomNo != null ? RoomNo.GetHashCode() : 0);
		        hashCode = (hashCode * 397) ^ (CountryID != null ? CountryID.GetHashCode() : 0);
		        hashCode = (hashCode * 397) ^ (CountryName != null ? CountryName.GetHashCode() : 0);
		        hashCode = (hashCode * 397) ^ (RegionName != null ? RegionName.GetHashCode() : 0);
		        hashCode = (hashCode * 397) ^ (TimeZone != null ? TimeZone.GetHashCode() : 0);
		        hashCode = (hashCode * 397) ^ (TaxJurisdictionCode != null ? TaxJurisdictionCode.GetHashCode() : 0);
		        hashCode = (hashCode * 397) ^ (LanguageID != null ? LanguageID.GetHashCode() : 0);
		        hashCode = (hashCode * 397) ^ (HouseSupplement != null ? HouseSupplement.GetHashCode() : 0);
		        hashCode = (hashCode * 397) ^ (AddressLine1 != null ? AddressLine1.GetHashCode() : 0);
		        hashCode = (hashCode * 397) ^ (AddressLine2 != null ? AddressLine2.GetHashCode() : 0);
		        hashCode = (hashCode * 397) ^ (AddressLine4 != null ? AddressLine4.GetHashCode() : 0);
		        hashCode = (hashCode * 397) ^ (AddressLine5 != null ? AddressLine5.GetHashCode() : 0);
		        hashCode = (hashCode * 397) ^ (AddressType != null ? AddressType.GetHashCode() : 0);
		        return hashCode;
	        }
        }

        public static bool operator ==(AddressInfo left, AddressInfo right)
        {
	        return Equals(left, right);
        }

        public static bool operator !=(AddressInfo left, AddressInfo right)
        {
	        return !Equals(left, right);
        }

        public string PostalCode { get; set; }
        public string Street { get; set; }
        public string Street2 { get; set; }
        public string Street3 { get; set; }
        public string Street4 { get; set; }
        public string Street5 { get; set; }
        public string DuosGroup { get; set; }
        public string HouseNo { get; set; }
        public string HouseNo2 { get; set; }
        public string CareOf { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string ShortForm { get; set; }
        public string Region { get; set; }
        public string District { get; set; }
        public string POBoxPostalCode { get; set; }
        public string POBox { get; set; }
        public string Building { get; set; }
        public string Floor { get; set; }
        public string RoomNo { get; set; }
        public string CountryID { get; set; }
        public string CountryName { get; set; }
        public string RegionName { get; set; }
        public string TimeZone { get; set; }
        public string TaxJurisdictionCode { get; set; }
        public string LanguageID { get; set; }
        public string HouseSupplement { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine4 { get; set; }
        public string AddressLine5 { get; set; }
        public AddressType AddressType { get; set; }

	
	}
}