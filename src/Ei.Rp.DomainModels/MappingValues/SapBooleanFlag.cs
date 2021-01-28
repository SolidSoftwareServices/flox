using System;
using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
	public class SapBooleanFlag : TypedStringValue<SapBooleanFlag>
	{
		[JsonConstructor]
		public SapBooleanFlag()
		{
		}

		public SapBooleanFlag(string value, string debuggerFriendlyDisplayValue) : base(value, debuggerFriendlyDisplayValue)
		{
		}

		public static readonly SapBooleanFlag No = new SapBooleanFlag(string.Empty, nameof(No));
		public static readonly SapBooleanFlag Yes = new SapBooleanFlag("X", nameof(Yes));

		public bool ToBoolean()
		{
			return this == Yes;
		}

		public static implicit operator SapBooleanFlag(bool? src)
		{
			if (!src.HasValue) return null;
			;
			return src.Value 
				? Yes 
				: No;
		}
	}
}