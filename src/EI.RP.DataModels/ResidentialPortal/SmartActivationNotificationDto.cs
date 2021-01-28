using System;

namespace EI.RP.DataModels.ResidentialPortal
{
	[Serializable]
	public class SmartActivationNotificationDto : IEquatable<SmartActivationNotificationDto>
    {
		public string AccountNumber { get; set; }
		public bool IsNotificationDismissed { get; set; }
		public DateTime? DismissedDateTime { get; set; }
		public string UserName { get; set; }

		public void Validate()
		{
			if (string.IsNullOrEmpty(UserName))
			{
				throw new ArgumentNullException($"{nameof(UserName)} is null. {nameof(UserName)} shouldn't be null");
			}
			if (string.IsNullOrEmpty(AccountNumber))
			{
				throw new ArgumentNullException($"{nameof(AccountNumber)} is null. {nameof(AccountNumber)} shouldn't be null");
			}
		}

		public bool Equals(SmartActivationNotificationDto other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return UserName == other.UserName &&
				   AccountNumber == other.AccountNumber &&
				   IsNotificationDismissed == other.IsNotificationDismissed;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((SmartActivationNotificationDto)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (UserName != null ? UserName.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (AccountNumber != null ? AccountNumber.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ IsNotificationDismissed.GetHashCode();;
				return hashCode;
			}
		}

		public static bool operator ==(SmartActivationNotificationDto left, SmartActivationNotificationDto right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(SmartActivationNotificationDto left, SmartActivationNotificationDto right)
		{
			return !Equals(left, right);
		}
	}
}
