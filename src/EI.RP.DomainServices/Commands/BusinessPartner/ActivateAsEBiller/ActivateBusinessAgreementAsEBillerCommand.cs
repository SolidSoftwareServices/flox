using System;
using EI.RP.CoreServices.Cqrs.Commands;

namespace EI.RP.DomainServices.Commands.BusinessPartner.ActivateAsEBiller
{
	public class ActivateBusinessAgreementAsEBillerCommand : DomainCommand, IEquatable<ActivateBusinessAgreementAsEBillerCommand>
	{
		public ActivateBusinessAgreementAsEBillerCommand(string businessAgreementId)
		{
			BusinessAgreementID = businessAgreementId;
		}

		public string BusinessAgreementID { get; }

		public bool Equals(ActivateBusinessAgreementAsEBillerCommand other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return BusinessAgreementID == other.BusinessAgreementID;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((ActivateBusinessAgreementAsEBillerCommand)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (BusinessAgreementID != null ? BusinessAgreementID.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(ActivateBusinessAgreementAsEBillerCommand left, ActivateBusinessAgreementAsEBillerCommand right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(ActivateBusinessAgreementAsEBillerCommand left, ActivateBusinessAgreementAsEBillerCommand right)
		{
			return !Equals(left, right);
		}
	}
}
