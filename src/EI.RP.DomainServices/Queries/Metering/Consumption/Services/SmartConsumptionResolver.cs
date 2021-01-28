using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Sap.ErpUmc.Dtos;
using EI.RP.DataServices;
using Ei.Rp.DomainErrors;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using Ei.Rp.DomainModels.Metering.Consumption;
using EI.RP.DomainServices.Queries.Metering.Devices;
using EI.RP.DomainServices.Queries.Metering.Premises;

namespace EI.RP.DomainServices.Queries.Metering.Consumption.Services
{
	class SmartConsumptionResolver : IConsumptionResolver
	{
		private readonly ISapRepositoryOfErpUmc _erpUmc;
		private readonly IDomainQueryResolver _queryResolver;

		public SmartConsumptionResolver(ISapRepositoryOfErpUmc erpUmc, IDomainQueryResolver queryResolver)
		{
			_erpUmc = erpUmc;
			_queryResolver = queryResolver;
		}

		public async Task<IEnumerable<(AccountConsumption.CostEntry cost, AccountConsumption.UsageEntry usage)>>
			ResolveCostsAndUsage(AccountConsumptionQuery queryModel, AccountInfo accountInfo)
		{
			if(!ResolverFor(queryModel.RetrievalType)) throw new InvalidOperationException();

			var aggregationType = queryModel.AggregationType;

			var ranges = ResolveRanges(queryModel, accountInfo);

			var results=(await Task.WhenAll(ranges.Select(x => _ResolveCostsAndUsage(x, aggregationType, accountInfo))))
				.SelectMany(x=>x)
				//TODO: remove once SAP is fixed 35660 and tracked on 35698
				.Where(x=> queryModel.Range.Contains(x.usage?.Date??x.cost.Date))
				.OrderBy(x=>x.usage.Date)
				.ToArray();

			return results;
		}

		private IEnumerable<DateTimeRange> ResolveRanges(AccountConsumptionQuery queryModel, AccountInfo accountInfo)
		{
			IEnumerable<DateTimeRange> ranges;
			var smartDataRange = accountInfo.SmartPeriods
				.Where(x => queryModel.Range.Intersects(x)).ToArray();

			if (queryModel.AggregationType==TimePeriodAggregationType.Monthly)
			{
				ranges = CreateSmartDataSplitRange(smartDataRange, 365 * 2);
			}
			else if (queryModel.AggregationType == TimePeriodAggregationType.Daily)
			{
				ranges = CreateSmartDataSplitRange(smartDataRange, 365);
			}
			else if (queryModel.AggregationType == TimePeriodAggregationType.Hourly)
			{
				ranges = CreateSmartDataSplitRange(smartDataRange, 365 / 4);
			}
			else if (queryModel.AggregationType == TimePeriodAggregationType.HalfHourly)
			{
				ranges = CreateSmartDataSplitRange(smartDataRange, 365 / 4);
			}
			else
			{
				throw new NotSupportedException();
			}

			return ranges.Where(x => accountInfo.SmartPeriods.Any(p => p.Intersects(x))).Select(x=>x.ToSapFilterDateTime()).ToArray();
		}

		public List<DateTimeRange> CreateSmartDataSplitRange(DateTimeRange[] smartDataRange, int dayChunkSize)
		{
			List<DateTimeRange> ranges = new List<DateTimeRange>();
			foreach (var smartData in smartDataRange)
			{
				var splitRange = SplitInRangesOfDays(dayChunkSize, smartData.Start, smartData.End > DateTime.Today ? DateTime.Today : smartData.End);
				foreach (var range in splitRange)
				{
					ranges.Add(range);
				}
			}

			return ranges;
		}

		private IEnumerable<DateTimeRange> SplitInRangesOfDays(int dayChunkSize, DateTime start, DateTime end)
		{
			start = start.Date;
			end = end.Date;

			return Enumerable
				.Range(0, Convert.ToInt32((end - start).TotalDays) / dayChunkSize + 1)
				.Select(x => new DateTimeRange(start.AddDays(dayChunkSize * x), start.AddDays(dayChunkSize * (x + 1)).AddMilliseconds(-1) > end
					? end : start.AddDays(dayChunkSize * (x + 1)).AddMilliseconds(-1)));
		}

		private async Task<IEnumerable<(AccountConsumption.CostEntry cost, AccountConsumption.UsageEntry usage)>> _ResolveCostsAndUsage(DateTimeRange range,TimePeriodAggregationType aggregationType, AccountInfo accountInfo)
		{
			try
			{
				var queryAggregationString =  aggregationType.ToString();
				var getCosts = _erpUmc.NewQuery<ContractDto>()
					.Key(accountInfo.ContractId)
					.NavigateTo<ProfileValueCostDto>()
					.Filter(x =>
						x.Timestamp >= range.Start && x.Timestamp <= range.End &&
						x.OutputInterval == queryAggregationString, true)
					.GetMany(false);

				var grouped = GroupTariffs(await getCosts);
				return await Map(grouped.ToArray(), accountInfo);
			}
			catch (DomainException ex)
			{
				if (ex.DomainError == ResidentialDomainError.DataAlreadyReleased)
				{
					return new (AccountConsumption.CostEntry cost, AccountConsumption.UsageEntry usage)[0];
				}

				throw;
			}
		}

		public bool ResolverFor(ConsumptionDataRetrievalType retrievalType)
		{
			return retrievalType.IsOneOf(ConsumptionDataRetrievalType.Smart);
		}

		private IEnumerable<ProfileValueCostDto> GroupTariffs(IEnumerable<ProfileValueCostDto> original)
		{
			var result = original.GroupBy(x => new {x.Timestamp,x.DeviceID}).Select(group => new ProfileValueCostDto
			{
				Timestamp = @group.Key.Timestamp,
				DeviceID = @group.Key.DeviceID,
				Amount = @group.Sum(x => x.Amount),
				Consumption = @group.Sum(x => x.Consumption),
			});
			return result;
		}

	
		private async Task<IEnumerable<(AccountConsumption.CostEntry cost, AccountConsumption.UsageEntry usage)>> Map(
			IReadOnlyList<ProfileValueCostDto> costs,
			AccountInfo accountInfo)
		{

			var getDevices = ResolveDevicesDictionary();


			var devices = await getDevices;
			return await Task.WhenAll(costs.Select(async x =>
			{

				var deviceWithPrn =devices[x.DeviceID];
				
				return (new AccountConsumption.CostEntry
						{
							Value = x.Amount,
							Date = x.Timestamp,
							SerialNumber = deviceWithPrn.device.SerialNum,
							Prn = deviceWithPrn.prn,
						},
						new AccountConsumption.UsageEntry
						{
							Value = x.Consumption,
							Date = x.Timestamp,
							SerialNumber = deviceWithPrn.device.SerialNum,
							Prn = deviceWithPrn.prn,
						}
					);
			}));

			async Task<Dictionary<string, (DeviceInfo device, string prn)>> ResolveDevicesDictionary()
			{
				var devicesByAccount = await _queryResolver.GetDevicesByAccount(accountInfo.AccountNumber, byPassPipeline:true);
				var task = costs.Select(x => x.DeviceID)
					.Distinct()
					.Select(async deviceId =>
					{
						var deviceById = devicesByAccount.Single(d => d.DeviceId == deviceId);
						var premise = await _queryResolver.GetPremise(deviceById.PremiseId,true);
						return new KeyValuePair<string, (DeviceInfo device, string prn)>(deviceId,
								(device: deviceById, prn: premise.PointOfDeliveries?.SingleOrDefault()?.Prn));
					});
				return (await Task.WhenAll(task))
					.ToDictionary(x => x.Key, x => x.Value);
			}

		
		}

		
	}
}