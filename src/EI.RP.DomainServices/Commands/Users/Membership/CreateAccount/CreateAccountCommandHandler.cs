using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.DataModels.Sap.CrmUmcUrm.Dtos;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.ModelExtensions;

namespace EI.RP.DomainServices.Commands.Users.Membership.CreateAccount
{
	internal class CreateAccountCommandHandler : ICommandHandler<CreateAccountCommand>
	{
		private readonly ISapRepositoryOfCrmUmcUrm _dataRepository;

		public CreateAccountCommandHandler(ISapRepositoryOfCrmUmcUrm dataRepository)
		{
			_dataRepository = dataRepository;
		}

		public async Task ExecuteAsync(CreateAccountCommand command)
		{
			var dataModel = await Map(command);
			await _dataRepository.Add(dataModel);
		}

		private async Task<UserRequestDto> Map(CreateAccountCommand domainCommandData)
		{
			var result = new UserRequestDto
			{
				ID = string.Empty,
				BusinessAgreementID = domainCommandData.AccountNumber,
				EmailAddress = domainCommandData.UserEmail,
				PhoneNumber = domainCommandData.ContactPhoneNumber,
				UsrCategory = UserCategory.SignUpUser,
				PoD = domainCommandData.MPRNGPRNLast6DigitsOf,
				AccountOwnerFlag = domainCommandData.AccountOwnerFlag,
				TermsConditionsFlag = domainCommandData.TermsConditionsFlag,
				Birthday = domainCommandData.Birthday,
				UserName = domainCommandData.UserEmail?.ToString().AdaptToSapUserNameFormat()
			};

			return result;
		}
	}
}