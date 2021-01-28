using EI.RP.CoreServices.Ports.OData;

namespace EI.RP.DataModels.Switch
{
	public class DiscountDto
	{
		/// <summary>
		/// Old paperflag
		/// </summary>
		public bool IsOnlineBilling { get; set; }
		/// <summary>
		/// old DirectDebitFlag
		/// </summary>
		public bool IsDirectDebit { get; set; }
		/// <summary>
		/// old AddGasAccount 
		/// </summary>
		public bool IsElectricityAndGas{ get; set; }
		/// <summary>
		/// old Acc
		/// </summary>
		public string ExistingCustomerPricePlanId { get; set; }
		public string DiscountPercent { get; set; }
	}
}