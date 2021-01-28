using System;
using EI.RP.CoreServices.Cqrs.Commands;
using Ei.Rp.DomainModels.MappingValues;

namespace EI.RP.DomainServices.Commands.Banking.DirectDebit.SetUpDirectDebit
{
	public class SetUpDirectDebitDomainCommand : DomainCommand, IEquatable<SetUpDirectDebitDomainCommand>
	{
		public SetUpDirectDebitDomainCommand(
			string accountNumber,
			string nameOnBankAccount,
			string existingIban,
            string newIban,
			string businessPartner,
			ClientAccountType accountType,
			PaymentMethodType paymentMethodType,
            string contractId = null,
			DateTime? startDate = null,
			DateTime? firstDueDate = null)
		{
			AccountNumber = accountNumber;
			NameOnBankAccount = nameOnBankAccount;
			ExistingIBAN = existingIban;
			NewIBAN = newIban;
			BusinessPartner = businessPartner;
			IsNewBankAccount = string.IsNullOrWhiteSpace(existingIban);
            PaymentMethodType = paymentMethodType;
            AccountType = accountType;
            ContractId = contractId;
            FirstDueDate = firstDueDate;
            StartDate = startDate;
        }

		public string ExistingIBAN { get; }
		public string NewIBAN { get; }

		public string AccountNumber { get; }
		public string NameOnBankAccount { get; }
		public string BusinessPartner { get; }
		public bool IsNewBankAccount { get; }
        public ClientAccountType AccountType { get; }
        public PaymentMethodType PaymentMethodType { get; }
        public string ContractId { get; }
        public DateTime? FirstDueDate { get; }
        public DateTime? StartDate { get; }


        public bool Equals(SetUpDirectDebitDomainCommand other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return ExistingIBAN == other.ExistingIBAN && NewIBAN == other.NewIBAN &&
			       AccountNumber == other.AccountNumber && NameOnBankAccount == other.NameOnBankAccount &&
			       BusinessPartner == other.BusinessPartner && IsNewBankAccount == other.IsNewBankAccount &&
                   AccountType == other.AccountType && PaymentMethodType == other.PaymentMethodType;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((SetUpDirectDebitDomainCommand) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = ExistingIBAN != null ? ExistingIBAN.GetHashCode() : 0;
				hashCode = (hashCode * 397) ^ (NewIBAN != null ? NewIBAN.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (AccountNumber != null ? AccountNumber.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (NameOnBankAccount != null ? NameOnBankAccount.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (BusinessPartner != null ? BusinessPartner.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ IsNewBankAccount.GetHashCode();
                hashCode = (hashCode * 397) ^ PaymentMethodType.GetHashCode();
                hashCode = (hashCode * 397) ^ AccountType.GetHashCode();
                return hashCode;
			}
		}

		public static bool operator ==(SetUpDirectDebitDomainCommand left, SetUpDirectDebitDomainCommand right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(SetUpDirectDebitDomainCommand left, SetUpDirectDebitDomainCommand right)
		{
			return !Equals(left, right);
		}
	}
}