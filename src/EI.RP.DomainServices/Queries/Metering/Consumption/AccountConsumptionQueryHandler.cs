using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering.Consumption;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.DomainServices.Queries.Metering.Consumption.Services;
using NLog;

namespace EI.RP.DomainServices.Queries.Metering.Consumption
{
	internal class AccountConsumptionQueryHandler : QueryHandler<AccountConsumptionQuery>
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		private readonly IDomainQueryResolver _queryResolver;
		private readonly IEnumerable<IConsumptionResolver> _consumptionResolvers;

		public AccountConsumptionQueryHandler(IDomainQueryResolver queryResolver, IEnumerable<IConsumptionResolver> consumptionResolvers)
		{
			_queryResolver = queryResolver;
			_consumptionResolvers = consumptionResolvers;
		}


		protected override Type[] ValidQueryResultTypes => typeof(AccountConsumption).ToOneItemArray();

		protected override async Task<IEnumerable<TQueryResult>> _ExecuteQueryAsync<TQueryResult>(AccountConsumptionQuery queryModel)
		{

			var accountInfo =await _queryResolver.GetAccountInfoByAccountNumber(queryModel.AccountNumber, true);

			var result = (await Task.WhenAll(_consumptionResolvers.Where(x => x.ResolverFor(queryModel.RetrievalType))
					.Select(x => x.ResolveCostsAndUsage(queryModel, accountInfo))))
				.SelectMany(x => x)
				.OrderBy(x => x.usage?.Date ?? x.cost.Date);
		
			var src = FillZeroes(queryModel, result).ToArray();
			
			var consumption = new AccountConsumption
			{
				CostEntries =ResolveCostEntries(src),
				UsageEntries = ResolveUsageEntries(src)
			};

			if (consumption.CostEntries.Count() != consumption.UsageEntries.Count())
			{
				Logger.Warn(()=>$"Consumption cost entries and usage entries dont match for query: {queryModel}");
			}
			

			return consumption
				.ToOneItemArray()
				.Cast<TQueryResult>();

			AccountConsumption.CostEntry[] ResolveCostEntries(
				(AccountConsumption.CostEntry cost, AccountConsumption.UsageEntry usage)[] valueTuples)
			{
				return valueTuples.Select(x=>x.cost).GroupBy(x=>new {x.Date,x.SerialNumber}).Select(g =>
				{
					if (g.Count() == 1) return g.Single();
					var aggregate = AccountConsumption.CostEntry.From(g.First());
					aggregate.Value = g.Sum(x => x.Value);
					return aggregate;

				}).ToArray();
			}

			AccountConsumption.UsageEntry[] ResolveUsageEntries((AccountConsumption.CostEntry cost, AccountConsumption.UsageEntry usage)[] valueTuples)
			{
				return valueTuples.Select(x => x.usage).GroupBy(x => new { x.Date, x.SerialNumber }).Select(g =>
				{
					if (g.Count() == 1) return g.Single();
					var aggregate = AccountConsumption.UsageEntry.From(g.First());
					aggregate.Value = g.Sum(x => x.Value);
					return aggregate;

				}).ToArray();
			}
		}

		private IEnumerable<(AccountConsumption.CostEntry cost, AccountConsumption.UsageEntry usage)> FillZeroes(
			AccountConsumptionQuery queryModel,
			IEnumerable<(AccountConsumption.CostEntry cost, AccountConsumption.UsageEntry usage)> src)
		{
			
			if (!queryModel.FillResultWithZeroes) return src;

			var resolvedInput= FillEmptyDataPoints().ToList();


			var orderedItems = resolvedInput.Select(x => x.usage?.Date??x.cost?.Date).Distinct().Where(x=>x.HasValue).Select(x=>x.Value).ToHashSet();
			InitializeAllDataPoints();


			return resolvedInput.OrderBy(x => x.usage?.Date??x.cost.Date);

			AccountConsumption.CostEntry CreateZeroCost(DateTime pointTime)
			{
				return new AccountConsumption.CostEntry
				{
					Value = EuroMoney.Zero,
					Date = pointTime,
					SerialNumber = string.Empty,
					Prn = string.Empty,
				};
			}

			AccountConsumption.UsageEntry CreateZeroUsage(DateTime pointTime)
			{
				return new AccountConsumption.UsageEntry
				{
					Value = 0,
					Date = pointTime,
					SerialNumber = string.Empty,
					Prn = string.Empty
				};
			}


			void InitializeAllDataPoints()
			{
				var dataPointTimes = ResolveDatetimePoints(queryModel);
				
				foreach (var pointTime in dataPointTimes)
				{
					if (!orderedItems.Contains(pointTime))
					{
						resolvedInput.Add((CreateZeroCost(pointTime),
								CreateZeroUsage(pointTime)
							));
					}
				}
			}

			IEnumerable<(AccountConsumption.CostEntry cost, AccountConsumption.UsageEntry usage)> FillEmptyDataPoints()
			{
				IEnumerable<(AccountConsumption.CostEntry cost, AccountConsumption.UsageEntry usage)>
					correctInputResult=new (AccountConsumption.CostEntry cost, AccountConsumption.UsageEntry usage)[0];
				if (queryModel.AggregationType.IsOneOf(TimePeriodAggregationType.BiMonthly,
					TimePeriodAggregationType.Monthly))
				{
					correctInputResult = src.Select(x =>
					{
						var item = (
							cost: AccountConsumption.CostEntry.From(x.cost),
							usage: AccountConsumption.UsageEntry.From(x.usage)
						);
						if (item.cost != null)
						{
							item.cost.Date = item.cost.Date.LastDayOfTheMonth();
						}
						else
						{
							item.cost = CreateZeroCost(item.usage.Date.LastDayOfTheMonth());
						}

						if (item.usage != null)
						{
							item.usage.Date = item.usage.Date.LastDayOfTheMonth();
						}
						else
						{
							item.usage = CreateZeroUsage(item.cost.Date.LastDayOfTheMonth());
						}

						return item;
					}).ToArray();
				}
				else if (queryModel.AggregationType.IsOneOf(TimePeriodAggregationType.Daily,
					TimePeriodAggregationType.Hourly, TimePeriodAggregationType.HalfHourly))
				{
					var dataPoints =
						queryModel.Range.SplitInDataPointsByTimespan(queryModel.AggregationType.Timespan.Value);
					var target = src.ToDictionary(x => x.usage?.Date ?? x.cost.Date, x => x);
					foreach (var dataPoint in dataPoints)
					{
						if (!target.ContainsKey(dataPoint))
						{
							target.Add(dataPoint, (CreateZeroCost(dataPoint), CreateZeroUsage(dataPoint)));
						}
						else
						{
							var entry = target[dataPoint];
							if (entry.cost == null) entry.cost = CreateZeroCost(dataPoint);
							if (entry.usage == null) entry.usage = CreateZeroUsage(dataPoint);
							target[dataPoint] = entry;
						}
					}

					correctInputResult = target.Values.ToArray();
				}

				return correctInputResult;
			}
		}

		private IEnumerable<DateTime> ResolveDatetimePoints(AccountConsumptionQuery query)
		{
			IEnumerable<DateTime> result;
			if (query.AggregationType == TimePeriodAggregationType.BiMonthly)
			{
				result = query.Range.SplitInBiMonthlyDataPoints();
			}
			else if (query.AggregationType == TimePeriodAggregationType.Monthly)
			{
				result = query.Range.SplitInMonthlyDataPoints(useLastDayOfMonth:true);
			}
			else if (query.AggregationType == TimePeriodAggregationType.Daily)
			{
				result = query.Range.SplitInDailyDataPoints();
			}
			else if (query.AggregationType == TimePeriodAggregationType.Hourly)
			{
				result = query.Range.SplitInHourlyDataPoints();
			}
			else if (query.AggregationType == TimePeriodAggregationType.HalfHourly)
			{
				result = query.Range.SplitInHalfHourlyDataPoints();
			}
			else
			{
				throw new NotSupportedException();
			}

			return result;
		}
	}
}