using System;

namespace EI.RP.DomainServices.Commands.Banking.ProcessPayment
{
	public sealed class ProcessPaymentSuccessfulResultCommand : PaymentResultCommand,
		IEquatable<ProcessPaymentSuccessfulResultCommand>
	{
		public ProcessPaymentSuccessfulResultCommand(string partner, string paymentCardType, string payerReference,
			string userName, string accountNumber)
		{
			if(accountNumber==0.ToString()) throw new ArgumentException("0 is not a valid value",nameof(accountNumber));

			Partner = partner;
			PaymentCardType = paymentCardType;
			PayerReference = payerReference;
			UserName = userName;
			AccountNumber = accountNumber ?? throw new ArgumentNullException(nameof(accountNumber));
		}

		public bool Equals(ProcessPaymentSuccessfulResultCommand other)
		{
			return base.Equals(other);
		}

		public static bool operator ==(ProcessPaymentSuccessfulResultCommand left,
			ProcessPaymentSuccessfulResultCommand right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(ProcessPaymentSuccessfulResultCommand left,
			ProcessPaymentSuccessfulResultCommand right)
		{
			return !Equals(left, right);
		}
	}
}