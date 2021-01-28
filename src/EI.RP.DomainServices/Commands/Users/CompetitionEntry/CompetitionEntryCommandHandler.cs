using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.DataModels.ResidentialPortal;
using EI.RP.DataServices;
using System.Threading.Tasks;

namespace EI.RP.DomainServices.Commands.Users.CompetitionEntry
{
    internal class CompetitionEntryCommandHandler : ICommandHandler<CompetitionEntryCommand>
    {
        private readonly IResidentialPortalDataRepository _repository;

        public CompetitionEntryCommandHandler(IResidentialPortalDataRepository repository)
        {
            _repository = repository;
        }

        public async Task ExecuteAsync(CompetitionEntryCommand command)
        {
            await _repository.AddCompetitionEntry(new CompetitionEntryDto
            {
                Answer = command.Answer,
                FirstName = command.FirstName,
                Surname = command.Surname,
                Email = command.Email,
                PhoneNumber = command.PhoneNumber,
                TCAccepted = command.TermsConditionsAccepted,
                CompetitionName = command.CompetitionName,
                CompetitionEntryDate = command.CompetitionEntryDate,
                UserName = command.UserName,
                BPNumber = command.BPNumber,
				IpAddress = command.IpAddress
            });
        }
    }
}
