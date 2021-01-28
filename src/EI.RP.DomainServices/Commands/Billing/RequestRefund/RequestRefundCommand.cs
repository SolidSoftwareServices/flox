using System;
using EI.RP.CoreServices.Cqrs.Commands;
using Ei.Rp.DomainModels.MappingValues;

namespace EI.RP.DomainServices.Commands.Billing.RequestRefund
{
	public sealed class RequestRefundCommand : DomainCommand, IEquatable<RequestRefundCommand>
	{
        public RequestRefundCommand(string accountNumber, string partner,PaymentMethodType paymentMethod, string description)
		{
            PaymentMethod = paymentMethod;
			AccountNumber = accountNumber;
            Partner = partner;
            Description = description;
        }

		public string AccountNumber { get; }

        public PaymentMethodType PaymentMethod { get; }

        public string Partner { get; }
        public string Description { get; }

        public bool Equals(RequestRefundCommand other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return AccountNumber == other.AccountNumber ;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((RequestRefundCommand) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((AccountNumber != null ? AccountNumber.GetHashCode() : 0) * 397) ;
			}
		}

		public static bool operator ==(RequestRefundCommand left, RequestRefundCommand right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(RequestRefundCommand left, RequestRefundCommand right)
		{
			return !Equals(left, right);
		}
	}
}