using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.DataModels.Events;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Queries.Contracts.Accounts;

namespace EI.RP.DomainServices.Commands.Contracts.CloseAccounts.Services
{
	class CloseAccountInfo :   ICloseAccountInfoProvider
	{
		private readonly IDomainQueryResolver _queryResolver;

		public CloseAccountInfo(IDomainQueryResolver queryResolver)
		{
			_queryResolver = queryResolver;
		}

		public async Task<CloseAccountInfo> Resolve(CloseAccountsCommand commandData)
		{
			var electricityAccountTask =
				_queryResolver.GetAccountInfoByAccountNumber(commandData.MeterReadingElectricityAccount?.AccountNumber, true);
			var gasAccountTask = _queryResolver.GetAccountInfoByAccountNumber(commandData.MeterReadingGasAccount?.AccountNumber, true);


			ElectricityAccount = await electricityAccountTask;
			GasAccount = await gasAccountTask;
ContractAccount = commandData.ContractAccountType == ClientAccountType.Electricity
					? ElectricityAccount
					: GasAccount;

				AuditSubCategory = GasAccount == null
				? EventSubCategory.CloseElectricity
				: ElectricityAccount == null
				? EventSubCategory.CloseGas
				: EventSubCategory.CloseElectricityAndGas;

			return this;
		}

		public long AuditSubCategory { get; set; }

		public AccountInfo ContractAccount { get; set; }

		public AccountInfo GasAccount { get; set; }

		public AccountInfo ElectricityAccount { get; set; }

		public IEnumerable<AccountInfo> AllAccounts()
		{
			return new[] { ElectricityAccount, GasAccount }.Where(x => x != null);
		}
	}
}