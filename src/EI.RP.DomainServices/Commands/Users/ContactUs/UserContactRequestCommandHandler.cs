using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Commands.Contracts.AddAdditionalAccount.AdditionalAccount;
using EI.RP.DomainServices.Commands.Platform.SendEmail;

namespace EI.RP.DomainServices.Commands.Users.ContactUs
{
    internal class UserContactRequestCommandHandler : ICommandHandler<UserContactRequest>
    {
        private readonly ICommandHandler<SendEmailCommand> _sendEmailHandler;
        private readonly ICommandHandler<AddAdditionalAccountCommand> _addAdditionalCommandHandler;

        public UserContactRequestCommandHandler(ICommandHandler<SendEmailCommand> sendEmailHandler, ICommandHandler<AddAdditionalAccountCommand> addAdditionalCommandHandler)
        {
            _sendEmailHandler = sendEmailHandler;
            _addAdditionalCommandHandler = addAdditionalCommandHandler;
        }

        public async Task ExecuteAsync(UserContactRequest command)
        {
            if (command.ContactType == ContactQueryType.AddAdditionalAccount)
            {
                var addAdditionalAccountCommand = new AddAdditionalAccountCommand(command.Partner,command.AccountNumber,command.MPRN,command.Subject
                ,command.Comments,command.ContactType);
                await _addAdditionalCommandHandler.ExecuteAsync(addAdditionalAccountCommand);
            }
            else
            {
                var sendEmailCommand = new SendEmailCommand(command.ContactType,
                    command.Subject, command.Comments, command.AccountNumber, command.Partner);
                await _sendEmailHandler.ExecuteAsync(sendEmailCommand);
            }
        }
    }
}
