using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.CoreServices.Resiliency;
using EI.RP.DataModels.Sap.ErpUmc.Dtos;
using EI.RP.DataServices;
using Ei.Rp.DomainErrors;
using Ei.Rp.DomainModels.Contracts.Accounts;
using EI.RP.DomainServices.Commands.Contracts.MovingHouse.Services.ChangeBusinessAgreementOperations;
using EI.RP.DomainServices.Commands.Contracts.MovingHouse.Services.Context;
using EI.RP.DomainServices.Commands.Contracts.MovingHouse.Services.MoveInOperationsHandler;
using EI.RP.DomainServices.Commands.Contracts.MovingHouse.Services.MoveOutOperationsHandler;
using EI.RP.DomainServices.InternalShared.ContractSales;

namespace EI.RP.DomainServices.Commands.Contracts.MovingHouse
{
	internal class MoveHouseCommandHandler : ICommandHandler<MoveHouse>
	{
		private readonly IMoveHomeChangeBusinessAgreementOperationsHandler _changeBusinessAgreementOperationsHandler;
		private readonly ICompleteMovingHouseContextFactory _context;
		private readonly IContractSaleCommand _contractSaleBuilder;
		private readonly IMoveHomeInOperationsHandler _moveInOperationsHandler;
		private readonly IMoveHomeOutOperationsHandler _moveOutOperationsHandler;
		private readonly ISapRepositoryOfCrmUmc _crmUmc;
		private readonly ISapRepositoryOfErpUmc _erpUmc;
		public MoveHouseCommandHandler(ICompleteMovingHouseContextFactory context
			, IMoveHomeChangeBusinessAgreementOperationsHandler changeBusinessAgreementOperationsHandler,
			IMoveHomeOutOperationsHandler moveOutOperationsHandler, IContractSaleCommand contractSaleBuilder,
			IMoveHomeInOperationsHandler moveInOperationsHandler, ISapRepositoryOfCrmUmc crmUmc, ISapRepositoryOfErpUmc erpUmc)
		{
			_context = context;
			_changeBusinessAgreementOperationsHandler = changeBusinessAgreementOperationsHandler;
			_moveOutOperationsHandler = moveOutOperationsHandler;
			_contractSaleBuilder = contractSaleBuilder;
			_moveInOperationsHandler = moveInOperationsHandler;
			_crmUmc = crmUmc;
			_erpUmc = erpUmc;
		}

		public async Task ExecuteAsync(MoveHouse command)
		{
			command.Context = await _context.Resolve(command);
			await _moveOutOperationsHandler.StoreIncommingOccupant(command);

			var contractSale = await _contractSaleBuilder.ResolveContractSaleChecks(command, false);

			await _moveOutOperationsHandler.SubmitMoveOutMeterReadings(command, contractSale);

			await _moveInOperationsHandler.SubmitMoveInMeterReadings(command, contractSale);

			await _changeBusinessAgreementOperationsHandler.SetNewAddressAndBusinessAgreementChanges(command);

			await _contractSaleBuilder.ExecuteContractSale(command);
			
			await EnsureDataSourceReplication();

			async Task EnsureDataSourceReplication()
			{
				var accountsToVerify = command.Context.AllAccounts().Select(x=>x.Account);
				await ResilientOperations.Default.RetryIfNeeded(async () => await IsDataReplicatedAsync(accountsToVerify)
					, maxAttempts: 30);

			}
		}
		private async Task IsDataReplicatedAsync(IEnumerable<AccountInfo> accounts)
		{
			await Task.WhenAll(accounts.Select(VerifySingleAccountReplication));
			

			async Task VerifySingleAccountReplication(AccountInfo account)
			{
				var getErpContracts = _erpUmc.NewQuery<ContractAccountDto>()
					.Key(account.AccountNumber)
					.NavigateTo<ContractDto>()
					//this is why is failing after the synchronization.
					//Sometimes the odata request will fail unless is synced so we need to make sure that is successful when verifying the synchronisation.
					.Expand(x=>x.Devices)
					.GetMany();

				var crmContracts = await _crmUmc.NewQuery<AccountDto>()
					.Key(account.Partner)
					.NavigateTo<DataModels.Sap.CrmUmc.Dtos.ContractItemDto>()
					.GetMany();
				var erpContracts = await getErpContracts;
				var isReplicated = erpContracts.All(erpContract =>
					crmContracts.Any(crmContract => crmContract.ContractID == erpContract.ContractID));

				if (!isReplicated) throw new DomainException(ResidentialDomainError.ErrorDataNotReplicated);
			}
		}
	}
}