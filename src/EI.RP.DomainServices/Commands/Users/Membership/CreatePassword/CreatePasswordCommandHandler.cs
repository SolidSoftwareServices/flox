using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.DataModels.Sap.CrmUmcUrm.Dtos;
using EI.RP.DataServices;
using EI.RP.DomainServices.Commands.Users.Session.CreateUserSession;
using EI.RP.DomainServices.ModelExtensions;
using NLog;

namespace EI.RP.DomainServices.Commands.Users.Membership.CreatePassword
{
	internal class CreatePasswordCommandHandler : ICommandHandler<CreatePasswordCommand>
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		private readonly ICommandHandler<CreateUserSessionCommand> _createSessionCommandHandler;

		private readonly ISapRepositoryOfCrmUmcUrm _dataRepository;


		public CreatePasswordCommandHandler(ISapRepositoryOfCrmUmcUrm dataRepository,
			ICommandHandler<CreateUserSessionCommand> createSessionCommandHandler)
		{
			_dataRepository = dataRepository;
			_createSessionCommandHandler = createSessionCommandHandler;
		}

		public async Task ExecuteAsync(CreatePasswordCommand command)
		{
			if (!string.IsNullOrWhiteSpace(command.ActivationKey) &&
			    !string.IsNullOrWhiteSpace(command.RequestId))
			{
				try
				{
					await _dataRepository.Update(new UserRequestActivationRequestDto
					{
						ActivationKey = command.ActivationKey,
						RequestID = command.RequestId,
						Password = command.NewPassword.AdaptToSapPasswordFormat()
					});
				}
				catch
				{
					Logger.Info(()=> $"link already used ak:{command.ActivationKey},rId:{command.RequestId}");
					//TODO:Missing log and explicit catching of"link already used"
				}

				await _createSessionCommandHandler.ExecuteAsync(new CreateUserSessionCommand(
					command.Email,
					command.NewPassword.AdaptToSapPasswordFormat()));
			}
		}
	}
}