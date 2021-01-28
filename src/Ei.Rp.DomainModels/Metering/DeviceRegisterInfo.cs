using System;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.MappingValues;


namespace Ei.Rp.DomainModels.Metering
{
	public class DeviceRegisterInfo:IQueryResult, IEquatable<DeviceRegisterInfo>
	{
		public MeterType MeterType { get; set; }
		public string MeterNumber { get; set; }

		public MeterUnit MeterUnit { get; set; }

		public MeterReadingRegisterType RegisterId { get; set; }
		public string DeviceId { get; set; }


		public bool Equals(DeviceRegisterInfo other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(MeterType, other.MeterType) && string.Equals(MeterNumber, other.MeterNumber) && Equals(MeterUnit, other.MeterUnit) && Equals(RegisterId, other.RegisterId) && string.Equals(DeviceId, other.DeviceId);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((DeviceRegisterInfo) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (MeterType != null ? MeterType.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (MeterNumber != null ? MeterNumber.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (MeterUnit != null ? MeterUnit.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (RegisterId != null ? RegisterId.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (DeviceId != null ? DeviceId.GetHashCode() : 0);
				return hashCode;
			}
		}

	}
}