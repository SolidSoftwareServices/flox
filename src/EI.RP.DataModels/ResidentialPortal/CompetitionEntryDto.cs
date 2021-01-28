using System;

namespace EI.RP.DataModels.ResidentialPortal
{
	public class CompetitionEntryDto : IEquatable<CompetitionEntryDto>
	{


		public int Id { get; set; }
		public string Answer { get; set; }
		public string FirstName { get; set; }
		public string Surname { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
		public bool TCAccepted { get; set; }
		public string CompetitionName { get; set; }
		public DateTime CompetitionEntryDate { get; set; }
		public string UserName { get; set; }
		public string BPNumber { get; set; }
		public string IpAddress { get; set; }


		public bool Equals(CompetitionEntryDto other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Id == other.Id && 
			       Answer == other.Answer && 
			       FirstName == other.FirstName &&
				   Surname == other.Surname && 
			       Email == other.Email && 
			       PhoneNumber == other.PhoneNumber &&
				   TCAccepted == other.TCAccepted && 
			       CompetitionName == other.CompetitionName &&
				   UserName == other.UserName && 
			       BPNumber == other.BPNumber &&
				   IpAddress == other.IpAddress;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((CompetitionEntryDto)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = Id;
				hashCode = (hashCode * 397) ^ (Answer != null ? Answer.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (FirstName != null ? FirstName.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Surname != null ? Surname.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Email != null ? Email.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (PhoneNumber != null ? PhoneNumber.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ TCAccepted.GetHashCode();
				hashCode = (hashCode * 397) ^ (CompetitionName != null ? CompetitionName.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (UserName != null ? UserName.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (BPNumber != null ? BPNumber.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (IpAddress != null ? IpAddress.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(CompetitionEntryDto left, CompetitionEntryDto right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(CompetitionEntryDto left, CompetitionEntryDto right)
		{
			return !Equals(left, right);
		}
	}
}
