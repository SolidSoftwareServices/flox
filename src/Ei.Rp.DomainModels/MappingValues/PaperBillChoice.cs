using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
	public class PaperBillChoice : TypedStringValue<PaperBillChoice>
	{

		[JsonConstructor]
		private PaperBillChoice()
		{
		}
		public PaperBillChoice(string value,string debuggerFriendlyDisplayValue) : base(value,debuggerFriendlyDisplayValue)
		{
		}

		public static readonly PaperBillChoice On = new PaperBillChoice("X",nameof(On));
		public static readonly PaperBillChoice Off = new PaperBillChoice(string.Empty, nameof(Off));

		public bool ToBoolean()
		{
			return this == On;
		}
	}
}