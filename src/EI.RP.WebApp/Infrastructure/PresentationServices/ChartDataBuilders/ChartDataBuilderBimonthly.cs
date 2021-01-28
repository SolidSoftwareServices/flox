using System;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering.Consumption;
using EI.RP.DomainServices.Queries.Metering.Consumption;
using EI.RP.WebApp.Models.Charts;

namespace EI.RP.WebApp.Infrastructure.PresentationServices.ChartDataBuilders
{
	internal class ChartDataBuilderBimonthly : ChartDataBuilder
	{
		public ChartDataBuilderBimonthly(IDomainQueryResolver queryResolver) : base(queryResolver)
		{
		}

		public override PeriodType ForPeriodType { get; } = PeriodType.Bimonthly;

		protected override async Task<ConsumptionData> _GetChartConsumptionData(UsageChartRequest request)
		{
			var firstDayOfTheYear = request.Date.FirstDayOfTheYear();
			var lastTimeOfTheYear = request.Date.LastTimeOfTheYear();
			var nonSmartData = await GetAccountConsumptionData(
				request.AccountNumber,
				TimePeriodAggregationType.BiMonthly,
				firstDayOfTheYear,
				lastTimeOfTheYear,
				ConsumptionDataRetrievalType.NonSmart);

			var smartData = await GetAccountConsumptionData(
			request.AccountNumber,
			TimePeriodAggregationType.Monthly,
			firstDayOfTheYear,
			lastTimeOfTheYear,
			ConsumptionDataRetrievalType.Smart);

			var isSmartCostEntriesAvailable = smartData.CostEntries.Any(x => !string.IsNullOrWhiteSpace(x.Prn) && !string.IsNullOrWhiteSpace(x.SerialNumber));
			var isSmartUsageEntriesAvailable = smartData.UsageEntries.Any(x => !string.IsNullOrWhiteSpace(x.Prn) && !string.IsNullOrWhiteSpace(x.SerialNumber));

			if (isSmartCostEntriesAvailable && isSmartUsageEntriesAvailable)
			{
				
				var all = smartData.ToOneItemArray().Append(nonSmartData);

				var data = new AccountConsumption
				{
					CostEntries = all.SelectMany(x => x.CostEntries)
						.GroupBy(x => x.Date.GetBimonthlyGroupIndex())
						.Select(g =>
						{
							var month = g.Key * 2;

							var groupedTime = g.Select(x => x.Date).Distinct().Max();
							var costEntryDate = new DateTime(groupedTime.Year, month, DateTime.DaysInMonth(groupedTime.Year, month));
							return new AccountConsumption.CostEntry
							{
								Date = costEntryDate,
								Value = smartData.CostEntries.Any(x=>x.Date == costEntryDate && !string.IsNullOrWhiteSpace(x.Prn) && 
															  !string.IsNullOrWhiteSpace(x.SerialNumber)) ?
										g.Where(v => !string.IsNullOrWhiteSpace(v.SerialNumber) && !string.IsNullOrWhiteSpace(v.Prn))
										.Sum(_ => _.Value) :
										g.Sum(_ => _.Value)
							};
						})
						.ToArray(),
					UsageEntries = all.SelectMany(x => x.UsageEntries)
						.GroupBy(x => x.Date.GetBimonthlyGroupIndex())
						.Select(g =>
						{
							var month = g.Key * 2;

							var groupedTime = g.Select(x => x.Date).Distinct().Max();
							var usageEntryDate = new DateTime(groupedTime.Year, month, DateTime.DaysInMonth(groupedTime.Year, month));
							return new AccountConsumption.UsageEntry
							{
								Date = usageEntryDate,
								Value = g.Sum(_ => _.Value)
							};
						})
						.ToArray()
				};
				return MapData(data, request.Period, "bimonthly", firstDayOfTheYear);
			}
			else
			{
				return MapData(nonSmartData, request.Period, "bimonthly", firstDayOfTheYear);
			}



		}
	}
}