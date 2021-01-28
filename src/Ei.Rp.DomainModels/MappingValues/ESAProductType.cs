using System;
using System.Collections.Generic;
using System.Text;
using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
    public class ESAProductType: TypedStringValue<ESAProductType>
    {
        [JsonConstructor]
        private ESAProductType()
        {
        }
        private ESAProductType(string value) : base(value)
        {
        }

        public static readonly ESAProductType GasBoilerServices = new ESAProductType("Gas Boiler Services");
        public static readonly ESAProductType SmarterHome = new ESAProductType("Smarter Home");
        public static readonly ESAProductType SmartHeatingControls = new ESAProductType("Smart Heating Controls");
        public static readonly ESAProductType SolarPV = new ESAProductType("Solar PV");
    }
}
