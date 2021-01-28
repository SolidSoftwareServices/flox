using EI.RP.CoreServices.System;

namespace EI.RP.WebApp.Flows.AppFlows.MakeAPayment.Components.PaymentGateway
{
	public class InputModel
	{
		public string AccountNumber { get; set; }
		public EuroMoney CurrentBalanceAmount { get; set; }
	}
}
