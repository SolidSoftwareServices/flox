using System;

namespace EI.RP.DomainServices.Commands.Banking.ProcessPayment
{
	public class ProcessPaymentFailedResultCommand : PaymentResultCommand, IEquatable<ProcessPaymentFailedResultCommand>
	{
		public ProcessPaymentFailedResultCommand(string partner, string paymentCardType, string payerReference,
			string userName, string accountNumber)
		{
			Partner = partner;
			PaymentCardType = paymentCardType;
			PayerReference = payerReference;
			UserName = userName;
			AccountNumber = accountNumber;
		}

		public bool Equals(ProcessPaymentFailedResultCommand other)
		{
			return base.Equals(other);
		}

		public static bool operator ==(ProcessPaymentFailedResultCommand left, ProcessPaymentFailedResultCommand right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(ProcessPaymentFailedResultCommand left, ProcessPaymentFailedResultCommand right)
		{
			return !Equals(left, right);
		}
	}
}