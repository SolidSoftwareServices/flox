using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Queries.Metering.Consumption;
using EI.RP.WebApp.Models.Charts;

namespace EI.RP.WebApp.Infrastructure.PresentationServices.ChartDataBuilders
{
	internal class ChartDataBuilderMonth: ChartDataBuilder
	{
		public ChartDataBuilderMonth(IDomainQueryResolver queryResolver) : base(queryResolver)
		{
		}

		public override PeriodType ForPeriodType { get; } = PeriodType.Month;

		protected override async Task<ConsumptionData> _GetChartConsumptionData(UsageChartRequest request)
		{
			var firstDayOfTheMonth = request.Date.Date.FirstDayOfTheMonth();

			var data = await GetAccountConsumptionData(
				request.AccountNumber,
				TimePeriodAggregationType.Daily,
				firstDayOfTheMonth,
				request.Date.Date.LastDayOfTheMonth(),
				ConsumptionDataRetrievalType.Smart);
			return MapData(data, request.Period, "days", firstDayOfTheMonth);
		}
	}
}