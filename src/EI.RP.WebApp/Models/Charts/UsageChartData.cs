using System;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Queries.Metering.Consumption;

namespace EI.RP.WebApp.Models.Charts
{
	public class UsageChartData
	{
		public DateTime FromDate { get; set; }
		public DateTime ToDate { get; set; }
		public string IntervalName { get; set; }
		public TimePeriodAggregationType ConsumptionAggregationType { get; set; }

		public ConsumptionDataRetrievalType RetrievalType { get; set; }
	}
}