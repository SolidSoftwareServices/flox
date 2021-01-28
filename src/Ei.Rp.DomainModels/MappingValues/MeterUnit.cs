using EI.RP.CoreServices.System;
using Newtonsoft.Json;


namespace Ei.Rp.DomainModels.MappingValues
{
    public class MeterUnit : TypedStringValue<MeterUnit>
    {
      
        [JsonConstructor]
        private MeterUnit()
        {
        }

        private MeterUnit(string value,string debuggerFriendlyDisplayValue=null) : base(value,debuggerFriendlyDisplayValue,true)
        {
        }

        public static readonly MeterUnit M3 = new MeterUnit("M3", "m³");
        public static readonly MeterUnit KWH = new MeterUnit("KWH", "kWh");
        public static readonly MeterUnit VAL = new MeterUnit("VAL");
        public static readonly MeterUnit KW = new MeterUnit("KW");
		public static readonly MeterUnit KRH = new MeterUnit("KRH");
	}
}
