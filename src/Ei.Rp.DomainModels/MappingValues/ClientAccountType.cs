using System;
using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
	public class ClientAccountType : TypedStringValue<ClientAccountType>
	{
		
		[JsonConstructor]
		private ClientAccountType()
		{
		}
		public ClientAccountType(string value) : base(value)
		{
		}

		public static readonly ClientAccountType Gas = new ClientAccountType("Gas");
		public static readonly ClientAccountType Electricity = new ClientAccountType("Electricity");
        public static readonly ClientAccountType EnergyService = new ClientAccountType("EnergyService");

        public DivisionType ToDivisionType()
        {
	        DivisionType result;
			if (this == Electricity)
			{
				result = DivisionType.Electricity;
			}
			else if (this == Gas)
			{
				result = DivisionType.Gas;
			}
			else if (this == EnergyService)
			{
				result = DivisionType.EnergyService;
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

        public bool IsElectricity()
        {
	        return this == Electricity;
        }
        public bool IsGas()
        {
	        return this == Gas;
        }
        public bool IsEnergyServices()
        {
	        return this == EnergyService;
        }
	}
}