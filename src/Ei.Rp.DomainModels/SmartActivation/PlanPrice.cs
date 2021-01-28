using Ei.Rp.DomainModels.MappingValues;
using EI.RP.CoreServices.System;

namespace Ei.Rp.DomainModels.SmartActivation
{
	public class PlanPrice
	{
		public SmartPlanPriceType Type { get; set; }
		public EuroMoney Value { get; set; }

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((PlanPrice)obj);
		}

		public bool Equals(PlanPrice other)
		{
			return Value.Equals(other.Value) && Type.Equals(other.Type);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (Type != null ? Type.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Value != null ? Value.GetHashCode() : 0);
				return hashCode;
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
