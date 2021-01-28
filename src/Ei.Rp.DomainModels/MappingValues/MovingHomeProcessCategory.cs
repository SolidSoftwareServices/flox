using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
	public class MovingHomeProcessCategory : TypedStringValue<MovingHomeProcessCategory>
	{

		[JsonConstructor]
		private MovingHomeProcessCategory()
		{
		}

		private MovingHomeProcessCategory(string value, string debuggerFriendlyDisplayValue) : base(value, debuggerFriendlyDisplayValue)
		{
		}

		public static readonly MovingHomeProcessCategory GasMoveOut = new MovingHomeProcessCategory("IUMO", nameof(GasMoveOut));
		public static readonly MovingHomeProcessCategory GasMoveIn= new MovingHomeProcessCategory("IUMI", nameof(GasMoveIn));
	}
}