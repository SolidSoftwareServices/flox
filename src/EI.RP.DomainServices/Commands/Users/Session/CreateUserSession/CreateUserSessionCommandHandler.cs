using System.Security.Claims;
using System.Threading.Tasks;
using EI.RP.CoreServices.Caching;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.CoreServices.Http.Session;
using EI.RP.DataServices;
using Ei.Rp.DomainErrors;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Membership;
using EI.RP.DomainServices.Infrastructure.Settings;
using EI.RP.DomainServices.ModelExtensions;

namespace EI.RP.DomainServices.Commands.Users.Session.CreateUserSession
{
	internal class CreateUserSessionCommandHandler : ICommandHandler<CreateUserSessionCommand>
	{
		private readonly ISapRepositoryOfUserManagement _repositoryOfUserManagement;
		private readonly IUserSessionProvider _userSessionProvider;
		private readonly IDomainSettings _domainSettings;
		private readonly ICacheAccountPreLoaderRequester _cachePreloader;

		public CreateUserSessionCommandHandler(ISapRepositoryOfUserManagement repositoryOfUserManagement,
			IUserSessionProvider userSessionProvider,IDomainSettings domainSettings, ICacheAccountPreLoaderRequester cachePreloader)
		{
			_repositoryOfUserManagement = repositoryOfUserManagement;
			_userSessionProvider = userSessionProvider;
			_domainSettings = domainSettings;
			_cachePreloader = cachePreloader;
		}


		public async Task ExecuteAsync(CreateUserSessionCommand command)
		{
			var userName = command.UserEmail.ToString().AdaptToSapUserNameFormat();

			Task preloadUserAccountsTask=Task.CompletedTask;
			if (!command.IsServiceAccount && !_domainSettings.IsInternalDeployment)
			{
				preloadUserAccountsTask = _cachePreloader.SubmitRequestAsync(userName);
			}

			var sessionInfo = await _repositoryOfUserManagement.LoginUser(userName,

				command.AdaptPasswordToSapConstraints
					? command.Password.AdaptToSapPasswordFormat()
					: command.Password,clearExistingSessionIfAnyOnError:!command.ChangingPassword);

			if (!command.IsServiceAccount && (IsAgentInNonInternalDeployment()|| IsNonAgentInInternalDeployment()))
			{
				throw new DomainException(ResidentialDomainError.AuthorizationError);
			}
			

            await _userSessionProvider.CreateSession(new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Email, userName),
                new Claim(ClaimTypes.NameIdentifier, userName),
                new Claim(ClaimTypes.Role, sessionInfo.SapUserRole),
                new Claim(ResidentialPortalClaim.SapCookie, sessionInfo.JsonCookie),
                new Claim(ResidentialPortalClaim.Csrf, sessionInfo.Csrf));

            await preloadUserAccountsTask;
			bool IsAgentInNonInternalDeployment()
			{
				return (sessionInfo.SapUserRole == ResidentialPortalUserRole.AgentUser ||
				        sessionInfo.SapUserRole == ResidentialPortalUserRole.AdminUser) 
				       && !_domainSettings.IsInternalDeployment;
			}

			bool IsNonAgentInInternalDeployment()
			{
				return !(sessionInfo.SapUserRole == ResidentialPortalUserRole.AgentUser ||
				         sessionInfo.SapUserRole == ResidentialPortalUserRole.AdminUser)
				       && _domainSettings.IsInternalDeployment;
			}
		}
	}
}