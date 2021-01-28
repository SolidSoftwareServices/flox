namespace EI.RP.DataModels.ResidentialPortal.SmartPlan
{
	public class PlanCharge
    {
		public string ChargeDescription { get; set; }
	    public PlanPrice StandingCharge24H { get; set; }
	    public PlanPrice LowUsageStandingCharge { get; set; }

	    protected bool Equals(PlanCharge other)
	    {
		    return string.Equals(ChargeDescription, other.ChargeDescription) &&
		           Equals(StandingCharge24H, other.StandingCharge24H) &&
		           Equals(LowUsageStandingCharge, other.LowUsageStandingCharge);
	    }

	    public override bool Equals(object obj)
	    {
		    if (ReferenceEquals(null, obj)) return false;
		    if (ReferenceEquals(this, obj)) return true;
		    if (obj.GetType() != this.GetType()) return false;
		    return Equals((PlanCharge)obj);
	    }

	    public override int GetHashCode()
	    {
		    unchecked
		    {
			    var hashCode = (ChargeDescription != null ? ChargeDescription.GetHashCode() : 0);
			    hashCode = (hashCode * 397) ^ (StandingCharge24H != null ? StandingCharge24H.GetHashCode() : 0);
			    hashCode = (hashCode * 397) ^ (LowUsageStandingCharge != null ? LowUsageStandingCharge.GetHashCode() : 0);
			    return hashCode;
		    }
	    }
	}
}
