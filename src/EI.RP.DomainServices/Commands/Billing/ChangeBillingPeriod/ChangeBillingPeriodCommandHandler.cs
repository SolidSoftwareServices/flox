using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Commands.Billing.ChangePaperBillChoice;

namespace EI.RP.DomainServices.Commands.Billing.ChangeBillingPeriod
{
	internal class ChangeBillingPeriodCommandHandler : ICommandHandler<SetMonthlyBillingPeriodCommand>, ICommandHandler<SetBiMonthlyBillingPeriodCommand>
	{
		private readonly ISapRepositoryOfCrmUmc _crmUmcRepository;
		private readonly IDomainCommandDispatcher _commandDispatcher;


		public ChangeBillingPeriodCommandHandler(ISapRepositoryOfCrmUmc crmUmcRepository,IDomainCommandDispatcher commandDispatcher)
		{
			_crmUmcRepository = crmUmcRepository;
			_commandDispatcher = commandDispatcher;
		}

		public async Task ExecuteAsync(SetMonthlyBillingPeriodCommand command)
		{
			await SetBillingDate(command.AccountNumber, command.DayOfTheMonth.ToString());
		}
		public async Task ExecuteAsync(SetBiMonthlyBillingPeriodCommand command)
		{
			await SetBillingDate(command.AccountNumber, string.Empty);
		}
		private async Task SetBillingDate(string accountNumber, string newBillingDate)
		{
			var businessAgreement =
				await _crmUmcRepository.GetOne(_crmUmcRepository.NewQuery<BusinessAgreementDto>()
					.Key(accountNumber));
			if (businessAgreement.FixedBillingDate != newBillingDate)
			{
				if (!string.IsNullOrEmpty(newBillingDate))
				{
					//changing through dispatcher to publish any event and other pipeline requirements
					await _commandDispatcher.ExecuteAsync(
						new ChangePaperBillChoiceCommand(accountNumber, PaperBillChoice.On), true);
				}

				businessAgreement.FixedBillingDate = newBillingDate;
				businessAgreement = await _crmUmcRepository.UpdateThenGet(businessAgreement);

				
			}
		}

	
	}
}