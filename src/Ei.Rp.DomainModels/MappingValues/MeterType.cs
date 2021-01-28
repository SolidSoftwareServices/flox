using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
    public class MeterType : TypedStringValue<MeterType>
    {
       
        [JsonConstructor]
        private MeterType()
        {
        }
        private MeterType(string value) : base(value, disableVerifyValueExists: true)
        {
        }

        public static readonly MeterType ElectricityNight = new MeterType("Night");
        public static readonly MeterType ElectricityDay = new MeterType("Day");
        public static readonly MeterType Electricity24h = new MeterType("24Hr");
        public static readonly MeterType ElectricityNightStorageHeater = new MeterType("NSH");
		public static readonly MeterType Gas = new MeterType("Gas");


		public static readonly MeterType GasVolume = new MeterType("Gas Volume");
		public static readonly MeterType GasConversionFactor = new MeterType("Gas Conversion Factor");
		public static readonly MeterType GasConsumption = new MeterType("Gas Consumption");
		public static readonly MeterType GasPeakDayCapacity = new MeterType("Gas Peak Day Capacity");

		
		public static readonly MeterType Wattless = new MeterType("Wattless");
		public static readonly MeterType MDNormal = new MeterType("MD Normal");
		public static readonly MeterType MDPeak = new MeterType("MD Peak");
		public static readonly MeterType CumulativeNormal = new MeterType("Cumulative Normal");
		public static readonly MeterType CumulativePeak = new MeterType("Cumulative Peak");
		public static readonly MeterType ImportkW = new MeterType("Import kW");
		public static readonly MeterType ImportkVAr = new MeterType("Import kVAr");
		public static readonly MeterType ExportkVAr = new MeterType("Export kVAr");

		public static readonly MeterType ExportkW = new MeterType("Export kW");

		public static readonly MeterType UnMetered = new MeterType("Unmetered");
		public static readonly MeterType DayOffPeak=new MeterType("Day Off Peak");
		public static readonly MeterType NightOffPeak = new MeterType("Night Off Peak");
		public static readonly MeterType Peak = new MeterType("Peak");
		public bool IsGas()
		{
			return IsOneOf(Gas, GasVolume, GasConversionFactor, GasConsumption, GasPeakDayCapacity);
		}

		public bool IsElectricity()
		{
			return this.IsOneOf(Electricity24h, ElectricityDay, ElectricityNight, ElectricityNightStorageHeater,DayOffPeak, NightOffPeak,Peak);
		}
    }

}
