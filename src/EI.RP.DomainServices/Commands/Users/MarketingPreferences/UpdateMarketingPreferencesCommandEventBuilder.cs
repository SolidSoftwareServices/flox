using EI.RP.CoreServices.Cqrs.Events;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Http;
using EI.RP.CoreServices.Http.Session;
using EI.RP.DataModels.Events;
using EI.RP.DomainServices.Commands.Users.UserContact;
using System;
using System.Threading.Tasks;
using EI.RP.DomainServices.Queries.Contracts.Accounts;

namespace EI.RP.DomainServices.Commands.Users.MarketingPreferences
{
	internal class UpdateMarketingPreferencesCommandEventBuilder : ICommandEventBuilder<UpdateMarketingPreferencesCommand>
	{
		private readonly IClientInfoResolver _clientInfoResolver;
		private readonly IDomainQueryResolver _queryResolver;
		private readonly IUserSessionProvider _userSessionProvider;

		public UpdateMarketingPreferencesCommandEventBuilder(IClientInfoResolver clientInfoResolver,
			IUserSessionProvider userSessionProvider, IDomainQueryResolver queryResolver)
		{
			_clientInfoResolver = clientInfoResolver;
			_userSessionProvider = userSessionProvider;
			_queryResolver = queryResolver;
		}

		public async Task<IEventApiMessage[]> BuildEventsOnSuccess(UpdateMarketingPreferencesCommand command)
		{
			return await BuildFor(command, EventAction.LastOperationWasSuccessful);
		}

		public async Task<IEventApiMessage[]> BuildEventsOnError(UpdateMarketingPreferencesCommand command,
			AggregateException exceptions)
		{
			return await BuildFor(command, EventAction.LastOperationFailed);
		}

		private async Task<IEventApiMessage[]> BuildFor(UpdateMarketingPreferencesCommand command, long eventAction)
		{
			var account = await _queryResolver.GetAccountInfoByAccountNumber(command.AccountNumber, true);

			IEventApiMessage[] messages =
			{
				new EventApiEvent(_clientInfoResolver)
				{
					CategoryId = EventCategory.Change,
					ActionId = eventAction,
					Username = _userSessionProvider.CurrentUserClaimsPrincipal.Identity.Name,
					SubCategoryId = EventSubCategory.MarketPreferences,
					Partner = long.Parse(account.Partner),
					ContractAccount = long.Parse(command.AccountNumber),
					Description = eventAction == EventAction.LastOperationWasSuccessful
						? "Market Preference Updated Successfully"
						: "Failed to Update Market Preference"
				}
			};
			return messages;
		}
	}
}