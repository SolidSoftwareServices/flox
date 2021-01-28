using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DataServices;

namespace EI.RP.DomainServices.Commands.BusinessPartner.ActivateAsEBiller
{
    internal class ActivateBusinessAgreementAsEBillerCommandHandler : ICommandHandler<ActivateBusinessAgreementAsEBillerCommand>
	{
		private readonly ISapRepositoryOfCrmUmc _repositoryOfCrmUmc;

		public ActivateBusinessAgreementAsEBillerCommandHandler(ISapRepositoryOfCrmUmc repositoryOfCrmUmc)
		{
			_repositoryOfCrmUmc = repositoryOfCrmUmc;
		}

		public async Task ExecuteAsync(ActivateBusinessAgreementAsEBillerCommand command)
        {
			var agreement = await _repositoryOfCrmUmc
				.NewQuery<BusinessAgreementDto>()
				.Key(command.BusinessAgreementID)
				.GetOne();

			if (agreement.EBiller != SapBooleanFlag.Yes)
			{
				agreement.EBiller = SapBooleanFlag.Yes;
				await _repositoryOfCrmUmc.Update(agreement);
			}			
		}
	}
}