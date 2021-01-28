using System;
using System.Linq;
using System.Collections.Generic;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DataModels.ResidentialPortal;

namespace Ei.Rp.DomainModels.SmartActivation
{
	public class SmartPlan : IQueryResult
	{
		public int ID { get; set; }
		public bool IsActive { get; set; }
		public SmartPlanGroup PlanType { get; set; }
		public int OrderIndex { get; set; }
		public string PlanName { get; set; }
		public string BonusDescription { get; set; }
		public IEnumerable<string> PlanFeatures { get; set; } = new string[0];
		public IEnumerable<PlanPrice> PlanPrices { get; set; } = new PlanPrice[0];		
		public EuroMoney EstimatedAnnualBill { get; set; }
		public string EABDescription { get; set; }
		public EuroMoney FirstYearCostPerKwh { get; set; }
		public IEnumerable<DayOfWeek> FreeDayOfElectricityChoice { get; set; } = new DayOfWeek[0];
		public string FreeDayOfElectricityDescription { get; set; }
		public string GeneralTermsAndConditionsUrl { get; set; }
		public string PricePlanTermsAndConditionsUrl { get; set; }
		public IEnumerable<SmartActivationProductProposal> ValidProductProposals { get; set; } = new SmartActivationProductProposal[0];
		public SmartActivationPlanFullPricing FullPricingInformation { get; set; }

		public bool Equals(SmartPlan other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return ID == other.ID && IsActive == other.IsActive &&
				PlanType.Equals(other.PlanType) &&
				OrderIndex == other.OrderIndex && 
				PlanName == other.PlanName && 
				BonusDescription == other.BonusDescription &&
				PlanFeatures.SequenceEqual(other.PlanFeatures) && 
				PlanPrices.SequenceEqual(other.PlanPrices) && 
				EstimatedAnnualBill == other.EstimatedAnnualBill &&
				EABDescription == other.EABDescription && 
				FirstYearCostPerKwh == other.FirstYearCostPerKwh && 
				FreeDayOfElectricityChoice.SequenceEqual(other.FreeDayOfElectricityChoice) &&
				FreeDayOfElectricityDescription == other.FreeDayOfElectricityDescription && 
				GeneralTermsAndConditionsUrl == other.GeneralTermsAndConditionsUrl && 
				PricePlanTermsAndConditionsUrl == other.PricePlanTermsAndConditionsUrl &&
				ValidProductProposals.SequenceEqual(other.ValidProductProposals);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((SmartPlan)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = ID.GetHashCode();
				hashCode = (hashCode * 397) ^ IsActive.GetHashCode();
				hashCode = (hashCode * 397) ^ (PlanType != null ? PlanType.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ OrderIndex.GetHashCode();
				hashCode = (hashCode * 397) ^ (PlanName != null ? PlanName.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (BonusDescription != null ? BonusDescription.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (PlanFeatures != null ? PlanFeatures.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (PlanPrices != null ? PlanPrices.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (EstimatedAnnualBill != null ? EstimatedAnnualBill.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (EABDescription != null ? EABDescription.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (FirstYearCostPerKwh != null ? FirstYearCostPerKwh.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (FreeDayOfElectricityChoice != null ? FreeDayOfElectricityChoice.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (FreeDayOfElectricityDescription != null ? FreeDayOfElectricityDescription.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (GeneralTermsAndConditionsUrl != null ? GeneralTermsAndConditionsUrl.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (PricePlanTermsAndConditionsUrl != null ? PricePlanTermsAndConditionsUrl.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (ValidProductProposals != null ? ValidProductProposals.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(SmartPlan left, SmartPlan right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(SmartPlan left, SmartPlan right)
		{
			return !Equals(left, right);
		}
	}
}