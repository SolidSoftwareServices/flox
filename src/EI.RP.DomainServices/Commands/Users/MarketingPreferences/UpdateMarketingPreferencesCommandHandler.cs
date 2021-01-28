using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EI.RP.CoreServices.Caching;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Commands.Platform.PublishBusinessActivity;
using EI.RP.DomainServices.Queries.Contracts.Accounts;

namespace EI.RP.DomainServices.Commands.Users.MarketingPreferences
{
	internal class UpdateMarketingPreferencesCommandHandler : ICommandHandler<UpdateMarketingPreferencesCommand>
	{
		private readonly ISapRepositoryOfCrmUmc _repositoryOfCrmUmc;
		private readonly IDomainQueryResolver _queryResolver;


		private readonly ICommandHandler<PublishBusinessActivityDomainCommand> _businessActivityPublisher;

		public UpdateMarketingPreferencesCommandHandler(ISapRepositoryOfCrmUmc repositoryOfCrmUmc,
			IDomainQueryResolver domainQueryResolver,
			ICommandHandler<PublishBusinessActivityDomainCommand> businessActivityPublisher)
		{
			_repositoryOfCrmUmc = repositoryOfCrmUmc;
			_queryResolver = domainQueryResolver;
			_businessActivityPublisher = businessActivityPublisher;
		}

		public async Task ExecuteAsync(UpdateMarketingPreferencesCommand command)
		{
			var userContactInfoTask = GetContactInfo();

			var userContactInfo = await userContactInfoTask;

			await UpdateMarketingPreferences();


			await SubmitBusinessActivity(command.AccountNumber);

			#region LocalFunctions

			async Task UpdateMarketingPreferences()
			{

				await UpdateMarketingPreference(CommunicationPreferenceType.SMS, command.SmsMarketingActive);

				await UpdateMarketingPreference(CommunicationPreferenceType.Mobile, command.MobileMarketingActive);

				await UpdateMarketingPreference(CommunicationPreferenceType.Post, command.PostMarketingActive);

				await UpdateMarketingPreference(CommunicationPreferenceType.DoorToDoor,
					command.DoorToDoorMarketingActive);

				await UpdateMarketingPreference(CommunicationPreferenceType.Email, command.EmailMarketingActive);

				await UpdateMarketingPreference(CommunicationPreferenceType.LandLine,
					command.LandLineMarketingActive);
			}

			async Task UpdateMarketingPreference(CommunicationPreferenceType preferenceType,
				bool isActive)
			{
				foreach (var permission in userContactInfo
											.CommunicationPermissions
											.Where(c =>c.CommunicationChannelID == preferenceType))
				{
					await _repositoryOfCrmUmc.Delete(permission);
				}

				var acceptedStatus = isActive
					? CommunicationPermissionStatusType.Accepted
					: CommunicationPermissionStatusType.NotAccepted;

				var perm = new CommunicationPermissionDto
				{
					AccountID = userContactInfo.AccountID,
					CommunicationChannelID = preferenceType,
					CommunicationPermissionStatusID = acceptedStatus
				};


				await _repositoryOfCrmUmc.Add(perm);
			}

			#endregion LocalFunctions

			async Task<AccountDto> GetContactInfo()
			{
				var accountInfo =
					await _queryResolver.GetAccountInfoByAccountNumber(command.AccountNumber, true);

				return await _repositoryOfCrmUmc
					.NewQuery<AccountDto>()
					.Key(accountInfo.Partner)
					.Expand(x => x.CommunicationPermissions[0].CommunicationChannel)
					.Expand(x => x.CommunicationPermissions[0].CommunicationPermissionStatus)
					.GetOne();

			}
		}

		private async Task SubmitBusinessActivity(string accountNumber)
		{
			var accountInfo =
				await _queryResolver.GetAccountInfoByAccountNumber(accountNumber, true);
			var businessActivityType =
				PublishBusinessActivityDomainCommand.BusinessActivityType.UpdateUserContactDetails;

			await _businessActivityPublisher.ExecuteAsync(new PublishBusinessActivityDomainCommand(businessActivityType,
				accountInfo.Partner, accountNumber));
		}
	}
}