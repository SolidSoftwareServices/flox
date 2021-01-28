using System;
using EI.RP.CoreServices.Cqrs.Commands;

namespace EI.RP.DomainServices.Commands.Banking.ProcessPayment
{
	public abstract class PaymentResultCommand : DomainCommand, IEquatable<PaymentResultCommand>
	{
		public string Partner { get; protected set; }


		public string PaymentCardType { get; protected set; }
		public string PayerReference { get; protected set; }
		public string UserName { get; protected set; }
		public string AccountNumber { get; protected set; }

		public bool Equals(PaymentResultCommand other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Partner == other.Partner && PaymentCardType == other.PaymentCardType &&
			       PayerReference == other.PayerReference && UserName == other.UserName &&
			       AccountNumber == other.AccountNumber;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((PaymentResultCommand) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = Partner != null ? Partner.GetHashCode() : 0;
				hashCode = (hashCode * 397) ^ (PaymentCardType != null ? PaymentCardType.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (PayerReference != null ? PayerReference.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (UserName != null ? UserName.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (AccountNumber != null ? AccountNumber.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(PaymentResultCommand left, PaymentResultCommand right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(PaymentResultCommand left, PaymentResultCommand right)
		{
			return !Equals(left, right);
		}
	}
}