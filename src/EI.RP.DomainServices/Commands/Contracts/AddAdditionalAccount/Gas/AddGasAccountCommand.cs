using System;
using EI.RP.CoreServices.Cqrs.Commands;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.Contracts;

namespace EI.RP.DomainServices.Commands.Contracts.AddAdditionalAccount.Gas
{
	public class AddGasAccountCommand : DomainCommand, IEquatable<AddGasAccountCommand>
    {
       
        public AddGasAccountCommand(GasPointReferenceNumber gprn, string baseElectricityAccountNumber, 
									decimal meterReading, PaymentSetUpType paymentSetUp, 
									string iban, string nameOnBankAccount)
		{
			GPRN = gprn;
			BaseElectricityAccountNumber = baseElectricityAccountNumber;
			MeterReading = meterReading;
            PaymentSetUp = paymentSetUp;
			IBAN = iban;
			NameOnBankAccount = nameOnBankAccount;
        }

		public GasPointReferenceNumber GPRN { get; }
		public string BaseElectricityAccountNumber { get; }
		public decimal MeterReading { get; }

        public PaymentSetUpType PaymentSetUp { get; }
		public string IBAN { get; }
		public string NameOnBankAccount { get; }

		public bool Equals(AddGasAccountCommand other)
        {
	        if (ReferenceEquals(null, other)) return false;
	        if (ReferenceEquals(this, other)) return true;
	        return GPRN == other.GPRN && BaseElectricityAccountNumber == other.BaseElectricityAccountNumber &&
	               MeterReading == other.MeterReading && PaymentSetUp == other.PaymentSetUp &&
				   IBAN == other.IBAN && NameOnBankAccount == other.NameOnBankAccount;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AddGasAccountCommand) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (GPRN != null ? GPRN.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (BaseElectricityAccountNumber != null ? BaseElectricityAccountNumber.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ MeterReading.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) PaymentSetUp;
				hashCode = (hashCode * 397) ^ (IBAN != null ? IBAN.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (NameOnBankAccount != null ? NameOnBankAccount.GetHashCode() : 0);
				return hashCode;
            }
        }

        public static bool operator ==(AddGasAccountCommand left, AddGasAccountCommand right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AddGasAccountCommand left, AddGasAccountCommand right)
        {
            return !Equals(left, right);
        }

        
    }
}