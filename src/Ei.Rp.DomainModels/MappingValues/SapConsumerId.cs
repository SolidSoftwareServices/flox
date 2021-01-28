using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
	public class SapConsumerId : TypedStringValue<SapConsumerId>
	{

		[JsonConstructor]
		private SapConsumerId()
		{
		}

		private SapConsumerId(string value, string debuggerFriendlyDisplayValue) : base(value,
			debuggerFriendlyDisplayValue)
		{
		}

		public static readonly SapConsumerId AddGasConsumer = new SapConsumerId("ROI_RES_PORTAL_ADDGAS", nameof(AddGasConsumer));
		public static readonly SapConsumerId MoveHomeConsumer = new SapConsumerId("ROI_RES_PORTAL_HOMEMOVE", nameof(MoveHomeConsumer));
		public static readonly SapConsumerId ExistingCustomer = new SapConsumerId("EXISTCUST", nameof(ExistingCustomer));
		public static readonly SapConsumerId ActivateSmart = new SapConsumerId("ROI_RES_PORTAL_SMART", nameof(ActivateSmart));

	}
}