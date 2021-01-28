using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
	public class CommsTechnicallyFeasibleValue : TypedStringValue<CommsTechnicallyFeasibleValue>
	{
		[JsonConstructor]
		private CommsTechnicallyFeasibleValue()
		{
		}

		public CommsTechnicallyFeasibleValue(string value):base(value, disableVerifyValueExists:true)
		{
		}

		public static readonly CommsTechnicallyFeasibleValue NotAvailableYet = new CommsTechnicallyFeasibleValue(null);
		public static readonly CommsTechnicallyFeasibleValue CTF1 = new CommsTechnicallyFeasibleValue("01");
		public static readonly CommsTechnicallyFeasibleValue CTF2 = new CommsTechnicallyFeasibleValue("02");
		public static readonly CommsTechnicallyFeasibleValue CTF3 = new CommsTechnicallyFeasibleValue("03");
		public static readonly CommsTechnicallyFeasibleValue CTF4 = new CommsTechnicallyFeasibleValue("04");

		public bool AllowsAllSmartFeatures()
		{
			return IsOneOf(CTF3, CTF4);
		}
	}
}