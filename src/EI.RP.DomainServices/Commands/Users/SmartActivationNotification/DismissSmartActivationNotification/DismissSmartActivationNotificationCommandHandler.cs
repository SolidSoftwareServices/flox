using System;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.DataModels.ResidentialPortal;
using EI.RP.DataServices;
using System.Threading.Tasks;
using EI.RP.CoreServices.Http.Session;

namespace EI.RP.DomainServices.Commands.Users.SmartActivationNotification.DismissSmartActivationNotification
{
	internal class DismissSmartActivationNotificationCommandHandler : ICommandHandler<DismissSmartActivationNotificationCommand>
	{
		private readonly IResidentialPortalDataRepository _repository;
		private readonly IUserSessionProvider _userSession;

		public DismissSmartActivationNotificationCommandHandler(IResidentialPortalDataRepository repository, IUserSessionProvider userSession)
		{
			_repository = repository;
			_userSession = userSession;
		}

		public async Task ExecuteAsync(DismissSmartActivationNotificationCommand command)
		{
			_repository.Save(new SmartActivationNotificationDto
			{
				UserName = _userSession.UserName,
				AccountNumber = command.AccountNumber,
				IsNotificationDismissed = true
			});
		}
	}
}
