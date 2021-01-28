using System;
using EI.RP.CoreServices.Cqrs.Commands;

namespace EI.RP.DomainServices.Commands.Premises.IncommingOccupants
{


	public class NotifyNewIncommingOccupant : DomainCommand, IEquatable<NotifyNewIncommingOccupant>
	{
		public NotifyNewIncommingOccupant(string accountNumber, string lettingAgentName, string lettingPhoneNumber)
		{
			LettingAgentName = lettingAgentName;
			LettingPhoneNumber = lettingPhoneNumber;
			AccountNumber = accountNumber;
		}


		public string LettingAgentName { get; }

		public string LettingPhoneNumber { get; }
		public string AccountNumber { get; }

		public bool Equals(NotifyNewIncommingOccupant other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(LettingAgentName, other.LettingAgentName) && string.Equals(LettingPhoneNumber, other.LettingPhoneNumber) && string.Equals(AccountNumber, other.AccountNumber);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((NotifyNewIncommingOccupant) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (LettingAgentName != null ? LettingAgentName.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (LettingPhoneNumber != null ? LettingPhoneNumber.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (AccountNumber != null ? AccountNumber.GetHashCode() : 0);
				return hashCode;
			}
		}
	}

}