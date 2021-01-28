using System;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.System;

namespace EI.RP.DomainServices.Commands.Users.Session.CreateUserSession
{
	public class CreateUserSessionCommand : DomainCommand, IEquatable<CreateUserSessionCommand>
	{
		public CreateUserSessionCommand(string userEmail, string password,
			bool adaptPasswordToSapConstraints = true, bool changingPassword=false,bool isServiceAccount=false)
		{
			UserEmail = userEmail;
			Password = password;
			AdaptPasswordToSapConstraints = adaptPasswordToSapConstraints;
            ChangingPassword = changingPassword;
            IsServiceAccount = isServiceAccount;
		}

		public EmailAddress UserEmail { get; }
		public string Password { get; }
		public bool AdaptPasswordToSapConstraints { get; }
        public bool ChangingPassword { get; }
        public bool IsServiceAccount { get; }


        public override bool InvalidatesCache { get; } = false;

        public bool Equals(CreateUserSessionCommand other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(UserEmail, other.UserEmail) && Password == other.Password && AdaptPasswordToSapConstraints == other.AdaptPasswordToSapConstraints && ChangingPassword == other.ChangingPassword && IsServiceAccount == other.IsServiceAccount;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((CreateUserSessionCommand) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (UserEmail != null ? UserEmail.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Password != null ? Password.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ AdaptPasswordToSapConstraints.GetHashCode();
				hashCode = (hashCode * 397) ^ ChangingPassword.GetHashCode();
				hashCode = (hashCode * 397) ^ IsServiceAccount.GetHashCode();
				return hashCode;
			}
		}

		public static bool operator ==(CreateUserSessionCommand left, CreateUserSessionCommand right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(CreateUserSessionCommand left, CreateUserSessionCommand right)
		{
			return !Equals(left, right);
		}

		public override string ToString()
		{
			return $"{nameof(UserEmail)}: {UserEmail}, {nameof(Password)}: {Password}, {nameof(AdaptPasswordToSapConstraints)}: {AdaptPasswordToSapConstraints}, {nameof(ChangingPassword)}: {ChangingPassword}, {nameof(IsServiceAccount)}: {IsServiceAccount}";
		}
	}
}