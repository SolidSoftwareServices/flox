using EI.RP.CoreServices.Cqrs.Commands;
using System;

namespace EI.RP.DomainServices.Commands.Users.SmartActivationNotification.DismissSmartActivationNotification
{
	public class DismissSmartActivationNotificationCommand : DomainCommand, IEquatable<DismissSmartActivationNotificationCommand>
    {
	    public DismissSmartActivationNotificationCommand(string accountNumber)
	    {
			AccountNumber = accountNumber;
	    }

	    public string AccountNumber { get; }

		public bool Equals(DismissSmartActivationNotificationCommand other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(AccountNumber, other.AccountNumber);
		}
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((DismissSmartActivationNotificationCommand)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (AccountNumber != null ? AccountNumber.GetHashCode() : 0);
				return hashCode;
			}
		}

	}
}
