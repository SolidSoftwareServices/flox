using EI.RP.CoreServices.Cqrs.Events;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Http;
using EI.RP.CoreServices.Http.Session;
using EI.RP.DataModels.Events;
using System;
using System.Threading.Tasks;
using EI.RP.DomainServices.Queries.Contracts.Accounts;

namespace EI.RP.DomainServices.Commands.Users.SmartActivationNotification.DismissSmartActivationNotification
{
	internal class DismissSmartActivationNotificationCommandEventBuilder : ICommandEventBuilder<DismissSmartActivationNotificationCommand>
    {
	    private readonly IClientInfoResolver _clientInfoResolver;
	    private readonly IDomainQueryResolver _queryResolver;
	    private readonly IUserSessionProvider _userSessionProvider;

	    public DismissSmartActivationNotificationCommandEventBuilder(IClientInfoResolver clientInfoResolver,
		    IUserSessionProvider userSessionProvider, IDomainQueryResolver queryResolver)
	    {
		    _clientInfoResolver = clientInfoResolver;
		    _userSessionProvider = userSessionProvider;
		    _queryResolver = queryResolver;
	    }

	    public async Task<IEventApiMessage[]> BuildEventsOnSuccess(DismissSmartActivationNotificationCommand command)
	    {
		    return await BuildFor(command, EventAction.LastOperationWasSuccessful);
	    }

	    public async Task<IEventApiMessage[]> BuildEventsOnError(DismissSmartActivationNotificationCommand command,
		    AggregateException exceptions)
	    {
		    return await BuildFor(command, EventAction.LastOperationFailed);
	    }


	    private async Task<IEventApiMessage[]> BuildFor(DismissSmartActivationNotificationCommand command, long eventAction)
	    {
		    var account = await _queryResolver.GetAccountInfoByAccountNumber(command.AccountNumber, true);

		    IEventApiMessage[] messages =
		    {
			    new EventApiEvent(_clientInfoResolver)
			    {
				    CategoryId = EventCategory.Change,
				    ActionId = eventAction,
				    Username = _userSessionProvider.CurrentUserClaimsPrincipal.Identity.Name,
				    SubCategoryId = EventSubCategory.DismissSmartActivationNotification,
				    Partner = long.Parse(account.Partner),
					MPRN = account.PointReferenceNumber.ToString(),
				    ContractAccount = long.Parse(command.AccountNumber),
				    Description = eventAction == EventAction.LastOperationWasSuccessful
					    ? "Success: Dismissed Smart Activation Notification"
					    : "Failed to dismissed Smart Activation Notification"
				},
		    };
		    return messages;
	    }
    }
}