using System.Collections.Generic;

namespace EI.RP.DataModels.ResidentialPortal.SmartPlan
{
	public class GasPricingInfo
    {
		public IEnumerable<PlanUnitPrice> UnitPrices { get; set; } = new PlanUnitPrice[0];
		public PlanPrice StandingCharge { get; set; }
		public PlanPrice CarbonTax { get; set; }

		protected bool Equals(GasPricingInfo other)
		{
			return Equals(UnitPrices, other.UnitPrices) &&
			       Equals(StandingCharge, other.StandingCharge) &&
			       Equals(CarbonTax, other.CarbonTax);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((GasPricingInfo) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (UnitPrices != null ? UnitPrices.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (StandingCharge != null ? StandingCharge.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (CarbonTax != null ? CarbonTax.GetHashCode() : 0);
				return hashCode;
			}
		}
    }
}
