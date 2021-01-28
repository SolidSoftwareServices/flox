using EI.RP.CoreServices.Cqrs.Commands;
using System;

namespace EI.RP.DomainServices.Commands.Users.MarketingPreferences
{
	public class UpdateMarketingPreferencesCommand : DomainCommand, IEquatable<UpdateMarketingPreferencesCommand>
	{
		public UpdateMarketingPreferencesCommand(string accountNumber,
			bool smsMarketingActive,
			bool landLineMarketingActive,
			bool mobileMarketingActive,
			bool postMarketingActive,
			bool doorToDoorMarketingActive,
			bool emailMarketingActive)
		{
			AccountNumber = accountNumber;
			SmsMarketingActive = smsMarketingActive;
			DoorToDoorMarketingActive = doorToDoorMarketingActive;
			LandLineMarketingActive = landLineMarketingActive;
			PostMarketingActive = postMarketingActive;
			EmailMarketingActive = emailMarketingActive;
			MobileMarketingActive = mobileMarketingActive;
		}

		public string AccountNumber { get; }

		public bool SmsMarketingActive { get; }

		public bool DoorToDoorMarketingActive { get; }

		public bool LandLineMarketingActive { get; }

		public bool PostMarketingActive { get; }

		public bool EmailMarketingActive { get; }

		public bool MobileMarketingActive { get; }


		public bool Equals(UpdateMarketingPreferencesCommand other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return AccountNumber == other.AccountNumber &&
			       SmsMarketingActive == other.SmsMarketingActive &&
				   DoorToDoorMarketingActive == other.DoorToDoorMarketingActive &&
				   LandLineMarketingActive == other.LandLineMarketingActive &&
				   PostMarketingActive == other.PostMarketingActive &&
				   EmailMarketingActive == other.EmailMarketingActive &&
				   MobileMarketingActive == other.MobileMarketingActive;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((UpdateMarketingPreferencesCommand)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (AccountNumber != null ? AccountNumber.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (SmsMarketingActive != null ? SmsMarketingActive.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (DoorToDoorMarketingActive != null ? DoorToDoorMarketingActive.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (LandLineMarketingActive != null ? LandLineMarketingActive.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (PostMarketingActive != null ? PostMarketingActive.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (EmailMarketingActive != null ? EmailMarketingActive.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (MobileMarketingActive != null ? MobileMarketingActive.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(UpdateMarketingPreferencesCommand left, UpdateMarketingPreferencesCommand right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(UpdateMarketingPreferencesCommand left, UpdateMarketingPreferencesCommand right)
		{
			return !Equals(left, right);
		}
	}
}
