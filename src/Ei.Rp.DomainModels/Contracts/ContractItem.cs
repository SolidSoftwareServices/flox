using System;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;

namespace Ei.Rp.DomainModels.Contracts
{
	public class ContractItem:IQueryResult, IEquatable<ContractItem>
	{
        public ProductType ProductType { get; set; }
		public string AccountID { get; set; }
        public string Description { get; set; }
        public DivisionType Division { get; set; }
        public Premise Premise { get; set; }
        public DateTime? ContractStartDate { get; set; }
		public string PointOfDeliveryGuid { get; set; }
		public DateTime? ContractEndDate { get; set; }
		public string ContractID { get; set; }
		public string BusinessAgreementID { get; set; }
		public bool IsBilledMonthly { get; set; }
		public ContractStatusType ContractStatus { get; set; }

		public bool IsActive()
		{
			return ContractStatus == ContractStatusType.Active;
		}

		public override string ToString()
		{
			return $"{nameof(ProductType)}: {ProductType}, {nameof(AccountID)}: {AccountID}, {nameof(Description)}: {Description}, {nameof(Division)}: {Division}, {nameof(Premise)}: {Premise}, {nameof(ContractStartDate)}: {ContractStartDate}, {nameof(PointOfDeliveryGuid)}: {PointOfDeliveryGuid}, {nameof(ContractEndDate)}: {ContractEndDate}, {nameof(ContractStatus)}: {ContractStatus}";
		}

		public static bool operator ==(ContractItem left, ContractItem right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(ContractItem left, ContractItem right)
		{
			return !Equals(left, right);
		}

		public bool Equals(ContractItem other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return ProductType == other.ProductType && AccountID == other.AccountID && Description == other.Description && Equals(Division, other.Division) && Equals(Premise, other.Premise) && Nullable.Equals(ContractStartDate, other.ContractStartDate) &&
				PointOfDeliveryGuid == other.PointOfDeliveryGuid && Nullable.Equals(ContractEndDate, other.ContractEndDate) && IsBilledMonthly == other.IsBilledMonthly &&
				ContractID == other.ContractID && BusinessAgreementID == other.BusinessAgreementID && ContractStatus == other.ContractStatus;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((ContractItem) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (ProductType != null ? ProductType.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (AccountID != null ? AccountID.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Division != null ? Division.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Premise != null ? Premise.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ ContractStartDate.GetHashCode();
				hashCode = (hashCode * 397) ^ (PointOfDeliveryGuid != null ? PointOfDeliveryGuid.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ ContractEndDate.GetHashCode();
				hashCode = (hashCode * 397) ^ IsBilledMonthly.GetHashCode();
				hashCode = (hashCode * 397) ^ (ContractID != null ? ContractID.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (BusinessAgreementID != null ? BusinessAgreementID.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (ContractStatus != null ? ContractStatus.GetHashCode() : 0);
				return hashCode;
			}
		}
	}
}