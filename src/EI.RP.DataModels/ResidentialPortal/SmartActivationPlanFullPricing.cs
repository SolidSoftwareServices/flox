using EI.RP.DataModels.ResidentialPortal.SmartPlan;

namespace EI.RP.DataModels.ResidentialPortal
{
	public class SmartActivationPlanFullPricing
    {
	    public ElectricityPricingInfo ElectricityPricingInfo { get; set; }
	    public GasPricingInfo GasPricingInfo { get; set; }
	    public string EstimatedAnnualBillType { get; set; }
	    public PlanPrice EstimatedAnnualBillForGas { get; set; }
		public PlanPrice EstimatedUrbanAnnualBill { get; set; }
		public PlanPrice EstimatedRuralAnnualBill { get; set; }
		public string PricesValidityMessage { get; set; }
		public string LowCostPricingMessage { get; set; }
		public string EstimatedAnnualBillMessage { get; set; }

		protected bool Equals(SmartActivationPlanFullPricing other)
		{
			return Equals(ElectricityPricingInfo, other.ElectricityPricingInfo) &&
			       Equals(GasPricingInfo, other.GasPricingInfo) &&
			       string.Equals(EstimatedAnnualBillType, other.EstimatedAnnualBillType) &&
			       Equals(EstimatedAnnualBillForGas, other.EstimatedAnnualBillForGas) &&
			       Equals(EstimatedUrbanAnnualBill, other.EstimatedUrbanAnnualBill) &&
			       Equals(EstimatedRuralAnnualBill, other.EstimatedRuralAnnualBill) &&
			       string.Equals(PricesValidityMessage, other.PricesValidityMessage) &&
			       string.Equals(LowCostPricingMessage, other.LowCostPricingMessage) &&
			       string.Equals(EstimatedAnnualBillMessage, other.EstimatedAnnualBillMessage);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((SmartActivationPlanFullPricing) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (ElectricityPricingInfo != null ? ElectricityPricingInfo.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (GasPricingInfo != null ? GasPricingInfo.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (EstimatedAnnualBillType != null ? EstimatedAnnualBillType.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (EstimatedAnnualBillForGas != null ? EstimatedAnnualBillForGas.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (EstimatedUrbanAnnualBill != null ? EstimatedUrbanAnnualBill.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (EstimatedRuralAnnualBill != null ? EstimatedRuralAnnualBill.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (PricesValidityMessage != null ? PricesValidityMessage.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (LowCostPricingMessage != null ? LowCostPricingMessage.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (EstimatedAnnualBillMessage != null ? EstimatedAnnualBillMessage.GetHashCode() : 0);
				return hashCode;
			}
		}
    }
}
