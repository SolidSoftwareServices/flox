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
	

	abstract class ChartDataBuilder : IChartDataBuilder
	{
		protected readonly IDomainQueryResolver QueryResolver;

		protected ChartDataBuilder(IDomainQueryResolver queryResolver)
		{
			QueryResolver = queryResolver;
		}

		public abstract PeriodType ForPeriodType { get; }
		protected async Task<AccountConsumption> GetAccountConsumptionData(string accountNumber,
			TimePeriodAggregationType timePeriodAggregationType,
			DateTime @from,
			DateTime to, ConsumptionDataRetrievalType retrievalType)
		{
			return await QueryResolver.GetAccountConsumption(accountNumber, timePeriodAggregationType,
				new DateTimeRange(@from, to), retrievalType, fillResultWithZeroes: true);
		}
		public async Task<ConsumptionData> GetChartConsumptionData(UsageChartRequest request)
		{

			if (request == null)
				throw new ArgumentNullException(nameof(request));

			if (request.Date.Equals(DateTime.MinValue))
				throw new ArgumentNullException(nameof(request.Date));

			if (string.IsNullOrEmpty(request.Period))
				throw new ArgumentNullException(nameof(request.Period));

			return await _GetChartConsumptionData(request);

		
		}

		protected abstract Task<ConsumptionData> _GetChartConsumptionData(UsageChartRequest request);
	
		protected ConsumptionData MapData(
			AccountConsumption accountConsumption,
			string periodName,
			string intervalName,
			DateTime from)
		{
			FilterOutOlderThan2Years();

			var ret = new ConsumptionData
			{
				Period =
				{
					Type = periodName,
					Start = from.ToString("yyyy-MM-dd"),
					Interval = intervalName
				},
				Price =
				{
					Values = accountConsumption.CostEntries.Select(x => Math.Round(x.Value.Amount ?? 0m, 2)).ToArray(),
					Total = Math.Round(accountConsumption.CostEntries.Sum(x => x.Value.Amount ?? 0m), 2)
				},
				Usage =
				{
					Values = accountConsumption.UsageEntries.Select(x => Math.Round(x.Value, 2)).ToArray(),
					Total = Math.Round(accountConsumption.UsageEntries.Sum(x => x.Value), 2)
				}
			};

			return ret;

			void FilterOutOlderThan2Years()
			{
				var lowerDateAllowed = DateTime.Today.AddYears(-2);
				accountConsumption.CostEntries = accountConsumption.CostEntries.Select(x =>
				{
					if (x.Date < lowerDateAllowed)
					{
						x.Value = 0M;
					}

					return x;
				}).ToArray();
				accountConsumption.UsageEntries = accountConsumption.UsageEntries.Select(x =>
				{
					if (x.Date < lowerDateAllowed)
					{
						x.Value = 0M;
					}

					return x;
				}).ToArray();
			}
		}

	}

	
}