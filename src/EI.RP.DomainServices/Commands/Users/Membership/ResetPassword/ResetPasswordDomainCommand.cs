using System;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.System;

namespace EI.RP.DomainServices.Commands.Users.Membership.ResetPassword
{
	public class ResetPasswordDomainCommand : DomainCommand, IEquatable<ResetPasswordDomainCommand>
	{
		public ResetPasswordDomainCommand(string email)
		{
			Email = email;
		}

		public EmailAddress Email { get; }

		public bool Equals(ResetPasswordDomainCommand other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Email, other.Email);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((ResetPasswordDomainCommand) obj);
		}

		public override int GetHashCode()
		{
			return Email != null ? Email.GetHashCode() : 0;
		}

		public static bool operator ==(ResetPasswordDomainCommand left, ResetPasswordDomainCommand right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(ResetPasswordDomainCommand left, ResetPasswordDomainCommand right)
		{
			return !Equals(left, right);
		}
	}
}