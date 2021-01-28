using System.Collections.Generic;

namespace EI.RP.DataModels.ResidentialPortal.SmartPlan
{
	public class ElectricityPricingInfo
    {
		public IEnumerable<PlanUnitPrice> UnitPrices { get; set; } = new PlanUnitPrice[0];
		public IEnumerable<PlanCharge> StandingCharges { get; set; } = new PlanCharge[0];
		public PlanPrice ElectricityPublicServiceObligationLevy { get; set; }

		protected bool Equals(ElectricityPricingInfo other)
		{
			return Equals(UnitPrices, other.UnitPrices) &&
			       Equals(StandingCharges, other.StandingCharges) &&
			       Equals(ElectricityPublicServiceObligationLevy, other.ElectricityPublicServiceObligationLevy);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((ElectricityPricingInfo) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (UnitPrices != null ? UnitPrices.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (StandingCharges != null ? StandingCharges.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (ElectricityPublicServiceObligationLevy != null ? ElectricityPublicServiceObligationLevy.GetHashCode() : 0);
				return hashCode;
			}
		}
    }
}
