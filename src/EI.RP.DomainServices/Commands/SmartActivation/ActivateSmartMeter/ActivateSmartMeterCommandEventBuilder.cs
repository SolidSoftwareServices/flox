using System;
using System.Collections.Generic;
using System.Text;
using System;
using EI.RP.CoreServices.Cqrs.Events;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Http;
using EI.RP.CoreServices.Http.Session;
using EI.RP.DataModels.Events;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using System.Threading.Tasks;

namespace EI.RP.DomainServices.Commands.SmartActivation.ActivateSmartMeter
{
	class ActivateSmartMeterCommandEventBuilder : ICommandEventBuilder<ActivateSmartMeterCommand>
	{
		private readonly IDomainQueryResolver _queryResolver;
		private readonly IClientInfoResolver _clientInfoResolver;
		private readonly IUserSessionProvider _userSessionProvider;

		public ActivateSmartMeterCommandEventBuilder(IDomainQueryResolver queryResolver,
			IClientInfoResolver clientInfoResolver,
			IUserSessionProvider userSessionProvider)
		{
			_queryResolver = queryResolver;
			_clientInfoResolver = clientInfoResolver;
			_userSessionProvider = userSessionProvider;
		}

		public Task<IEventApiMessage[]> BuildEventsOnSuccess(ActivateSmartMeterCommand command)
		{
			return BuildFor(command, EventAction.LastOperationWasSuccessful);
		}

		public Task<IEventApiMessage[]> BuildEventsOnError(ActivateSmartMeterCommand command, AggregateException exceptions)
		{
			return BuildFor(command, EventAction.LastOperationFailed);
		}

		private async Task<IEventApiMessage[]> BuildFor(ActivateSmartMeterCommand command, long eventAction)
		{
			var account = await _queryResolver.GetAccountInfoByAccountNumber(command.ElectricityAccountNumber, true);

			IEventApiMessage[] messages =
			{
				new EventApiEvent(_clientInfoResolver)
				{
					CategoryId = EventCategory.SmartActivation,
					ActionId = eventAction,
					Username = _userSessionProvider.CurrentUserClaimsPrincipal.Identity.Name,
					SubCategoryId = EventSubCategory.SmartMeterActivation,
					Partner = long.Parse(account.Partner),
					MPRN = account.PointReferenceNumber?.ToString(),
					ContractAccount = long.Parse(command.ElectricityAccountNumber),
					Description = eventAction == EventAction.LastOperationWasSuccessful
						? "Successfully activated SmartMeter"
						: "Failed to activate SmartMeter"
				},
			};
			return messages;
		}
	}
}


