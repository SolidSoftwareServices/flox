using Ei.Rp.DomainModels.MappingValues;
using EI.RP.UiFlows.Mvc.Components;


namespace EI.RP.WebApp.Flows.AppFlows.MakeAPayment.Components.PaymentMessages
{
	public class ViewModel : FlowComponentViewModel
	{
		public ClientAccountType AccountType { get; set; }
		public PaymentMethodType PaymentMethod { get; set; }
	}
}