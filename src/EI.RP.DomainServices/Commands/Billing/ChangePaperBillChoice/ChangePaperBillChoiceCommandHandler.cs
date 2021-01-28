using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DataServices;

namespace EI.RP.DomainServices.Commands.Billing.ChangePaperBillChoice
{
	internal class ChangePaperBillChoiceCommandHandler : ICommandHandler<ChangePaperBillChoiceCommand>
	{
		private readonly ISapRepositoryOfCrmUmc _crmUmcRepository;


		public ChangePaperBillChoiceCommandHandler(ISapRepositoryOfCrmUmc crmUmcRepository)
		{
			_crmUmcRepository = crmUmcRepository;
		}

		public async Task ExecuteAsync(ChangePaperBillChoiceCommand command)
		{
			var businessAgreement =
				await _crmUmcRepository.GetOne(_crmUmcRepository.NewQuery<BusinessAgreementDto>()
					.Key(command.AccountNumber));
			if (businessAgreement.PaperBill != command.NewChoice)
			{
				businessAgreement.PaperBill = command.NewChoice;
				businessAgreement=await _crmUmcRepository.UpdateThenGet(businessAgreement);
			}
		}
	}
}