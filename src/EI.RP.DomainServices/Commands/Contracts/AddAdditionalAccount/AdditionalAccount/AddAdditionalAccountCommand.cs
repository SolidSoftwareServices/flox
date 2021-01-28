using System;
using EI.RP.CoreServices.Cqrs.Commands;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.MappingValues;

namespace EI.RP.DomainServices.Commands.Contracts.AddAdditionalAccount.AdditionalAccount
{
    public sealed class AddAdditionalAccountCommand: DomainCommand, IEquatable<AddAdditionalAccountCommand>
    {
        public AddAdditionalAccountCommand(string partner, string accountNumber, PointReferenceNumber mprnGprn, string subject, string comments, ContactQueryType contactType)
        {
            Partner = partner;
            AccountNumber = accountNumber;
            MPRNGPRN = mprnGprn;
            Subject = subject;
            Comments = comments;
            ContactType = contactType;
        }

        public string Partner { get; }

        public string AccountNumber { get; }

        public PointReferenceNumber MPRNGPRN { get; }

        public string Subject { get; }

        public string Comments { get; }

        public ContactQueryType ContactType { get; }

        public bool Equals(AddAdditionalAccountCommand other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Partner == other.Partner && AccountNumber == other.AccountNumber && MPRNGPRN == other.MPRNGPRN && Subject == other.Subject && Comments == other.Comments && Equals(ContactType, other.ContactType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AddAdditionalAccountCommand) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Partner != null ? Partner.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (AccountNumber != null ? AccountNumber.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (MPRNGPRN != null ? MPRNGPRN.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Subject != null ? Subject.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Comments != null ? Comments.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ContactType != null ? ContactType.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(AddAdditionalAccountCommand left, AddAdditionalAccountCommand right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AddAdditionalAccountCommand left, AddAdditionalAccountCommand right)
        {
            return !Equals(left, right);
        }
    }
}
