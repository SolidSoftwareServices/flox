using System;

namespace EI.RP.DataModels.Sap.ErpUmc.Dtos
{
	public partial class InvoiceDto
	{
		public bool IsBill()
		{
			return InvoiceText != null && InvoiceText.Equals("bill", StringComparison.InvariantCultureIgnoreCase);
		}
	}
}