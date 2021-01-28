using System;
using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.MappingValues
{
	public class TimePeriodAggregationType : TypedStringValue<TimePeriodAggregationType>
	{
		[JsonProperty]
		public TimeSpan? Timespan { get; private set; }

		[JsonConstructor]
		private TimePeriodAggregationType()
		{
		}
		private TimePeriodAggregationType(string value,string debuggerText,TimeSpan? timespan=null) : base(value,debuggerText)
		{
			Timespan = timespan;
		}

		public static readonly TimePeriodAggregationType HalfHourly = new TimePeriodAggregationType("30","HalfHourly",TimeSpan.FromMinutes(30));
		public static readonly TimePeriodAggregationType Hourly = new TimePeriodAggregationType("60","Hourly", TimeSpan.FromHours(1));
		public static readonly TimePeriodAggregationType Daily = new TimePeriodAggregationType("DAY","Daily",TimeSpan.FromDays(1));
		public static readonly TimePeriodAggregationType Monthly = new TimePeriodAggregationType("MON","Monthly");

		public static readonly TimePeriodAggregationType BiMonthly = new TimePeriodAggregationType("NSB","Bimonthly");
	}
}