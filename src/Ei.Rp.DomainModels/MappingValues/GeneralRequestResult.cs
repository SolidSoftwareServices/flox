using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
	public class GeneralRequestResult : TypedStringValue<GeneralRequestResult>
	{

		[JsonConstructor]
		private GeneralRequestResult()
		{
		}

		public GeneralRequestResult(string value, string debuggerFriendlyDisplayValue) : base(value,
			debuggerFriendlyDisplayValue)
		{
		}

		public static readonly GeneralRequestResult Success = new GeneralRequestResult("S", nameof(Success));
		public static readonly GeneralRequestResult Warning = new GeneralRequestResult("W", nameof(Warning));
		public static readonly GeneralRequestResult Error = new GeneralRequestResult("E", nameof(Error));
	}
}