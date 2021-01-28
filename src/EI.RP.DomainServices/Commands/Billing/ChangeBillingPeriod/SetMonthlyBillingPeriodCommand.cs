using System;
using EI.RP.CoreServices.Cqrs.Commands;

namespace EI.RP.DomainServices.Commands.Billing.ChangeBillingPeriod
{
	public sealed class SetMonthlyBillingPeriodCommand : DomainCommand, IEquatable<SetMonthlyBillingPeriodCommand>
	{
		
		public SetMonthlyBillingPeriodCommand(string accountNumber,int dayOfTheMonth)
		{
			AccountNumber = accountNumber;
			DayOfTheMonth = dayOfTheMonth;
		}

		public string AccountNumber { get; }
		public int DayOfTheMonth { get; }

		public bool Equals(SetMonthlyBillingPeriodCommand other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return AccountNumber == other.AccountNumber && DayOfTheMonth == other.DayOfTheMonth;
		}

		public override bool Equals(object obj)
		{
			return ReferenceEquals(this, obj) || obj is SetMonthlyBillingPeriodCommand other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((AccountNumber != null ? AccountNumber.GetHashCode() : 0) * 397) ^ DayOfTheMonth;
			}
		}

		public static bool operator ==(SetMonthlyBillingPeriodCommand left, SetMonthlyBillingPeriodCommand right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(SetMonthlyBillingPeriodCommand left, SetMonthlyBillingPeriodCommand right)
		{
			return !Equals(left, right);
		}
	}
}