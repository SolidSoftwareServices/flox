using System;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering.Consumption;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.DomainServices.Queries.Metering.Consumption;
using EI.RP.WebApp.Models.Charts;
using System.Collections.Generic;

namespace EI.RP.WebApp.Infrastructure.PresentationServices.ChartDataBuilders
{
	internal class ChartDataBuilderYear : ChartDataBuilder
	{
		public ChartDataBuilderYear(IDomainQueryResolver queryResolver) : base(queryResolver)
		{
		}

		public override PeriodType ForPeriodType { get; } = PeriodType.Year;


		protected override async Task<ConsumptionData> _GetChartConsumptionData(UsageChartRequest request)
		{
			var getAccountInfo = QueryResolver.GetAccountInfoByAccountNumber(request.AccountNumber);

			var firstDayOfTheYear = request.Date.Date.FirstDayOfTheYear();
			var getDataTasks = GetAccountConsumptionData(
				request.AccountNumber,
				TimePeriodAggregationType.Monthly,
				firstDayOfTheYear,
				request.Date.Date.LastDayOfTheYear(),
				ConsumptionDataRetrievalType.Smart).ToOneItemArray().ToList();

			var accountInfo = await getAccountInfo;

			var hasNonSmart = AddNonSmartIfNhhInTheLast2Years();
			var all = await Task.WhenAll(getDataTasks);
			var data = !hasNonSmart
				? all.Single()
				: BuildSmartAndNonSmartBimonthlyResult(accountInfo.SmartPeriods);

			return MapData(data, request.Period, "month", firstDayOfTheYear);

			bool AddNonSmartIfNhhInTheLast2Years()
			{
				var last2YearsRange = new DateTimeRange(DateTime.Today.AddYears(-2), DateTime.Today);

				var result = accountInfo.NonSmartPeriods.Any(x => x.Intersects(last2YearsRange));
				if (result)
				{
					var getNonSmartData = GetAccountConsumptionData(
						request.AccountNumber,
						TimePeriodAggregationType.BiMonthly,
						firstDayOfTheYear,
						request.Date.Date.LastDayOfTheYear(),
						ConsumptionDataRetrievalType.NonSmart);
					getDataTasks.Add(getNonSmartData);
				}

				return result;
			}

			AccountConsumption BuildSmartAndNonSmartBimonthlyResult(IEnumerable<DateTimeRange> smartPeriods)
			{
				return new AccountConsumption
				{
					CostEntries = all.SelectMany(x => x.CostEntries)
						.GroupBy(x => x.Date.GetBimonthlyGroupIndex())
						.Select(g =>
						{
							var month = g.Key * 2;

							var groupedTime = g.Select(x => x.Date).Distinct().Max();
							var costEntryDate = new DateTime(groupedTime.Year, month, DateTime.DaysInMonth(groupedTime.Year, month));
							bool isWithinSmartPeriod = smartPeriods.Any(p => p.Start <= costEntryDate && costEntryDate <= p.End);
							return new AccountConsumption.CostEntry
							{
								Date = costEntryDate,
								Value = isWithinSmartPeriod ?
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
			}
		}


	}
}