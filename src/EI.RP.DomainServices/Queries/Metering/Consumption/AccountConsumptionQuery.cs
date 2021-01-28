using System;
using System.Collections.Generic;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.MappingValues;

namespace EI.RP.DomainServices.Queries.Metering.Consumption
{
	public enum ConsumptionDataRetrievalType
	{
		NotSpecified,
		Smart,
		NonSmart,

	}

	public partial class AccountConsumptionQuery : IQueryModel
	{
	
		public QueryCacheResultsType CacheResults { get; } = QueryCacheResultsType.UserSpecific;

		public string AccountNumber { get; set; }

		public DateTimeRange Range { get; set; } = new DateTimeRange(DateTime.Today.AddYears(-2),DateTime.Today);

		public TimePeriodAggregationType AggregationType { get; set; }=TimePeriodAggregationType.Monthly;
		public ConsumptionDataRetrievalType RetrievalType { get; set; } = ConsumptionDataRetrievalType.NotSpecified;
		/// <summary>
		/// When true the result will contain zeroes in the moments where no information was provided
		/// </summary>
		public bool FillResultWithZeroes { get; set; } = false;

		public bool IsValidQuery(out string[] notValidReasons)
		{
			var reasons=new List<string>();
			bool result = true;
			if (RetrievalType == ConsumptionDataRetrievalType.NotSpecified)
			{
				reasons.Add($"{nameof(RetrievalType)} wasn't specified.");
				result = false;
			}
			if (AggregationType == null)
			{
				reasons.Add($"{nameof(AggregationType)} cannot be null");
				result = false;
			}

		
			notValidReasons = reasons.ToArray();
			return result;
		}

	}
}