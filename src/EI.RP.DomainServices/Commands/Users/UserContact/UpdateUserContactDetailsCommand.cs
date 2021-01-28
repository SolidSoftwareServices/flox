using EI.RP.CoreServices.Cqrs.Commands;
using System;

namespace EI.RP.DomainServices.Commands.Users.UserContact
{
	public class UpdateUserContactDetailsCommand : DomainCommand, IEquatable<UpdateUserContactDetailsCommand>
    {
        public UpdateUserContactDetailsCommand(string accountNumber, string primaryPhoneNumber, string alternativePhoneNumber, string contactEMail,
            string previousContactEMail)
        {
            AccountNumber = accountNumber;
            PrimaryPhoneNumber = primaryPhoneNumber;
            AlternativePhoneNumber = alternativePhoneNumber;
            ContactEMail = contactEMail;
            PreviousContactEMail = previousContactEMail;
        }

        public string AccountNumber { get;  }

        public string PreviousContactEMail { get;  }
        public string ContactEMail { get;  }

        public string PrimaryPhoneNumber { get;  }

        public string AlternativePhoneNumber { get;  }


        public bool Equals(UpdateUserContactDetailsCommand other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return AccountNumber == other.AccountNumber &&
                   PreviousContactEMail == other.PreviousContactEMail &&
                   ContactEMail == other.ContactEMail &&
                   PrimaryPhoneNumber == other.PrimaryPhoneNumber &&
                   AlternativePhoneNumber == other.AlternativePhoneNumber;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UpdateUserContactDetailsCommand)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (AccountNumber != null ? AccountNumber.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (PreviousContactEMail != null ? PreviousContactEMail.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ContactEMail != null ? ContactEMail.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (PrimaryPhoneNumber != null ? PrimaryPhoneNumber.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (AlternativePhoneNumber != null ? AlternativePhoneNumber.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(UpdateUserContactDetailsCommand left, UpdateUserContactDetailsCommand right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(UpdateUserContactDetailsCommand left, UpdateUserContactDetailsCommand right)
        {
            return !Equals(left, right);
        }
    }
}
