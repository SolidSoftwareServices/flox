using System;
using EI.RP.CoreServices.Cqrs.Commands;
using Ei.Rp.DomainModels.MappingValues;

namespace EI.RP.DomainServices.Commands.Platform.SendEmail
{
	public class SendEmailCommand : DomainCommand, IEquatable<SendEmailCommand>
	{
        public SendEmailCommand(ContactQueryType queryType, string subject, string messageContent,
            string accountNumber = null, string partner = null)
        {
            Subject = subject;
            MessageContent = messageContent;
            AccountNumber = accountNumber;
            Partner = partner;
            QueryType = queryType;
        }


        public string Partner { get; }
        public string Subject { get; }
        public string MessageContent { get; }
        public string AccountNumber { get; }
        public ContactQueryType QueryType { get; }


		public bool Equals(SendEmailCommand other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(QueryType, other.QueryType) && Partner == other.Partner && Subject == other.Subject && MessageContent == other.MessageContent && AccountNumber == other.AccountNumber;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((SendEmailCommand) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (QueryType != null ? QueryType.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Partner != null ? Partner.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Subject != null ? Subject.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (MessageContent != null ? MessageContent.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (AccountNumber != null ? AccountNumber.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(SendEmailCommand left, SendEmailCommand right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(SendEmailCommand left, SendEmailCommand right)
		{
			return !Equals(left, right);
		}
        
    }
}