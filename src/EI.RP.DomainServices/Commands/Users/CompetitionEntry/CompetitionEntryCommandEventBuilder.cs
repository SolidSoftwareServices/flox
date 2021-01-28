using System;
using EI.RP.CoreServices.Cqrs.Events;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Http;
using EI.RP.CoreServices.Http.Session;
using EI.RP.DataModels.Events;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using System.Threading.Tasks;

namespace EI.RP.DomainServices.Commands.Users.CompetitionEntry
{
	internal class CompetitionEntryCommandEventBuilder: ICommandEventBuilder<CompetitionEntryCommand>
	{
		private readonly IDomainQueryResolver _queryResolver;
		private readonly IClientInfoResolver _clientInfoResolver;
		private readonly IUserSessionProvider _userSessionProvider;

		public CompetitionEntryCommandEventBuilder(IDomainQueryResolver queryResolver,
			IClientInfoResolver clientInfoResolver,
			IUserSessionProvider userSessionProvider)
		{
			_queryResolver = queryResolver;
			_clientInfoResolver = clientInfoResolver;
			_userSessionProvider = userSessionProvider;
		}

		public Task<IEventApiMessage[]> BuildEventsOnSuccess(CompetitionEntryCommand command)
		{
			return BuildFor(command, EventAction.LastOperationWasSuccessful);
		}

		public Task<IEventApiMessage[]> BuildEventsOnError(CompetitionEntryCommand command, AggregateException exceptions)
		{
			return BuildFor(command, EventAction.LastOperationFailed);
		}

		private async Task<IEventApiMessage[]> BuildFor(CompetitionEntryCommand command, long eventAction)
		{
			var account = await _queryResolver.GetAccountInfoByAccountNumber(command.AccountNumber, true);

			IEventApiMessage[] messages =
			{
				new EventApiEvent(_clientInfoResolver)
				{
					CategoryId = EventCategory.Competitions,
					ActionId = eventAction,
					Username = _userSessionProvider.CurrentUserClaimsPrincipal.Identity.Name,
					SubCategoryId = EventSubCategory.CompetitionEntry,
					Partner = long.Parse(account.Partner),
					MPRN = account.PointReferenceNumber?.ToString(),
					ContractAccount = long.Parse(command.AccountNumber),
					Description = eventAction == EventAction.LastOperationWasSuccessful
						? "Successfully entered Competition"
						: "Failed to enter Competition"
				},
			};
			return messages;
		}
	}
}
