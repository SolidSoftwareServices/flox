using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.CoreServices.Http.Session;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DataServices;
using Ei.Rp.DomainErrors;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Commands.Platform.PublishBusinessActivity;
using EI.RP.DomainServices.Commands.Platform.SendEmail;

namespace EI.RP.DomainServices.Commands.Contracts.AddAdditionalAccount.AdditionalAccount
{
    internal class AddAdditionalAccountCommandHandler : ICommandHandler<AddAdditionalAccountCommand>
    {
        private readonly ISapRepositoryOfCrmUmc _dataRepository;
        private readonly ICommandHandler<PublishBusinessActivityDomainCommand> _businessActivityPublisher;
        private readonly ICommandHandler<SendEmailCommand> _sendEmailHandler;
        private readonly IUserSessionProvider _userSessionProvider;

        public AddAdditionalAccountCommandHandler(IUserSessionProvider userSessionProvider, ICommandHandler<SendEmailCommand> sendEmailHandler,
            ICommandHandler<PublishBusinessActivityDomainCommand> businessActivityPublisher, ISapRepositoryOfCrmUmc dataRepository)
        {
            _userSessionProvider = userSessionProvider;
            _sendEmailHandler = sendEmailHandler;
            _businessActivityPublisher = businessActivityPublisher;
            _dataRepository = dataRepository;
        }

        public async Task ExecuteAsync(AddAdditionalAccountCommand command)
        {
            var additionalAccount = new AdditionalAccountRegistrationDto();
            additionalAccount.BusinessAgreementID = command.AccountNumber;
            additionalAccount.PoD = (string)command.MPRNGPRN;
            additionalAccount.UsrCategory = UserCategoryType.AddAdditionalAccountUserCategory;
            additionalAccount.UserName = _userSessionProvider.CurrentUserClaimsPrincipal.Identity.Name.Trim();
            try
            {
                await _dataRepository.Update(additionalAccount);
                await SubmitBusinessActivity(command.AccountNumber, command.Partner);
            }
            catch (DomainException ex)
            {
	            if (ex.DomainError.Equals(ResidentialDomainError.InvalidBusinessAgreement))
	            {
		            var sendEmailCommand = new SendEmailCommand(command.ContactType,
			            command.Subject, command.Comments, command.AccountNumber, command.Partner);
		            await _sendEmailHandler.ExecuteAsync(sendEmailCommand);
	            }
	            else
	            {
		            throw;
	            }
            }
        }

        private async Task SubmitBusinessActivity(string accountNumber, string partner)
        {
            var businessActivityType = PublishBusinessActivityDomainCommand.BusinessActivityType.AddAccount;

            await _businessActivityPublisher.ExecuteAsync(new PublishBusinessActivityDomainCommand(businessActivityType,
                partner, accountNumber));
        }
    }
}
