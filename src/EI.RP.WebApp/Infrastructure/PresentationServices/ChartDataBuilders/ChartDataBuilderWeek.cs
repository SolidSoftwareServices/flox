using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Queries.Metering.Consumption;
using EI.RP.WebApp.Models.Charts;

namespace EI.RP.WebApp.Infrastructure.PresentationServices.ChartDataBuilders
{
	internal class ChartDataBuilderWeek : ChartDataBuilder
	{
		public ChartDataBuilderWeek(IDomainQueryResolver queryResolver) : base(queryResolver)
		{
		}

		public override PeriodType ForPeriodType { get; } = PeriodType.Week;

		protected override async Task<ConsumptionData> _GetChartConsumptionData(UsageChartRequest request)
		{
			var from = request.Date.Date.FirstDayOfWeek(DayOfWeek.Monday);

			var data = await GetAccountConsumptionData(
				request.AccountNumber,
				TimePeriodAggregationType.Daily,
				from,
				from.LastDayOfWeek(DayOfWeek.Monday).AddDays(1).Subtract(TimeSpan.FromMilliseconds(1)),
				ConsumptionDataRetrievalType.Smart);
			return MapData(data, request.Period, "days", from);
		}
	}
}