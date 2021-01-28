using System.Collections.Generic;
using System.Threading.Tasks;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;

namespace EI.RP.DomainServices.InternalShared.Accounts
{
	internal interface IAccountExtraInfoResolver
	{
		Task<bool> AddressesMatchForBundle(string accountNumber);
		Task<bool> CanEqualizePayments(AccountInfo accountInfo);
	
	}
}