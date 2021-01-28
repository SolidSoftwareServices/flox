using System;

namespace Ei.Rp.DomainModels.Metering
{
    public class MeterReadingResult : IEquatable<MeterReadingResult>
    {
        public MeterReadingResult(string meterNumber, string readingResult)
        {
            MeterNumber = meterNumber;
            ReadingResult = readingResult;
        }

        public string MeterNumber { get; set; }
        public string ReadingResult { get; set; }

        public bool Equals(MeterReadingResult other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return MeterNumber == other.MeterNumber &&
                   ReadingResult == other.ReadingResult;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MeterReadingResult)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (MeterNumber != null ? MeterNumber.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ReadingResult != null ? ReadingResult.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(MeterReadingResult left, MeterReadingResult right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MeterReadingResult left, MeterReadingResult right)
        {
            return !Equals(left, right);
        }
    }
}
