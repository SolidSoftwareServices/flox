using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
	public class AccountActivityType : TypedStringValue<AccountActivityType>
	{
		[JsonConstructor]
		private AccountActivityType()
		{
		}
		private AccountActivityType(string value) : base(value)
		{
		}

		public static readonly AccountActivityType Invoice = new AccountActivityType("Invoice");
		public static readonly AccountActivityType Payment = new AccountActivityType("Payment");

	}
}