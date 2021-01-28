using System.Linq;
using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
	public class SmartPlanPriceType : TypedStringValue<SmartPlanPriceType>
	{
		[JsonConstructor]
		public SmartPlanPriceType()
		{
		}

		public SmartPlanPriceType(string value) : base(value)
		{
		}

		public static readonly SmartPlanPriceType Electricity24H = new SmartPlanPriceType(nameof(Electricity24H));
		public static readonly SmartPlanPriceType Gas24H = new SmartPlanPriceType(nameof(Gas24H ));
		public static readonly SmartPlanPriceType ElectricityDay = new SmartPlanPriceType(nameof(ElectricityDay));
		public static readonly SmartPlanPriceType ElectricityNight= new SmartPlanPriceType(nameof(ElectricityNight));
		public static readonly SmartPlanPriceType ElectricityBoost= new SmartPlanPriceType(nameof(ElectricityBoost));
		public static readonly SmartPlanPriceType ElectricityPeak= new SmartPlanPriceType(nameof(ElectricityPeak));
		
	}
}