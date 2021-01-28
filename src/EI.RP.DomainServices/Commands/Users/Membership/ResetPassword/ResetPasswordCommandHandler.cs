using System;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.DataModels.Sap.CrmUmcUrm.Functions;
using EI.RP.DataServices;
using Ei.Rp.DomainErrors;
using EI.RP.DomainServices.Infrastructure.Util;
using EI.RP.DomainServices.ModelExtensions;

namespace EI.RP.DomainServices.Commands.Users.Membership.ResetPassword
{
	internal class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordDomainCommand>
	{
		private readonly ISapRepositoryOfCrmUmcUrm _dataRepository;


		public ResetPasswordCommandHandler(ISapRepositoryOfCrmUmcUrm dataRepository)
		{
			_dataRepository = dataRepository;
		}

		public async Task ExecuteAsync(ResetPasswordDomainCommand command)
		{
			var email = MapEmail(command);

			var result=await _dataRepository.ExecuteFunctionWithSingleResult(new ResetUserCredentialFunction
			{
				Query =
				{
					UserName = email
				}
			});
			if (result.ResetStatus.HasValue && !result.ResetStatus.Value)
			{
				throw new DomainException(ResidentialDomainError.ResetPassword_InvalidLink);
			}
		}

		private string MapEmail(ResetPasswordDomainCommand domainCommandData)
		{
			//NI portal User Check
			var niUsers = NIHelper.GetNIPortalUserValue();
			var commandDataEmail = domainCommandData.Email;
			var niuser = niUsers.FirstOrDefault(x =>
				x.Value.Equals(commandDataEmail, StringComparison.InvariantCultureIgnoreCase));
			if (niuser.Value != null)
				commandDataEmail += "_ROI";

			return commandDataEmail?.ToString().AdaptToSapUserNameFormat();
			
			
		}
	}
}