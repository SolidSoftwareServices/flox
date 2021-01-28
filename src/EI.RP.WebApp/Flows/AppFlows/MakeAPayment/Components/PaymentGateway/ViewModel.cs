
using EI.RP.CoreServices.System;
using EI.RP.UiFlows.Mvc.Components;

namespace EI.RP.WebApp.Flows.AppFlows.MakeAPayment.Components.PaymentGateway
{
	public class ViewModel : FlowComponentViewModel
	{
        public EuroMoney CurrentBalanceAmount { get; set; }
        public bool IsLowBalance { get; set; }
        
        public string Account { get; set; }
        public string ContractAccountNumber { get; set; }
        public string BusinessPartner { get; set; }
        public int Amount { get; set; }
        public string MerchantId { get; set; }
        public string OrderId { get; set; }
        public string ProductId { get; set; }
        public string Currency { get; set; }
        public string TimeStamp { get; set; }
        public string SHA1HashValue { get; set; }
        public bool AutoSettle { get; set; }
        public string RequestUrl { get; set; }
        public string ResponseUrl { get; set; }
        public string CookieJson { get; set; }
        public string CSRF { get; set; }
        public bool PayerExists { get; set; }
        public string PayerRef { get; set; }
        public string UserName { get; set; }
        public string ResponseDomain { get; set; }
    }
}