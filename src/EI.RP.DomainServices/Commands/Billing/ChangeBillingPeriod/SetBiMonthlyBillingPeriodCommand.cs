using System;
using EI.RP.CoreServices.Cqrs.Commands;
using Ei.Rp.DomainModels.MappingValues;

namespace EI.RP.DomainServices.Commands.Billing.ChangeBillingPeriod
{
	public sealed class SetBiMonthlyBillingPeriodCommand : DomainCommand, IEquatable<SetBiMonthlyBillingPeriodCommand>
	{
		public string AccountNumber { get; }

		public SetBiMonthlyBillingPeriodCommand(string accountNumber)
		{
			AccountNumber = accountNumber;
		}

		public bool Equals(SetBiMonthlyBillingPeriodCommand other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return AccountNumber == other.AccountNumber;
		}

		public override bool Equals(object obj)
		{
			return ReferenceEquals(this, obj) || obj is SetBiMonthlyBillingPeriodCommand other && Equals(other);
		}

		public override int GetHashCode()
		{
			return (AccountNumber != null ? AccountNumber.GetHashCode() : 0);
		}

		public static bool operator ==(SetBiMonthlyBillingPeriodCommand left, SetBiMonthlyBillingPeriodCommand right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(SetBiMonthlyBillingPeriodCommand left, SetBiMonthlyBillingPeriodCommand right)
		{
			return !Equals(left, right);
		}
	}
}