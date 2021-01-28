using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Sap.ErpUmc.Dtos;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering.Consumption;
using EI.RP.DomainServices.Queries.Billing.Activity;
using EI.RP.DomainServices.Queries.Metering.Devices;
using Ei.Rp.DomainModels.Metering;

namespace EI.RP.DomainServices.Queries.Metering.Consumption.Services
{
	class NonSmartConsumptionResolver : IConsumptionResolver
	{
		private readonly IDomainQueryResolver _queryResolver;
		private readonly ISapRepositoryOfErpUmc _erpUrm;
		public NonSmartConsumptionResolver(IDomainQueryResolver queryResolver, ISapRepositoryOfErpUmc erpUrm)
		{
			_queryResolver = queryResolver;
			_erpUrm = erpUrm;
		}

		public bool ResolverFor(ConsumptionDataRetrievalType retrievalType)
		{
			return retrievalType.IsOneOf(ConsumptionDataRetrievalType.NonSmart);
		}
		public async Task<IEnumerable<(AccountConsumption.CostEntry cost, AccountConsumption.UsageEntry usage)>> ResolveCostsAndUsage(AccountConsumptionQuery queryModel, AccountInfo accountInfo)
		{
			if (!ResolverFor(queryModel.RetrievalType)) throw new InvalidOperationException();


			 var getUsage= GetUsage(queryModel,accountInfo) ;
			 var getCost = GetCost(queryModel, accountInfo);

			 var usage = (await getUsage).ToArray();
			 var cost = (await getCost).ToArray();
			 return usage.Select(x => x.Date)
				 .Union(cost.Select(x => x.Date))
				 .Distinct()
				 .Select(x=>(cost.SingleOrDefault(c=>c.Date==x),usage.SingleOrDefault(u=>u.Date==x)));


		}

		private async Task<IEnumerable<AccountConsumption.CostEntry>> GetCost(AccountConsumptionQuery queryModel,
			AccountInfo accountInfo)
		{
			var range = queryModel.Range.ToSapFilterDateTime();
			return (await _queryResolver.GetInvoicesByAccountNumber(accountInfo.AccountNumber,null, null, true))
				.Where(x => x.OriginalDate >= range.Start && x.OriginalDate <= range.End)
				.GroupBy(x => x.OriginalDate.GetBimonthlyGroupIndex())
				.Select(g =>
					{
						var month = g.Key * 2;

						var readingDateTime = g.Select(x => x.OriginalDate).Distinct().First();
						
						return new AccountConsumption.CostEntry
						{
							Value = g.Where(x=>x.IsBill()).Sum(x => (decimal) x.Amount),
							Date = new DateTime(readingDateTime.Year, month, DateTime.DaysInMonth(readingDateTime.Year, month)),
						};
					}
				)
				.OrderBy(x => x.Date);
		}

		private async Task<IEnumerable<AccountConsumption.UsageEntry>> GetUsage(AccountConsumptionQuery queryModel, AccountInfo accountInfo)
		{
			if(!accountInfo.IsOpen)
			{
				return new AccountConsumption.UsageEntry[0];
			}

			var getDevices = _queryResolver.GetDevicesByAccountAndContract(queryModel.AccountNumber, accountInfo.ContractId, true);
			
			return GetUsage((await getDevices).SelectMany(x => x.MeterReadingResults), queryModel.Range);
		}
		private IEnumerable<AccountConsumption.UsageEntry> GetUsage(IEnumerable<MeterReadingInfo> meterReadingResults, DateTimeRange range)
		{
			var r = range.ToSapFilterDateTime();
			var src = meterReadingResults
				.Where(x => x.ReadingDateTime >= r.Start && x.ReadingDateTime <= r.End
														  && x.ReadingUnit == MeterUnit.KWH
														  && (x.MeterReadingStatus != MeterReadingStatus.OrderCreated
															  || x.MeterReadingReasonID != MeterReadingReason.MeterReadingAtMoveIn))

				.GroupBy(x => x.ReadingDateTime.GetBimonthlyGroupIndex())
				.Select(g =>
				{
					var month = g.Key * 2;
					var readingDateTime = g.Select(x => x.ReadingDateTime).Distinct().First();
					return new AccountConsumption.UsageEntry
					{
						Value = g.Sum(x => x.Consumption),
						Date = new DateTime(readingDateTime.Year, month, DateTime.DaysInMonth(readingDateTime.Year, month)),
					};
				})
				.OrderBy(x => x.Date)
				.ToArray();

			return src;
		}

		
	}
}