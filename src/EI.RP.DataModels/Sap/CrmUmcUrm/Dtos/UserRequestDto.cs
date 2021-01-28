using System;
using System.Collections.Generic;

namespace EI.RP.DataModels.Sap.CrmUmcUrm.Dtos
{
	public partial class UserRequestDto : IEquatable<UserRequestDto>
	{

		public bool Equals(UserRequestDto other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return ID == other.ID && UserName == other.UserName && FirstName == other.FirstName &&
			       LastName == other.LastName && EmailAddress == other.EmailAddress &&
			       PhoneNumber == other.PhoneNumber && UsrCategory == other.UsrCategory && Password == other.Password &&
			       StatusCode == other.StatusCode && AccountID == other.AccountID &&
			       BusinessAgreementID == other.BusinessAgreementID && Nullable.Equals(Birthday, other.Birthday) &&
			       CountryID == other.CountryID && RegionID == other.RegionID && City == other.City &&
			       PostalCode == other.PostalCode && Street == other.Street && HouseNo == other.HouseNo &&
			       RoomNo == other.RoomNo && PoD == other.PoD && AccountOwnerFlag == other.AccountOwnerFlag &&
			       TermsConditionsFlag == other.TermsConditionsFlag;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((UserRequestDto) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (ID != null ? ID.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (UserName != null ? UserName.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (FirstName != null ? FirstName.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (LastName != null ? LastName.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (EmailAddress != null ? EmailAddress.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (PhoneNumber != null ? PhoneNumber.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (UsrCategory != null ? UsrCategory.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Password != null ? Password.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (StatusCode != null ? StatusCode.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (AccountID != null ? AccountID.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (BusinessAgreementID != null ? BusinessAgreementID.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ Birthday.GetHashCode();
				hashCode = (hashCode * 397) ^ (CountryID != null ? CountryID.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (RegionID != null ? RegionID.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (City != null ? City.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (PostalCode != null ? PostalCode.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Street != null ? Street.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (HouseNo != null ? HouseNo.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (RoomNo != null ? RoomNo.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (PoD != null ? PoD.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ AccountOwnerFlag.GetHashCode();
				hashCode = (hashCode * 397) ^ TermsConditionsFlag.GetHashCode();
				return hashCode;
			}
		}

		public static bool operator ==(UserRequestDto left, UserRequestDto right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(UserRequestDto left, UserRequestDto right)
		{
			return !Equals(left, right);
		}
	}
}