using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
	public class AddressType : TypedStringValue<AccountActivityType>
	{
		[JsonConstructor]
		private AddressType()
		{
		}
		private AddressType(string value) : base(value)
		{
		}

		public static readonly AddressType RepublicOfIreland = new AddressType("ROI");
		public static readonly AddressType NI = new AddressType("NI");
		public static readonly AddressType PO = new AddressType("PO");

	}
}