using EI.RP.CoreServices.System;

namespace EI.RP.DataModels.ResidentialPortal.SmartPlan
{
	public class PlanPrice
    {
	    public EuroMoney ExcludingVat { get; set; }
		public EuroMoney IncludingVat { get; set; }

		protected bool Equals(PlanPrice other)
		{
			return ExcludingVat == other.ExcludingVat 
			       && IncludingVat == other.IncludingVat;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((PlanPrice) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (ExcludingVat.GetHashCode() * 397) ^ IncludingVat.GetHashCode();
			}
		}

		public static bool operator ==(PlanPrice left, PlanPrice right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(PlanPrice left, PlanPrice right)
		{
			return !Equals(left, right);
		}
	}
}
