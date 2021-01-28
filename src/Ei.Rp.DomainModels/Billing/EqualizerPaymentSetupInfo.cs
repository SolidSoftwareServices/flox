using System;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;

namespace Ei.Rp.DomainModels.Billing
{
	public class EqualizerPaymentSetupInfo : IQueryResult, IEquatable<EqualizerPaymentSetupInfo>
	{
		public string AccountNumber { get; set; }

		public EuroMoney Amount { get; set; }

		public bool CanSetUpEqualizer { get; set; }
        public DateTime? StartDate { get; set; }
        public string ContractId { get; set; }

        public bool Equals(EqualizerPaymentSetupInfo other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return AccountNumber == other.AccountNumber && Equals(Amount, other.Amount) && CanSetUpEqualizer == other.CanSetUpEqualizer;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((EqualizerPaymentSetupInfo) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (AccountNumber != null ? AccountNumber.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Amount != null ? Amount.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ CanSetUpEqualizer.GetHashCode();
				return hashCode;
			}
		}

		public static bool operator ==(EqualizerPaymentSetupInfo left, EqualizerPaymentSetupInfo right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(EqualizerPaymentSetupInfo left, EqualizerPaymentSetupInfo right)
		{
			return !Equals(left, right);
		}

		public override string ToString()
		{
			return $"{nameof(AccountNumber)}: {AccountNumber}, {nameof(Amount)}: {Amount}, {nameof(CanSetUpEqualizer)}: {CanSetUpEqualizer}";
		}
	}
}