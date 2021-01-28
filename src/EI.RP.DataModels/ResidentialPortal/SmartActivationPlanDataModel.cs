using System.Collections.Generic;
using EI.RP.CoreServices.System;

namespace EI.RP.DataModels.ResidentialPortal
{

	public class SmartActivationPlanDataModel
	{
		public int ID { get; set; }
		public bool IsAvailable { get; set; }
		public string GroupName { get; set; }
		public int OrderIndex { get; set; }
		public string PlanName { get; set; }
		public string BonusDescription { get; set; }
		public IEnumerable<string> PlanFeatures { get; set; } =new string[0];
		public EuroMoney PriceElectricity24H { get; set; }
		public EuroMoney PriceGas24H { get; set; }
		public EuroMoney PriceDay { get; set; }
		public EuroMoney PriceNight { get; set; }
		public EuroMoney PriceBoost { get; set; }
		
		public EuroMoney FirstYearCostPerKwh { get; set; }
		public EuroMoney EstimatedAnnualBill { get; set; }

		public string EABDescription { get; set; }

		public EuroMoney PricePeak { get; set; }

		public string[] FreeDayOfElectricityChoice { get; set; } = new string[0];
		public string FreeDayOfElectricityDescription { get; set; }

		public string GeneralTermsAndConditionsUrl { get; set; }
		public string PricePlanTermsAndConditionsUrl { get; set; }

		public IEnumerable<SmartActivationProductProposal> ValidProductProposals { get; set; } = new SmartActivationProductProposal[0];
		public SmartActivationPlanFullPricing FullPricingInformation { get; set; }

	}
}