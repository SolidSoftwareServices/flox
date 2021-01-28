using System;
using EI.RP.CoreServices.Cqrs.Commands;

namespace EI.RP.DomainServices.Commands.BusinessPartner.Deregister
{
    public class DeRegisterBusinessPartnerCommand: DomainCommand, IEquatable<DeRegisterBusinessPartnerCommand>
    {
		public DeRegisterBusinessPartnerCommand(string partnerNumber, bool isSingleUserBusinessPartner)
	    {
		    PartnerNumber = partnerNumber;
		    IsSingleUserBusinessPartner = isSingleUserBusinessPartner;
	    }

		public string PartnerNumber { get; }
		public bool IsSingleUserBusinessPartner { get; }

		public bool Equals(DeRegisterBusinessPartnerCommand other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return PartnerNumber == other.PartnerNumber && IsSingleUserBusinessPartner == other.IsSingleUserBusinessPartner;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((DeRegisterBusinessPartnerCommand) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (PartnerNumber != null ? PartnerNumber.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ IsSingleUserBusinessPartner.GetHashCode();
				return hashCode;
			}
		}

		public static bool operator ==(DeRegisterBusinessPartnerCommand left, DeRegisterBusinessPartnerCommand right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(DeRegisterBusinessPartnerCommand left, DeRegisterBusinessPartnerCommand right)
		{
			return !Equals(left, right);
		}
    }
}
