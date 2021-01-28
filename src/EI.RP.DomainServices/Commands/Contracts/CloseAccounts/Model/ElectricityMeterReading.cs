using System;

namespace EI.RP.DomainServices.Commands.Contracts.CloseAccounts.Model
{

	

	public class ElectricityMeterReading : IAccountMeterReading, IEquatable<ElectricityMeterReading>
	{
		public ElectricityMeterReading(string accountNumber, int? meterReading24Hrs=null, int? meterReadingDay=null, int? meterReadingNight=null)
		{
			if (!meterReading24Hrs.HasValue && !meterReadingDay.HasValue && !meterReadingNight.HasValue)
			{
				throw new ArgumentException();
			}

			AccountNumber = accountNumber ?? throw new ArgumentNullException(nameof(accountNumber));
			MeterReading24Hrs = meterReading24Hrs;
			MeterReadingDay = meterReadingDay;
			MeterReadingNight = meterReadingNight;
		}

		public string AccountNumber { get; }
		public int? MeterReading24Hrs { get; }
		public int? MeterReadingDay { get; }
		public int? MeterReadingNight { get; }

		public bool Equals(ElectricityMeterReading other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(AccountNumber, other.AccountNumber) && MeterReading24Hrs == other.MeterReading24Hrs && MeterReadingDay == other.MeterReadingDay && MeterReadingNight == other.MeterReadingNight;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((ElectricityMeterReading) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (AccountNumber != null ? AccountNumber.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ MeterReading24Hrs.GetHashCode();
				hashCode = (hashCode * 397) ^ MeterReadingDay.GetHashCode();
				hashCode = (hashCode * 397) ^ MeterReadingNight.GetHashCode();
				return hashCode;
			}
		}

		public static bool operator ==(ElectricityMeterReading left, ElectricityMeterReading right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(ElectricityMeterReading left, ElectricityMeterReading right)
		{
			return !Equals(left, right);
		}
	}
}