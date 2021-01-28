using System;
using System.Collections.Generic;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.ResidentialPortal;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.SmartActivation;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;

namespace EI.RP.WebApp.Flows.AppFlows.SmartActivation.Components.SmartActivationPlanCard
{
	public class ViewModel : FlowComponentViewModel
	{
		public int ID { get; set; }

		public string PlanName { get; set; }

		public IEnumerable<PlanFeature> Features { get; set; }

		public string BonusText { get; set; }

		public IEnumerable<PlanPrice> Prices { get; set; }

		public EuroMoney EstimatedAnnualBill { get; set; }

		public EuroMoney FirstYearCostPerKWh { get; set; }

		public string EABDescription { get; set; }

		public int Position { get; set; }

		public IEnumerable<DayOfWeek> FreeDayOfElectricityChoice { get; set; }

		public UiFlowScreenModel ContainerModel { get; set; }

		public DayOfWeek? SelectedFreeDay { get; set; }

		public string FreeDayOfElectricityDescription { get; set; }

		public SmartActivationPlanFullPricing FullPricingInformation { get; set; }
		public SmartPlanGroup PlanType { get; set; }

		public class PlanFeature
		{
			public string Text { get; set; }
			public bool Highlighted { get; set; }
		}
	}
}