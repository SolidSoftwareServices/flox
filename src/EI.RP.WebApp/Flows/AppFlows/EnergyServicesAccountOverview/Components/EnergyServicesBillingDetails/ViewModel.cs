
using System;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.UiFlows.Mvc.Components;

namespace EI.RP.WebApp.Flows.AppFlows.EnergyServicesAccountOverview.Components.EnergyServicesBillingDetails
{
	public class ViewModel : FlowComponentViewModel
	{
		public PaymentMethodType PaymentMethod { get; set; }
		public EuroMoney Amount { get; set; }
		public DateTime PaymentDate { get; set; }
		public string AccountDescription { get; set; }
	}
}