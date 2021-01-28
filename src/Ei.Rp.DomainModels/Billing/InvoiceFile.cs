using EI.RP.CoreServices.Cqrs.Queries;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.Billing
{
    [JsonObject(MemberSerialization.Fields)]
	public class InvoiceFile : IQueryResult
	{
		private readonly string _invoiceNumber;

		public InvoiceFile(string invoiceNumber)
		{
			_invoiceNumber = invoiceNumber;
		}

		public byte[] FileData { get; set; }

		public string GetFileName()
		{
			return $"Bill_{_invoiceNumber}.pdf";
		}
	}
}