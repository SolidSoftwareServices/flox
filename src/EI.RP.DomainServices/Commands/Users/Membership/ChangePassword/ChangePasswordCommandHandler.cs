using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.CoreServices.Http.Session;
using EI.RP.DataModels.Sap.CrmUmcUrm.Dtos;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DataModels.Sap.UserManagement.Dtos;
using EI.RP.DataServices;
using Ei.Rp.DomainErrors;
using EI.RP.DomainServices.Commands.Users.Session.CreateUserSession;
using EI.RP.DomainServices.ModelExtensions;

namespace EI.RP.DomainServices.Commands.Users.Membership.ChangePassword
{
    internal class ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand>
    {
	    [Serializable]
	    internal class ChangePasswordCounter
	    {
		    public int Count { get; set; }
	    }

		private readonly ICommandHandler<CreateUserSessionCommand> _createSessionCommand;
        private readonly ISapRepositoryOfCrmUmcUrm _repositoryOfCrmUmcUrm;
        private readonly ISapRepositoryOfUserManagement _repositoryOfUserManagement;
        private readonly IUserSessionProvider _userSessionProvider;

        public ChangePasswordCommandHandler(
	        ISapRepositoryOfCrmUmcUrm repositoryOfCrmUmcUrm,
            ISapRepositoryOfUserManagement repositoryOfUserManagement,
            ICommandHandler<CreateUserSessionCommand> createSessionCommand,
	        IUserSessionProvider userSessionProvider)
        {
            _repositoryOfCrmUmcUrm = repositoryOfCrmUmcUrm;
            _repositoryOfUserManagement = repositoryOfUserManagement;
            _createSessionCommand = createSessionCommand;
            _userSessionProvider = userSessionProvider;
        }

        public async Task ExecuteAsync(ChangePasswordCommand command)
        {
			if(command.ActivationStatus == UserRequestStatusCode.Open)
				await ActivateUserIfNeeded(command);

            await EnsureCurrentPasswordIsCorrect(command);

            await UpdatePasswordAndRecreateSession(command);
        }

		private async Task UpdatePasswordAndRecreateSession(ChangePasswordCommand domainCommandData)
		{
			await _repositoryOfUserManagement.Update(new CredentialDto
			{
				UserName = domainCommandData.UserName,
				CurrentPassword = domainCommandData.CurrentPassword.AdaptToSapPasswordFormat(),
				Password = domainCommandData.NewPassword.AdaptToSapPasswordFormat()
			});

			await _createSessionCommand.ExecuteAsync(new CreateUserSessionCommand(
				domainCommandData.UserName,
				domainCommandData.NewPassword.AdaptToSapPasswordFormat()));
		}

		private async Task EnsureCurrentPasswordIsCorrect(ChangePasswordCommand domainCommandData)
        {
	        const int maxChangePasswordAttemptsAllowed = 4;

	        try
			{
				await _repositoryOfUserManagement.LoginUser(
					domainCommandData.UserName,
					domainCommandData.CurrentPassword,
		   clearExistingSessionIfAnyOnError: GetAndIncrementChangePasswordAttemptCount() > maxChangePasswordAttemptsAllowed
				);

				ResetChangePasswordAttemptCount();
			}
			catch (DomainException e)
			{
				if (!e.DomainError.Equals(ResidentialDomainError.AuthenticationError))
				{
					throw;
				}

				throw new DomainException(ResidentialDomainError.IncorrectPasswordError);
			}

	        string GetChangePasswordCounterKey()
	        {
				return $"{nameof(ChangePasswordCounter)}";
	        }

			int GetAndIncrementChangePasswordAttemptCount()
			{
				var changePasswordCounter = _userSessionProvider.Get<ChangePasswordCounter>(GetChangePasswordCounterKey()) ?? new ChangePasswordCounter();
				changePasswordCounter.Count++;
				_userSessionProvider.Set(GetChangePasswordCounterKey(), changePasswordCounter);
				return changePasswordCounter.Count;
			}

			void ResetChangePasswordAttemptCount()
			{
				_userSessionProvider.Remove(GetChangePasswordCounterKey());
			}
        }

        private async Task ActivateUserIfNeeded(ChangePasswordCommand domainCommandData)
        {
            if (!string.IsNullOrWhiteSpace(domainCommandData.ActivationKey) &&
                !string.IsNullOrWhiteSpace(domainCommandData.RequestId))
            {
                try
                {
                    await _repositoryOfCrmUmcUrm.Update(new UserRequestActivationRequestDto
                    {
                        ActivationKey = domainCommandData.ActivationKey,
                        RequestID = domainCommandData.RequestId,
                        Password = null
                    });
                }
                catch (DomainException e)
                {
                    if (!e.DomainError.Equals(ResidentialDomainError.AuthorizationError))
                    {
                        throw;
                    }
                }
            }
        }
    }
}