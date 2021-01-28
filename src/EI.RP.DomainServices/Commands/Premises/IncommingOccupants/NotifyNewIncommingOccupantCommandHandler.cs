using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DataServices;
using EI.RP.DomainServices.Queries.Contracts.Accounts;

namespace EI.RP.DomainServices.Commands.Premises.IncommingOccupants
{
	internal class NotifyNewIncommingOccupantCommandHandler : ICommandHandler<NotifyNewIncommingOccupant>
	{
		private readonly ISapRepositoryOfCrmUmc _crmUmc;
		private readonly IDomainQueryResolver _queryResolver;

		public NotifyNewIncommingOccupantCommandHandler(ISapRepositoryOfCrmUmc crmUmc, IDomainQueryResolver queryResolver)
		{
			_crmUmc = crmUmc;
			_queryResolver = queryResolver;
		}

		public async Task ExecuteAsync(NotifyNewIncommingOccupant command)
		{
			var account = await _queryResolver.GetAccountInfoByAccountNumber(command.AccountNumber, true);
			var premiseDetail = await _crmUmc.NewQuery<BusinessAgreementDto>().Key(command.AccountNumber)
				.NavigateTo<ContractItemDto>().Key(account.ContractId)
				.NavigateTo<PremiseDto>()
				.NavigateTo<PremiseDetailDto>()
				.GetOne();


			premiseDetail.PremiseNotes =
				$"Name: {command.LettingAgentName}, Phone: {command.LettingPhoneNumber} -- {premiseDetail.PremiseNotes}";

			await _crmUmc.Update(premiseDetail);
		}
	}
}