using EI.RP.CoreServices.Cqrs.Commands;
using System;

namespace EI.RP.DomainServices.Commands.Users.CompetitionEntry
{
    public sealed class CompetitionEntryCommand : DomainCommand, IEquatable<CompetitionEntryCommand>
    {
        public CompetitionEntryCommand(string userName, 
            string accountNumber,
            string firstName, 
            string surname,
            string email,
            string phoneNumber,
            string competitionName,
            DateTime competitionEntryDate,
            string answer,
            string bpNumber,
            bool termsConditionsAccepted,
            string ipAddress)
        {
            UserName = userName;
            AccountNumber = accountNumber;
            FirstName = firstName;
            Surname = surname;
            Email = email;
            PhoneNumber = phoneNumber;
            CompetitionName = competitionName;
            CompetitionEntryDate = competitionEntryDate;
            Answer = answer;
            BPNumber = bpNumber;
            TermsConditionsAccepted = termsConditionsAccepted;
            IpAddress = ipAddress;
        }

        public string Answer { get; }
        public string FirstName { get; }
        public string Surname { get; }
        public string Email { get; }
        public string PhoneNumber { get; }
        public bool TermsConditionsAccepted { get; }
        public string CompetitionName { get; }
        public DateTime CompetitionEntryDate { get; }
        public string UserName { get; }
        public string BPNumber { get; }
        public string AccountNumber { get; }
        public string IpAddress { get; }

		public bool Equals(CompetitionEntryCommand other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Answer, other.Answer) && 
                   string.Equals(FirstName, other.FirstName) && 
                   string.Equals(Surname, other.Surname) && 
                   string.Equals(Email, other.Email) && 
                   string.Equals(PhoneNumber, other.PhoneNumber) && 
                   TermsConditionsAccepted == other.TermsConditionsAccepted && 
                   string.Equals(CompetitionName, other.CompetitionName) &&
                   IsCompetitionEntryDateEqualWithOutSeconds(CompetitionEntryDate, other.CompetitionEntryDate) && 
                   string.Equals(UserName, other.UserName) && 
                   string.Equals(BPNumber, other.BPNumber)&& 
                   string.Equals(AccountNumber, other.AccountNumber)&& 
                   string.Equals(IpAddress, other.IpAddress);
        }

        private bool IsCompetitionEntryDateEqualWithOutSeconds(DateTime left, DateTime right)
        {
            var firstDate = new DateTime(left.Year, left.Month, left.Day, left.Hour, left.Minute, 0);
            var secondDate = new DateTime(right.Year, right.Month, right.Day, right.Hour, right.Minute, 0);
            return firstDate.Equals(secondDate);
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CompetitionEntryCommand) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Answer != null ? Answer.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (FirstName != null ? FirstName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Surname != null ? Surname.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Email != null ? Email.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (PhoneNumber != null ? PhoneNumber.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ TermsConditionsAccepted.GetHashCode();
                hashCode = (hashCode * 397) ^ (CompetitionName != null ? CompetitionName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ CompetitionEntryDate.GetHashCode();
                hashCode = (hashCode * 397) ^ (UserName != null ? UserName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (AccountNumber != null ? AccountNumber.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (BPNumber != null ? BPNumber.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (IpAddress != null ? IpAddress.GetHashCode() : 0);
                return hashCode;
            }
        }


    }
}
