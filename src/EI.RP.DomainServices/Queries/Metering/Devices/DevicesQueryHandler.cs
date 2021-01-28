using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Caching;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.CoreServices.Resiliency;
using EI.RP.DataModels.Sap.ErpUmc.Dtos;
using EI.RP.DataServices;
using Ei.Rp.DomainErrors;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.Metering;
using EI.RP.DomainServices.Infrastructure.Mappers;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.DomainServices.Queries.Metering.Premises;
using BusinessAgreementDto = EI.RP.DataModels.Sap.CrmUmc.Dtos.BusinessAgreementDto;
using ContractItemDto = EI.RP.DataModels.Sap.CrmUmc.Dtos.ContractItemDto;
using Ei.Rp.DomainModels.MappingValues;
using NLog;

namespace EI.RP.DomainServices.Queries.Metering.Devices
{
	internal class DevicesQueryHandler : QueryHandler<DevicesQuery>
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		private readonly IDomainQueryResolver _queryResolver;
		private readonly IDomainMapper<DeviceDto, InstallationDto, DeviceInfo> _deviceMapper;
		private readonly ICacheProvider _cache;
		private readonly IUserSessionProvider _userSessionProvider;
		private readonly ISapRepositoryOfErpUmc _erpUmc;
		private readonly ISapRepositoryOfCrmUmc _crmUmcRepository;

		public DevicesQueryHandler(
			ISapRepositoryOfErpUmc erpUmc,
			ISapRepositoryOfCrmUmc crmUmcRepository,
			IDomainQueryResolver queryResolver,
			IDomainMapper<DeviceDto, InstallationDto, DeviceInfo> deviceMapper,
			ICacheProvider cache,
			IUserSessionProvider userSessionProvider)
		{
			_erpUmc = erpUmc;
			_crmUmcRepository = crmUmcRepository;
			_queryResolver = queryResolver;
			_deviceMapper = deviceMapper;
			_cache = cache;
			_userSessionProvider = userSessionProvider;
		}

		protected override Type[] ValidQueryResultTypes { get; } = { typeof(DeviceInfo) };

		protected override async Task<IEnumerable<TQueryResult>> _ExecuteQueryAsync<TQueryResult>(DevicesQuery query)
		{
			IFluentODataModelQuery<DeviceDto> devicesQuery = null;
			Premise premise = null;

             var accountInfo = string.IsNullOrEmpty(query.AccountNumber) || !string.IsNullOrEmpty(query.ContractId) ? 
	            await Task.FromResult<AccountInfo>(null) :
	            await _queryResolver.GetAccountInfoByAccountNumber(query.AccountNumber, true);

			if (query.PremisePrn != null)
			{
				premise = await _queryResolver.GetPremiseByPrn(query.PremisePrn, true);
				devicesQuery = ResolveByPremise(query.DeviceId, premise);
			}
			else if (!string.IsNullOrEmpty(query.AccountNumber) && !string.IsNullOrEmpty(query.ContractId))
            {

	            devicesQuery = await ResolveByAccount(query.AccountNumber, query.ContractId);
			}
            else if (!string.IsNullOrEmpty(query.AccountNumber) && !string.IsNullOrEmpty(query.DeviceId))
            {
	            accountInfo = await _queryResolver.GetAccountInfoByAccountNumber(query.AccountNumber, true);
				devicesQuery = await ResolveByAccount(query.AccountNumber, accountInfo.ContractId);
                devicesQuery.Key(query.DeviceId);
            }
            else if (!string.IsNullOrEmpty(query.AccountNumber))
            {
	            devicesQuery = await ResolveByAccount(query.AccountNumber, accountInfo.ContractId);
            }
            else
            {
                devicesQuery = await ResolveByDeviceId(query.DeviceId);
            }

			if (!string.IsNullOrEmpty(accountInfo?.AccountNumber) || !string.IsNullOrEmpty(query.AccountNumber))
			{
				var accountNumber = !string.IsNullOrEmpty(query.AccountNumber) ? query.AccountNumber : accountInfo?.AccountNumber;
				if (!await CanGetDevicesForAccountContract(accountNumber,query.ContractId))
				{
					//TODO: refactor this. single return 
					return new TQueryResult[0];
				}
			}

			devicesQuery = await AddQueryExpansions(devicesQuery);

			var result = await ExecuteQuery();

			return result.Cast<TQueryResult>().ToArray();
			async Task<IFluentODataModelQuery<DeviceDto>> AddQueryExpansions(IFluentODataModelQuery<DeviceDto> querySource)
			{

				var accountIsOpened = string.IsNullOrEmpty(query.AccountNumber)
					? null
					: (bool?)await CanRequestMeterReadings(query.AccountNumber);

				if (!accountIsOpened.HasValue|| accountIsOpened.Value)
				{
					querySource = querySource
						.Expand(x => x.RegistersToRead)
						.Expand(x => x.RegistersToRead[0].RegisterType)
						.Expand(x => x.RegistersToRead[0].MeterReadingCategory);
					if (accountIsOpened==true)
					{
						//this fails for closed accounts
						//TODO: account related devices should hang from the account info aggregate and not from the query
						//the devices query should only support devices unassociated to the account context
						querySource = querySource.Expand(x => x.MeterReadingResults);
					}
				}

				return querySource;
			}
			

			IFluentODataModelQuery<DeviceDto> ResolveByPremise(string deviceId, Premise p)
			{
				return _erpUmc
					.NewQuery<PremiseDto>().Key(p.PremiseId)
					.NavigateTo<InstallationDto>().Key(p.Installations.Single(x => x.Devices.Any(y => y.DeviceId == deviceId)).InstallationId)
					.NavigateTo<DeviceDto>();
			}

			async Task<IFluentODataModelQuery<DeviceDto>> ResolveByAccount(string accountNumber, string contractId)
			{
				IFluentODataModelQuery<ContractDto> contractQuery;

				if (accountNumber != null)
				{
					contractQuery = _erpUmc.NewQuery<ContractAccountDto>()
						.Key(accountNumber)
						.NavigateTo<ContractDto>();
				}
				else
				{
					contractQuery = _erpUmc.NewQuery<ContractDto>();
				}

				return  contractQuery
					.Key(contractId)
					.NavigateTo<DeviceDto>();

			}

			async Task<IFluentODataModelQuery<DeviceDto>> ResolveByDeviceId(string deviceId)
			{
				return _erpUmc.NewQuery<DeviceDto>().Key(deviceId);
			}

			async Task<DeviceInfo[]> ExecuteQuery()
			{
				try
				{
					return await ResilientOperations.Default.RetryIfNeeded(async () =>
					{
						//44307-retrying to compensate sap issues when moving home
						return await Task.WhenAll((await _erpUmc.GetMany(devicesQuery))
							.Where(x => query.DeviceId == null || x.DeviceID == query.DeviceId).Select(async deviceDto =>
							{
								var getInstallationDtoTask = GetInstallationDto(query, deviceDto, premise);
								return await _deviceMapper.Map(deviceDto, await getInstallationDtoTask);
							}));
					}, maxAttempts: 6, waitBetweenAttempts: TimeSpan.FromSeconds(1));
				}
				catch (DomainException ex)
				{
					Logger.Error(() => $"DevicesQuery that failed:{query} - {new StackTrace()} -{Environment.NewLine}{ex}");
					throw;
				}

			}
		}

		

		protected async Task<InstallationDto> GetInstallationDto(DevicesQuery queryModel, DeviceDto deviceDto, Premise premise = null)
		{
			IFluentODataModelQuery<InstallationDto> installationQuery = null;
			if (queryModel.PremisePrn != null)
			{
				installationQuery = _erpUmc
					.NewQuery<PremiseDto>().Key(premise.PremiseId)
					.NavigateTo<InstallationDto>().Key(premise.Installations
						.Single(x => x.Devices.Any(y => y.DeviceId == deviceDto.DeviceID)).InstallationId);
			}
			else
			{
				installationQuery = _erpUmc
					.NewQuery<InstallationDto>()
					.Key(deviceDto.InstallationID);
			}

			installationQuery = installationQuery
				.Expand(x => x.InstallationFacts);

			return await _erpUmc.GetOne(installationQuery);
		}



		private async Task<bool> CanGetDevicesForAccountContract(string accountNumber,string contractId)
		{
			var contract = await GetContract(accountNumber,contractId);

			var contractStatus = (ContractStatusType)contract.ContractItemEXTAttrs.USER_STAT_DESC_SHORT;
			return contractStatus.IsAcquisitionCompletedState();
		}
		private async Task<bool> CanRequestMeterReadings(string accountNumber)
		{
			var contract = await GetContract(accountNumber);

			return ((ContractStatusType)contract.ContractItemEXTAttrs.USER_STAT_DESC_SHORT)==ContractStatusType.Active;
		}

		private async Task<ContractItemDto> GetContract(string accountNumber,string contractId=null)
		{
			return await _cache.GetOrAddAsync($"{GetType().Name}.{nameof(GetContract)}.{accountNumber}_{contractId}",
				
				async () =>
				{
					var query = _crmUmcRepository.NewQuery<BusinessAgreementDto>()
						.Key(accountNumber)
						.NavigateTo<ContractItemDto>();
					if (contractId != null)
					{
						query = query.Key(contractId);
					}

					return await query
						.Expand(x => x.ContractItemEXTAttrs)
						.GetOne();
				},
				_userSessionProvider.ResolveUserBasedCacheKeyContext(),
				maxDurationFromNow:TimeSpan.FromSeconds(30) );
		}
	}
}