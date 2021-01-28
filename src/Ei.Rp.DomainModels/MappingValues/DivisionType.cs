using System;
using System.Linq;
using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
	public class DivisionType : TypedStringValue<DivisionType>
	{
	
		[JsonConstructor]
		private DivisionType()
		{
		}
		private DivisionType(string value, string debuggerFriendlyDisplayValue) : base(value,debuggerFriendlyDisplayValue, true)
		{
		}

		public static readonly DivisionType Electricity = new DivisionType("01", nameof(Electricity));
		public static readonly DivisionType Gas = new DivisionType("02", nameof(Gas));
        public static readonly DivisionType EnergyService = new DivisionType("07", nameof(EnergyService));

        public ClientAccountType ToAccountType()
		{
			ClientAccountType result;
			if (this == Electricity)
			{
				result= ClientAccountType.Electricity;
			}
			else if (this == Gas)
			{
				result= ClientAccountType.Gas;
			}
            else if (this == EnergyService)
            {
                result = ClientAccountType.EnergyService;
            }
            else if (this.ValueId == null)
			{
				result = null;
			}
			else
			{
				throw new NotSupportedException();
			}

			return result;
		}
	}
}