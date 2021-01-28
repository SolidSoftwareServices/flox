using EI.RP.CoreServices.Cqrs.Events;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Http;
using EI.RP.CoreServices.Http.Session;
using EI.RP.DataModels.Events;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using System;
using System.Threading.Tasks;

namespace EI.RP.DomainServices.Commands.Premises.IncommingOccupants
{
	internal class NotifyNewIncommingOccupantCommandEventBuilder : ICommandEventBuilder<NotifyNewIncommingOccupant>
	{
		private readonly IDomainQueryResolver _queryResolver;
		private readonly IClientInfoResolver _clientInfoResolver;
		private readonly IUserSessionProvider _userSessionProvider;

		public NotifyNewIncommingOccupantCommandEventBuilder(IDomainQueryResolver queryResolver,
			IClientInfoResolver clientInfoResolver,
			IUserSessionProvider userSessionProvider)
		{
			_queryResolver = queryResolver;
			_clientInfoResolver = clientInfoResolver;
			_userSessionProvider = userSessionProvider;
		}

		public Task<IEventApiMessage[]> BuildEventsOnSuccess(NotifyNewIncommingOccupant command)
		{
			return BuildFor(command, EventAction.LastOperationWasSuccessful);
		}

		public Task<IEventApiMessage[]> BuildEventsOnError(NotifyNewIncommingOccupant command, AggregateException exceptions)
		{
			return BuildFor(command, EventAction.LastOperationFailed);
		}

		private async Task<IEventApiMessage[]> BuildFor(NotifyNewIncommingOccupant command, long eventAction)
		{
			var account = await _queryResolver.GetAccountInfoByAccountNumber(command.AccountNumber, true);

			IEventApiMessage[] messages =
			{
				new EventApiEvent(_clientInfoResolver)
				{
					CategoryId = EventCategory.MovingHouse,
					ActionId = eventAction,
					Username = _userSessionProvider.CurrentUserClaimsPrincipal.Identity.Name,
					SubCategoryId = EventSubCategory.UpdatePremiseNotes,
					Partner = long.Parse(account.Partner),
					MPRN = account.PointReferenceNumber?.ToString(),
					ContractAccount = long.Parse(command.AccountNumber),
					Description = eventAction == EventAction.LastOperationWasSuccessful
						? "Successfully Updated PremiseNotes"
						: "Failed to update PremiseNotes"
				},
			};
			return messages;
		}
	}
}
