using System;
using System.Collections.Generic;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;

namespace EI.RP.DomainServices.Commands.Users.Membership.ChangePassword
{
	public class ChangePasswordCommand : DomainCommand, IEquatable<ChangePasswordCommand>
	{
		
		public ChangePasswordCommand(string userName, string currentPassword, string newPassword):this(userName,currentPassword,newPassword,null, null, null)
        {

        }
        public ChangePasswordCommand(string userName, string currentPassword,
            string newPassword,
            string activationKey, string activationStatus, string requestId)
		{
			UserName = userName;
			NewPassword = newPassword;
			CurrentPassword = currentPassword;
			ActivationKey = activationKey;
			ActivationStatus = activationStatus;
			RequestId = requestId;
		}

        public string UserName { get; }
		public string NewPassword { get; }
		public string CurrentPassword { get; }
		public string ActivationKey { get; }
		public string ActivationStatus { get; }
		public string RequestId { get; }

		public bool Equals(ChangePasswordCommand other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return UserName == other.UserName && NewPassword == other.NewPassword &&
			       CurrentPassword == other.CurrentPassword && ActivationKey == other.ActivationKey &&
			       ActivationStatus == other.ActivationStatus && RequestId == other.RequestId;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((ChangePasswordCommand) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = UserName != null ? UserName.GetHashCode() : 0;
				hashCode = (hashCode * 397) ^ (NewPassword != null ? NewPassword.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (CurrentPassword != null ? CurrentPassword.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (ActivationKey != null ? ActivationKey.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (ActivationStatus != null ? ActivationStatus.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (RequestId != null ? RequestId.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(ChangePasswordCommand left, ChangePasswordCommand right)
		{
			return Equals(left, right);
		}

        public static bool operator !=(ChangePasswordCommand left, ChangePasswordCommand right)
        {
			return !Equals(left, right);
		}
	}
}