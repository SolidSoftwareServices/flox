using System;
using EI.RP.CoreServices.Cqrs.Commands;

namespace EI.RP.DomainServices.Commands.Platform.PublishBusinessActivity
{
	internal class PublishBusinessActivityDomainCommand : DomainCommand,
		IEquatable<PublishBusinessActivityDomainCommand>
	{
		public enum BusinessActivityType
		{
			EditDirectDebit = 1,
			SubmitDirectDebit,
			MeterReading,
			AddAccount,
			RefundRequestForDirectDebit,
			RefundRequestForNonDirectDebit,
			PaymentResultSuccessful,
			PaymentResultFailed,
            EqualizerSetup,
            UpdateUserContactDetails,
            DeregisterSingleUserBp,
            DeregisterMultipleUserBp,
			MoveHouse
		}

		public PublishBusinessActivityDomainCommand(BusinessActivityType activityType, string businessPartner,
			string accountNumber, string documentStatus = "", string processType = "", string subject = "",
			string description = "")
		{
			ActivityType = activityType;
			BusinessPartner = businessPartner;
			AccountNumber = accountNumber;
			ProcessType = processType;
			DocumentStatus = documentStatus;
			Subject = subject;
			Description = description;
		}

		public BusinessActivityType ActivityType { get; }
		public string DocumentStatus { get; }
		public string BusinessPartner { get; }
		public string Subject { get; }
		public string AccountNumber { get; }
		public string Description { get; }
		public string ProcessType { get; }

		public bool Equals(PublishBusinessActivityDomainCommand other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return ActivityType == other.ActivityType && DocumentStatus == other.DocumentStatus &&
			       BusinessPartner == other.BusinessPartner && Subject == other.Subject &&
			       AccountNumber == other.AccountNumber && Description == other.Description &&
			       ProcessType == other.ProcessType;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((PublishBusinessActivityDomainCommand) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (int) ActivityType;
				hashCode = (hashCode * 397) ^ (DocumentStatus != null ? DocumentStatus.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (BusinessPartner != null ? BusinessPartner.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Subject != null ? Subject.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (AccountNumber != null ? AccountNumber.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (ProcessType != null ? ProcessType.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(PublishBusinessActivityDomainCommand left,
			PublishBusinessActivityDomainCommand right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(PublishBusinessActivityDomainCommand left,
			PublishBusinessActivityDomainCommand right)
		{
			return !Equals(left, right);
		}
	}
}