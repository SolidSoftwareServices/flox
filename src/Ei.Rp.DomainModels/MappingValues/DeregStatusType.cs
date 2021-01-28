using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
	public class DeregStatusType : TypedStringValue<DeregStatusType>
	{
		[JsonConstructor]
		private DeregStatusType()
		{
		}

		private DeregStatusType(string value, string debuggerFriendlyDisplayValue = null) : base(value, debuggerFriendlyDisplayValue, true)
		{
		}

		public static readonly DeregStatusType Registered = new DeregStatusType("Z1", nameof(Registered));
		public static readonly DeregStatusType Deregistered = new DeregStatusType("Z2", nameof(Deregistered));
		public static readonly DeregStatusType TariffExemption = new DeregStatusType("Z3", nameof(TariffExemption));
	}
}
