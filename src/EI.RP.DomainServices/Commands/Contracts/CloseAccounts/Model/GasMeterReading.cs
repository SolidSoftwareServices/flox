using System;

namespace EI.RP.DomainServices.Commands.Contracts.CloseAccounts.Model
{
	public class GasMeterReading : IAccountMeterReading, IEquatable<GasMeterReading>
	{
		public GasMeterReading(string accountNumber, 
			int? meterReading)
		{
			AccountNumber = accountNumber;
			MeterReading = meterReading;
		}

		public string AccountNumber { get; }
		public int? MeterReading { get; }

		public bool Equals(GasMeterReading other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(AccountNumber, other.AccountNumber) && MeterReading == other.MeterReading;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((GasMeterReading) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((AccountNumber != null ? AccountNumber.GetHashCode() : 0) * 397) ^ MeterReading.GetHashCode();
			}
		}

		public static bool operator ==(GasMeterReading left, GasMeterReading right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(GasMeterReading left, GasMeterReading right)
		{
			return !Equals(left, right);
		}
	}
}