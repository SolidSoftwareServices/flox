namespace EI.RP.DataModels.ResidentialPortal.SmartPlan
{
	public class PlanUnitPrice
    {
	    public string Description { get; set; }
		public PlanPrice StandardPrice { get; set; }
		public PlanPrice EffectivePrice { get; set; }
		public float EffectiveDirectDebitDiscountPercentage { get; set; }


		protected bool Equals(PlanUnitPrice other)
		{
			return string.Equals(Description,other.Description) &&
			       Equals(StandardPrice,other.StandardPrice) &&
			       Equals(EffectivePrice,other.EffectivePrice) &&
			       EffectiveDirectDebitDiscountPercentage.Equals(other.EffectiveDirectDebitDiscountPercentage);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((PlanUnitPrice)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (Description != null ? Description.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (StandardPrice != null ? StandardPrice.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (EffectivePrice != null ? EffectivePrice.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ EffectiveDirectDebitDiscountPercentage.GetHashCode();
				return hashCode;
			}
		}
	}
}
