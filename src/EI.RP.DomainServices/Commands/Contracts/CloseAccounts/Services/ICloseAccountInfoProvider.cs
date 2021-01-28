using System.Threading.Tasks;

namespace EI.RP.DomainServices.Commands.Contracts.CloseAccounts.Services
{
	internal interface ICloseAccountInfoProvider
	{
		Task<CloseAccountInfo> Resolve(CloseAccountsCommand commandData);

	}
}