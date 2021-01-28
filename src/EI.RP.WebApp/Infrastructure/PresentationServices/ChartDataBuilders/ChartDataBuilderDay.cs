using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Queries.Metering.Consumption;
using EI.RP.WebApp.Models.Charts;

namespace EI.RP.WebApp.Infrastructure.PresentationServices.ChartDataBuilders
{
	internal class ChartDataBuilderDay : ChartDataBuilder
	{
		public ChartDataBuilderDay(IDomainQueryResolver queryResolver) : base(queryResolver)
		{
		}

		public override PeriodType ForPeriodType { get; } = PeriodType.Day;

		protected override async Task<ConsumptionData> _GetChartConsumptionData(UsageChartRequest request)
		{
			var from = request.Date.Date;
			var data = await GetAccountConsumptionData(
				request.AccountNumber,
				TimePeriodAggregationType.Hourly,
				from,
				from.LastTimeOfTheDay(),
				ConsumptionDataRetrievalType.Smart);
			return MapData(data, request.Period, "hours", from);
		}
	}
}