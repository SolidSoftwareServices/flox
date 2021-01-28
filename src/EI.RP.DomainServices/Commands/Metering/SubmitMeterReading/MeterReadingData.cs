using System;

namespace EI.RP.DomainServices.Commands.Metering.SubmitMeterReading
{
	public class MeterReadingData : IEquatable<MeterReadingData>
	{
		public string DeviceId { get; set; }
		public string MeterReading { get; set;  }
		public string RegisterId { get; set; }
		public string MeterNumber { get; set; }
		public string MeterTypeName { get; set; }
		public string Lcpe { get; set; }
		public string MeterReadingReasonID { get; set; }
		public DateTime ReadingDateTime { get; set; } = DateTime.Today;
		public bool? FmoRequired { get; set; }  = null;
	
		public bool Equals(MeterReadingData other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return DeviceId == other.DeviceId && MeterReading == other.MeterReading && RegisterId == other.RegisterId && MeterNumber == other.MeterNumber && 
			       other.MeterTypeName == MeterTypeName && other.Lcpe == Lcpe && other.MeterReadingReasonID == MeterReadingReasonID &&
				   Equals(ReadingDateTime, other.ReadingDateTime) && Equals(FmoRequired, other.FmoRequired);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((MeterReadingData)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (DeviceId != null ? DeviceId.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (MeterReading != null ? MeterReading.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (RegisterId != null ? RegisterId.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (MeterNumber != null ? MeterNumber.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (MeterTypeName != null ? MeterTypeName.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Lcpe != null ? Lcpe.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (MeterReadingReasonID != null ? MeterReadingReasonID.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (ReadingDateTime != null ? ReadingDateTime.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (FmoRequired != null ? FmoRequired.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(MeterReadingData left, MeterReadingData right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(MeterReadingData left, MeterReadingData right)
		{
			return !Equals(left, right);
		}
	}
}
