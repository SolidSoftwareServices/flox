using System;
using EI.RP.CoreServices.Cqrs.Events;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Http;
using EI.RP.CoreServices.Http.Session;
using EI.RP.DataModels.Events;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using System.Threading.Tasks;

namespace EI.RP.DomainServices.Commands.Contracts.ChangeSmartPlanToStandard
{
	class ChangeSmartPlanToStandardCommandEventBuilder : ICommandEventBuilder<ChangeSmartPlanToStandardCommand>
	{
		private readonly IDomainQueryResolver _queryResolver;
		private readonly IClientInfoResolver _clientInfoResolver;
		private readonly IUserSessionProvider _userSessionProvider;

		public ChangeSmartPlanToStandardCommandEventBuilder(IDomainQueryResolver queryResolver,
			IClientInfoResolver clientInfoResolver,
			IUserSessionProvider userSessionProvider)
		{
			_queryResolver = queryResolver;
			_clientInfoResolver = clientInfoResolver;
			_userSessionProvider = userSessionProvider;
		}

		public Task<IEventApiMessage[]> BuildEventsOnSuccess(ChangeSmartPlanToStandardCommand command)
		{
			return BuildFor(command, EventAction.LastOperationWasSuccessful);
		}

		public Task<IEventApiMessage[]> BuildEventsOnError(ChangeSmartPlanToStandardCommand command, AggregateException exceptions)
		{
			return BuildFor(command, EventAction.LastOperationFailed);
		}

		private async Task<IEventApiMessage[]> BuildFor(ChangeSmartPlanToStandardCommand command, long eventAction)
		{
			var account = await _queryResolver.GetAccountInfoByAccountNumber(command.ElectricityAccountNumber, true);

			IEventApiMessage[] messages =
			{
				new EventApiEvent(_clientInfoResolver)
				{
					CategoryId = EventCategory.Change,
					ActionId = eventAction,
					Username = _userSessionProvider.CurrentUserClaimsPrincipal.Identity.Name,
					SubCategoryId = EventSubCategory.ChangeSmartPlanToStandard,
					Partner = long.Parse(account.Partner),
					MPRN = account.PointReferenceNumber?.ToString(),
					ContractAccount = long.Parse(command.ElectricityAccountNumber),
					Description = eventAction == EventAction.LastOperationWasSuccessful
						? "Successfully changed Smart Plan to Standard"
						: "Failed to change Smart Plan to Standard"
				},
			};
			return messages;
		}
	}
}

