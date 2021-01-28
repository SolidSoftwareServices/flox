using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.DataModels.Sap.ErpUmc.Dtos;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.Contracts.Accounts;

namespace EI.RP.DomainServices.InternalShared.Contracts
{
	[Obsolete("The need of this needs to be supported by the public domain contract query")]
	class ContractQueryResolver:IContractQueryResolver
	{
		private readonly ISapRepositoryOfErpUmc _erpRepository;

		public ContractQueryResolver(ISapRepositoryOfErpUmc erpRepository)
		{
			_erpRepository = erpRepository;
		}

		public  async Task<ContractDto> GetCurrentContract(AccountInfo account)
		{
			if (!account.ContractStatus.IsAcquisitionCompletedState()) return null;

			var query = _erpRepository.NewQuery<ContractAccountDto>()
					.Key(account.AccountNumber)
					.NavigateTo<ContractDto>()
					.Key(account.ContractId);
			query = AddExpansions(query,account);
			return await query.GetOne();
		}

		private IFluentODataModelQuery<ContractDto> AddExpansions(IFluentODataModelQuery<ContractDto> query,
			AccountInfo account)
		{

			query = query
				.Expand(x => x.Devices)
				.Expand(x => x.ActivePaymentScheme)
				.Expand(x => x.ContractAccount.ContractAccountBalance)
				.Expand(x => x.Premise)
				.Expand(x => x.Premise.Installations[0].InstallationFacts);
			
			if (account.IsOpen)
			{
				query = query
					.Expand(x => x.Devices[0].MeterReadingResults)
					.Expand(x => x.Devices[0].FutureMeterReadings);
			}

			return query;
		}

		public async Task<IEnumerable<ContractDto>> GetAllContracts(AccountInfo account)
		{
			if (!account.ContractStatus.IsAcquisitionCompletedState()) return new ContractDto[0];

			var query = _erpRepository.NewQuery<ContractAccountDto>()
				.Key(account.AccountNumber)
				.NavigateTo<ContractDto>();
			query=AddExpansions(query,account);
				
			return await query.GetMany();
		}
	
	}
}
