using System.Collections.Generic;
using System.Threading.Tasks;
using EI.RP.DataModels.Sap.ErpUmc.Dtos;
using Ei.Rp.DomainModels.Contracts.Accounts;

namespace EI.RP.DomainServices.InternalShared.Contracts
{
	internal interface IContractQueryResolver
	{
		Task<ContractDto> GetCurrentContract(AccountInfo account);
		Task<IEnumerable<ContractDto>> GetAllContracts(AccountInfo account);
	}
}