using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
	public class InvoiceStatus : TypedStringValue<InvoiceStatus>
	{

		[JsonConstructor]
		private InvoiceStatus()
		{
		}
		public InvoiceStatus(string value, string debuggerFriendlyDisplayValue) : base(value, debuggerFriendlyDisplayValue)
		{
		}

		public static readonly InvoiceStatus Open = new InvoiceStatus("1", nameof(Open));

		public static readonly InvoiceStatus DebitMemoOrCardReleasedByCustomer = new InvoiceStatus("2", nameof(DebitMemoOrCardReleasedByCustomer));
		public static readonly InvoiceStatus DebitMemoOrCardReleasedByCustomerFCC = new InvoiceStatus("2F", nameof(DebitMemoOrCardReleasedByCustomerFCC));
		public static readonly InvoiceStatus DebitMemoOrCardReleasedByCustomerWebChannel = new InvoiceStatus("4F", nameof(DebitMemoOrCardReleasedByCustomerWebChannel));
		public static readonly InvoiceStatus AutomaticCollection = new InvoiceStatus("3", nameof(AutomaticCollection));
		public static readonly InvoiceStatus CheckOrTransferNotifiedByCustomer = new InvoiceStatus("4", nameof(CheckOrTransferNotifiedByCustomer));
		public static readonly InvoiceStatus CheckOrTransferNotifiedByCustomerFCC = new InvoiceStatus("4F", nameof(CheckOrTransferNotifiedByCustomerFCC));
		public static readonly InvoiceStatus CannotBePaid = new InvoiceStatus("8", nameof(CannotBePaid));
		public static readonly InvoiceStatus Paid = new InvoiceStatus("9", nameof(Paid));

		public static readonly InvoiceStatus PromisedToPayExists = new InvoiceStatus("P", nameof(PromisedToPayExists));
		public static readonly InvoiceStatus PaymentSpecificationExists = new InvoiceStatus("I", nameof(PaymentSpecificationExists));

	}
}