using System;
using EI.RP.CoreServices.Cqrs.Queries;

namespace Ei.Rp.DomainModels.Banking
{
	public class PaymentCardInfo : IQueryResult, IEquatable<PaymentCardInfo>
	{
		public bool IsStandard { get; set; }
		public string CardHolder { get; set; }
		public string Description { get; set; }
		public string Issuer { get; set; }

		public bool Equals(PaymentCardInfo other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return IsStandard == other.IsStandard && CardHolder == other.CardHolder && Description == other.Description && Issuer == other.Issuer;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((PaymentCardInfo) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = IsStandard.GetHashCode();
				hashCode = (hashCode * 397) ^ (CardHolder != null ? CardHolder.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Issuer != null ? Issuer.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(PaymentCardInfo left, PaymentCardInfo right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(PaymentCardInfo left, PaymentCardInfo right)
		{
			return !Equals(left, right);
		}
	}
}