using System;
using EI.RP.CoreServices.Cqrs.Commands;

namespace EI.RP.DomainServices.Commands.Users.Membership.CreatePassword
{
	public class CreatePasswordCommand : DomainCommand, IEquatable<CreatePasswordCommand>
	{
		public CreatePasswordCommand(string newPassword, string activationKey, string requestId, string email,
			string currentPassword)
		{
			NewPassword = newPassword;
			ActivationKey = activationKey;
			RequestId = requestId;
			Email = email;
			CurrentPassword = currentPassword;
		}

		public string ActivationKey { get; }
		public string NewPassword { get; }
		public string RequestId { get; }
		public string Email { get; }
		public string CurrentPassword { get; }

		public bool Equals(CreatePasswordCommand other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return ActivationKey == other.ActivationKey && NewPassword == other.NewPassword &&
			       RequestId == other.RequestId && Email == other.Email && CurrentPassword == other.CurrentPassword;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((CreatePasswordCommand) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = ActivationKey != null ? ActivationKey.GetHashCode() : 0;
				hashCode = (hashCode * 397) ^ (NewPassword != null ? NewPassword.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (RequestId != null ? RequestId.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Email != null ? Email.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (CurrentPassword != null ? CurrentPassword.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(CreatePasswordCommand left, CreatePasswordCommand right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(CreatePasswordCommand left, CreatePasswordCommand right)
		{
			return !Equals(left, right);
		}
	}
}