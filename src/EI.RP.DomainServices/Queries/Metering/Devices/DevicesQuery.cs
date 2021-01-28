using System;
using System.Collections.Generic;
using System.Linq;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using EI.RP.CoreServices.Cqrs.Queries;

namespace EI.RP.DomainServices.Queries.Metering.Devices
{
	public class DevicesQuery : IQueryModel, IEquatable<DevicesQuery>
	{
		public QueryCacheResultsType CacheResults { get; } = QueryCacheResultsType.UserSpecific;
		public string AccountNumber { get; set; }
		public string ContractId { get; set; }

		public string DeviceId { get; set; }
		public PointReferenceNumber PremisePrn { get; set; }


        public bool IsValidQuery(out string[] notValidReasons)
		{
			var result = new List<string>();
			if (string.IsNullOrWhiteSpace(AccountNumber) && string.IsNullOrWhiteSpace(DeviceId))
			{
				result.Add($"Must specify {nameof(AccountNumber)} or {nameof(DeviceId)}");
			}

			if (!string.IsNullOrEmpty(ContractId) && string.IsNullOrEmpty(AccountNumber))
			{
				result.Add($"Must specify {nameof(AccountNumber)} with {nameof(ContractId)}");
			}

			if (PremisePrn!=null && string.IsNullOrWhiteSpace(DeviceId))
			{
				result.Add($"specify{nameof(DeviceId)} when {nameof(PremisePrn)} specified");
			}
			notValidReasons = result.ToArray();
			return !result.Any();
		}

		public bool Equals(DevicesQuery other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return AccountNumber == other.AccountNumber && 
			       DeviceId == other.DeviceId && 
			       ContractId == other.ContractId &&
			       Equals(PremisePrn, other.PremisePrn);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((DevicesQuery)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (AccountNumber != null ? AccountNumber.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (DeviceId != null ? DeviceId.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (PremisePrn != null ? PremisePrn.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (ContractId != null ? ContractId.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(DevicesQuery left, DevicesQuery right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(DevicesQuery left, DevicesQuery right)
		{
			return !Equals(left, right);
		}

		public override string ToString()
		{
			return $"{nameof(AccountNumber)}: {AccountNumber}, {nameof(DeviceId)}: {DeviceId}, {nameof(PremisePrn)}: {PremisePrn}, {nameof(ContractId)}: {ContractId}";
		}
	}
}