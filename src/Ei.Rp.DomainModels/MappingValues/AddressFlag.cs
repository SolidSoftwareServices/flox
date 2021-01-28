using System;
using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
	public class AddressFlag : TypedStringValue<AddressFlag>
	{
		[JsonConstructor]
		public AddressFlag()
		{
		}

		public AddressFlag(string value, string debuggerFriendlyDisplayValue) : base(value, debuggerFriendlyDisplayValue)
		{
		}

		public static readonly AddressFlag StandardFlag = new AddressFlag("X", nameof(StandardFlag));
		public static readonly AddressFlag None = new AddressFlag(String.Empty, nameof(None));
	}
}