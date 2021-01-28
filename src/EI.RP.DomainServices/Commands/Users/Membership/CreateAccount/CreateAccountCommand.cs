using System;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.System;

namespace EI.RP.DomainServices.Commands.Users.Membership.CreateAccount
{
	public class CreateAccountCommand : DomainCommand, IEquatable<CreateAccountCommand>
	{
		public CreateAccountCommand(string accountNumber, string userEmail, string mprngprnLast6DigitsOf,
			string contactPhoneNumber, DateTime birthday, bool accountOwnerFlag,
			bool termsConditionsFlag)
		{
			AccountNumber = accountNumber;
			UserEmail = userEmail;
			MPRNGPRNLast6DigitsOf = mprngprnLast6DigitsOf;
			ContactPhoneNumber = contactPhoneNumber;
			Birthday = birthday;
			AccountOwnerFlag = accountOwnerFlag;
			TermsConditionsFlag = termsConditionsFlag;
		}

		protected CreateAccountCommand()
		{
		}

		public virtual string MPRNGPRNLast6DigitsOf { get; }
		public virtual string AccountNumber { get; }
		public virtual EmailAddress UserEmail { get; }
		public virtual string ContactPhoneNumber { get; }
		public virtual DateTime Birthday { get; }

		public virtual bool AccountOwnerFlag { get; }

		public virtual bool TermsConditionsFlag { get; }

		public bool Equals(CreateAccountCommand other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return MPRNGPRNLast6DigitsOf == other.MPRNGPRNLast6DigitsOf && AccountNumber == other.AccountNumber &&
			       Equals(UserEmail, other.UserEmail) && ContactPhoneNumber == other.ContactPhoneNumber &&
			       Birthday.Equals(other.Birthday) &&
			       AccountOwnerFlag == other.AccountOwnerFlag && TermsConditionsFlag == other.TermsConditionsFlag;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((CreateAccountCommand) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = MPRNGPRNLast6DigitsOf != null ? MPRNGPRNLast6DigitsOf.GetHashCode() : 0;
				hashCode = (hashCode * 397) ^ AccountNumber.GetHashCode();
				hashCode = (hashCode * 397) ^ (UserEmail != null ? UserEmail.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (ContactPhoneNumber != null ? ContactPhoneNumber.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ Birthday.GetHashCode();
				hashCode = (hashCode * 397) ^ AccountOwnerFlag.GetHashCode();
				hashCode = (hashCode * 397) ^ TermsConditionsFlag.GetHashCode();
				return hashCode;
			}
		}

		public static bool operator ==(CreateAccountCommand left, CreateAccountCommand right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(CreateAccountCommand left, CreateAccountCommand right)
		{
			return !Equals(left, right);
		}
	}
}