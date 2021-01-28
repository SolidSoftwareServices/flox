using System;
using EI.RP.CoreServices.Cqrs.Commands;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.MappingValues;

namespace EI.RP.DomainServices.Commands.Users.ContactUs
{
    public sealed class UserContactRequest: DomainCommand, IEquatable<UserContactRequest>
    {
        public UserContactRequest(string partner, string accountNumber,ElectricityPointReferenceNumber mprn, string subject,string comments, ContactQueryType contactType)
        {
            Partner = partner;
            AccountNumber = accountNumber;
            MPRN = mprn;
            Subject = subject;
            Comments = comments;
            ContactType = contactType;
        }

        public string Partner { get; }

        public string AccountNumber { get; }

        public ElectricityPointReferenceNumber MPRN { get; }

        public string Subject { get; }

        public string Comments { get; }

        public ContactQueryType ContactType { get; }

        public bool Equals(UserContactRequest other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Partner == other.Partner && AccountNumber == other.AccountNumber && MPRN == other.MPRN && Subject == other.Subject && Comments == other.Comments && Equals(ContactType, other.ContactType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UserContactRequest) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Partner != null ? Partner.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (AccountNumber != null ? AccountNumber.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (MPRN != null ? MPRN.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Subject != null ? Subject.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Comments != null ? Comments.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ContactType != null ? ContactType.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(UserContactRequest left, UserContactRequest right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(UserContactRequest left, UserContactRequest right)
        {
            return !Equals(left, right);
        }
    }
}
