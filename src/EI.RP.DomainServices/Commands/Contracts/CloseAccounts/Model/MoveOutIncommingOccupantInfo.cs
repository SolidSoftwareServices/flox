using System;

namespace EI.RP.DomainServices.Commands.Contracts.CloseAccounts.Model
{
	public class MoveOutIncommingOccupantInfo : IEquatable<MoveOutIncommingOccupantInfo>
	{
		public bool IncomingOccupant { get; set; }

		public string LettingAgentName { get; set; }

		public string LettingPhoneNumber { get; set; }

		public bool Equals(MoveOutIncommingOccupantInfo other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return IncomingOccupant == other.IncomingOccupant && LettingAgentName == other.LettingAgentName && LettingPhoneNumber == other.LettingPhoneNumber;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((MoveOutIncommingOccupantInfo)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = IncomingOccupant.GetHashCode();
				hashCode = (hashCode * 397) ^ (LettingAgentName != null ? LettingAgentName.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (LettingPhoneNumber != null ? LettingPhoneNumber.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(MoveOutIncommingOccupantInfo left, MoveOutIncommingOccupantInfo right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(MoveOutIncommingOccupantInfo left, MoveOutIncommingOccupantInfo right)
		{
			return !Equals(left, right);
		}
	}
}