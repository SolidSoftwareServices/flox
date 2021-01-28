using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.DataModels.Sap.ErpUmc.Dtos;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using EI.RP.DomainServices.Infrastructure.Mappers;
using EI.RP.DomainServices.Queries.Contracts.Accounts;

namespace EI.RP.DomainServices.Queries.Metering.MeterReadings
{
	internal class MeterReadingsQueryHandler : QueryHandler<MeterReadingsQuery>
	{
		private readonly IDomainQueryResolver _queryResolver;
		private readonly IDomainMapper<MeterReadingResultDto, MeterReadingInfo> _mapper;
		private readonly ISapRepositoryOfErpUmc _repository;

		public MeterReadingsQueryHandler(
			ISapRepositoryOfErpUmc repository, IDomainQueryResolver queryResolver, IDomainMapper<MeterReadingResultDto, MeterReadingInfo> mapper)
		{
			_repository = repository;
			_queryResolver = queryResolver;
			_mapper = mapper;
		}

		protected override Type[] ValidQueryResultTypes { get; } = {typeof(MeterReadingInfo)};

		protected override async Task<IEnumerable<TQueryResult>> _ExecuteQueryAsync<TQueryResult>(
			MeterReadingsQuery queryModel)
		{
			var accountTask = _queryResolver.GetAccountInfoByAccountNumber(queryModel.AccountNumber, true);

			IFluentODataModelQuery<ContractDto> query;

			if (queryModel.AccountNumber != null)
			{
				query = _repository.NewQuery<ContractAccountDto>().Key(queryModel.AccountNumber)
					.NavigateTo<ContractDto>();
			}
			else
			{
				query = _repository.NewQuery<ContractDto>();
			}

			var accountInfo = await accountTask;

			query = query
				.Key(accountInfo.ContractId)
				.Expand(x => x.Devices)
				.Expand(x => x.Devices[0].MeterReadingResults)
				.Expand(x => x.Devices[0].MeterReadingResults[0].MeterReadingCategory)
				.Expand(x => x.Devices[0].MeterReadingResults[0].Device.RegistersToRead[0].RegisterType)
				.Expand(x => x.Devices[0].RegistersToRead)
				.Expand(x => x.Devices[0].RegistersToRead[0].MeterReadingCategory)
				.Expand(x => x.Devices[0].RegistersToRead[0].RegisterType);

			var contractDtos = await _repository.GetMany(query);
			IEnumerable<MeterReadingInfo> result;

			if (accountInfo.ClientAccountType == ClientAccountType.Electricity)
			{
				result = (await Task.WhenAll(contractDtos.SelectMany(x => x.Devices).Select(async x =>
					await GetElectricityMeterReadingHistory(x, ClientAccountType.Electricity, queryModel))))
					.SelectMany(s => s);
			}
			else if (accountInfo.ClientAccountType == ClientAccountType.Gas)
			{
				result = (await Task.WhenAll(contractDtos.SelectMany(x => x.Devices)
						.Select(async x => await GetGasMeterReadingHistory(x, ClientAccountType.Gas, queryModel))))
					.SelectMany(s => s);
			}
			else
			{
				throw new NotSupportedException();
			}

			return result.OrderByDescending(x => x.ReadingDateTime).Cast<TQueryResult>().ToArray();
		}

		private IEnumerable<MeterReadingResultDto> GetMeterReadingResultsByDateTimeDesc(DeviceDto device,
			int monthsToGet, ClientAccountType accountType)
		{
			bool IsGasResult(MeterReadingResultDto x)
			{
				return accountType == ClientAccountType.Gas &&
				       (x.MeterReadingStatusID != MeterReadingStatus.OrderCreated ||
				        x.MeterReadingReasonID != MeterReadingReason.MeterReadingAtMoveIn);
			}

			bool IsElectricityResult(MeterReadingResultDto x)
			{
				return accountType == ClientAccountType.Electricity &&
				       x.MeterReadingStatusID != MeterReadingStatus.OrderCreated &&
				       x.MeterReadingReasonID != MeterReadingReason.MeterReadingAtMoveIn;
			}

			return device.MeterReadingResults
				.Where(x => x.ReadingDateTime > DateTime.Now.AddMonths(-monthsToGet) &&
				            (IsElectricityResult(x) || IsGasResult(x)))
				.OrderByDescending(x => x.ReadingDateTime);
		}

		private async Task<IEnumerable<MeterReadingInfo>> GetGasMeterReadingHistory(DeviceDto device, ClientAccountType accountType, MeterReadingsQuery query)
		{
			var result = new List<MeterReadingInfo>();
			var meterRead24Months = GetMeterReadingResultsByDateTimeDesc(device, 24, accountType).ToArray();
			var meterRead27Months = GetMeterReadingResultsByDateTimeDesc(device, 27, accountType).ToArray();

			var prevIterateDate = string.Empty;
			var prevIterateMeterType = string.Empty;
			decimal? conversionFactor = null;
			string unit = null;
			decimal? consumption = null;
			decimal? reading =null;
			DateTime? previousReadingDate = null;

			for (var i = 0; i < meterRead27Months.Length; i++)
			{
				var item = meterRead27Months[i];
				var meterReadingResult =
					meterRead27Months.Skip(i + 1).FirstOrDefault(x => x.RegisterID == item.RegisterID);
				var meterType = device.RegistersToRead.FirstOrDefault(x => x.RegisterID == item.RegisterID)
					?.RegisterType.Description;
				var readingDate = item.ReadingDateTime.Value.ToString("dd-MMM-yyyy");
				if (!(prevIterateDate.Equals(readingDate) && prevIterateMeterType.Equals(meterType)))
				{
					prevIterateDate = readingDate;
					prevIterateMeterType = meterType;
				}

				//3 different entries are combined into one
				if (item.ReadingUnit == MeterUnit.VAL)
				{
					conversionFactor = item.ReadingResultAsDecimal();
				}
				else if (item.ReadingUnit == MeterUnit.M3)
				{
					reading = item.ReadingResultAsDecimal();
					unit = item.ReadingUnit;
				}
				else if (item.ReadingUnit == MeterUnit.KWH)
				{
					consumption = item.ConsumptionAsDecimal();
				}

				MeterReadingInfo readingInfo = null;
				if (unit != null && consumption != null && conversionFactor != null)
				{
					if (meterReadingResult?.ReadingDateTime != null)
						previousReadingDate = meterReadingResult.ReadingDateTime;
					readingInfo = await MapGasReadings(device, item, previousReadingDate, reading, conversionFactor);

					unit = null;
					consumption = null;
					conversionFactor = 0M;
				}

				if (readingInfo != null)
					result.Add(readingInfo);
			}

			if (query.MeterReadingsPeriodFrom == null)
			{
				if (meterRead27Months.Length > meterRead24Months.Length)
					result = result.Where(x => x.DateFrom > DateTime.Now.AddMonths(-24)).ToList();
			}
			else
			{
				result = result.Where(x => x.DateFrom >= DateTime.Now.AddMonths(-Math.Abs(query.MeterReadingsPeriodFrom.FromPeriodsAgo))).ToList();
			}

			return result;
		}

		public async Task<IEnumerable<MeterReadingInfo>> GetElectricityMeterReadingHistory(DeviceDto device,
			ClientAccountType accountType, 
			MeterReadingsQuery query)
		{
			var result = new List<MeterReadingInfo>();
			var meterRead24Months = GetMeterReadingResultsByDateTimeDesc(device, 24, accountType).ToArray();
			var meterRead27Months = GetMeterReadingResultsByDateTimeDesc(device, 27, accountType).ToArray();
			DateTime? previousReadingDate = null;

			for (var i = 0; i < meterRead27Months.Length; i++)
			{
				var item = meterRead27Months[i];
				var meterReadingResult =
					meterRead27Months.Skip(i + 1).FirstOrDefault(x => x.RegisterID == item.RegisterID);

				if (meterReadingResult?.ReadingDateTime != null)
					previousReadingDate = meterReadingResult.ReadingDateTime;

				var readingInfo = await MapElectricity(device, item, previousReadingDate);

				result.Add(readingInfo);
			}

			if (query.MeterReadingsPeriodFrom == null)
			{
				if (meterRead27Months.Length > meterRead24Months.Length)
					result = result.Where(x => x.ReadingDate > DateTime.Now.AddMonths(-24)).ToList();
			}
			else
			{
				result = result.Where(x => x.ReadingDate >= DateTime.Now.AddMonths(-Math.Abs(query.MeterReadingsPeriodFrom.FromPeriodsAgo))).ToList();
			}

			return result;
		}

		private async Task<MeterReadingInfo> MapElectricity(DeviceDto device, MeterReadingResultDto item,
			DateTime? previousReadingDate)
		{
			var readingInfo = await _mapper.Map(item);
			readingInfo.AccountType = ClientAccountType.Electricity;
			if (previousReadingDate != null) readingInfo.DateFrom = previousReadingDate;

			if (readingInfo.ReadingDate == readingInfo.DateFrom)
			{
				var lastMeterRead = device.MeterReadingResults
					.Where(x => x.ReadingDateTime < item.ReadingDateTime)
					.OrderByDescending(x => x.ReadingDateTime)
					.FirstOrDefault();

				readingInfo.DateFrom = lastMeterRead != null ? lastMeterRead.ReadingDateTime : readingInfo.DateFrom;
			}

			return readingInfo;
		}


		private async Task<MeterReadingInfo> MapGasReadings(DeviceDto device, MeterReadingResultDto item,
			DateTime? previousReadingDate, decimal? reading, decimal? conversionFactor)
		{
			var readingInfo = await _mapper.Map(item);
			readingInfo.AccountType = ClientAccountType.Gas;
			if (previousReadingDate != null) readingInfo.DateFrom = previousReadingDate.Value.AddDays(1);

			if (readingInfo.ReadingDate?.AddDays(1) == readingInfo.DateFrom)
			{
				var lastMeterReading = device.MeterReadingResults
					.Where(x => x.ReadingDateTime < item.ReadingDateTime)
					.OrderByDescending(x => x.ReadingDateTime)
					.FirstOrDefault();

				readingInfo.DateFrom =
					lastMeterReading != null ? lastMeterReading.ReadingDateTime : readingInfo.DateFrom;
			}

			readingInfo.Reading = reading??0M;
			readingInfo.ConversionFactor = conversionFactor??1M;
			return readingInfo;
		}
	}
}