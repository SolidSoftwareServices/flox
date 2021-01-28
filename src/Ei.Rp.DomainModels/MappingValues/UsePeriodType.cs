using System;
using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
    public class UsePeriodType : TypedStringValue<UsePeriodType>
    {

        [JsonConstructor]
        private UsePeriodType()
        {
        }

        private UsePeriodType(string value, string debuggerFriendlyDisplayValue = null) : base(value, debuggerFriendlyDisplayValue)
        {
        }

        public static readonly UsePeriodType FullDay = new UsePeriodType("24H");
        public static readonly UsePeriodType Day = new UsePeriodType("00D");
        public static readonly UsePeriodType Night = new UsePeriodType("00N");

        public MeterType ToMeterType()
        {
            if (this == FullDay) return MeterType.Electricity24h;
            if (this == Day) return MeterType.ElectricityDay;
            if (this == Night) return MeterType.ElectricityNight;
            throw new NotSupportedException();
        }
    }
}