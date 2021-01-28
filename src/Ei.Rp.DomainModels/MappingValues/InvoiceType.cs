using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
	public class InvoiceType : TypedStringValue<InvoiceType>
	{
		

		[JsonConstructor]
		private InvoiceType()
		{
		}

		private InvoiceType(string value) : base(value)
		{
		}

		public static readonly InvoiceType Bill = new InvoiceType("Bill");

	}
}