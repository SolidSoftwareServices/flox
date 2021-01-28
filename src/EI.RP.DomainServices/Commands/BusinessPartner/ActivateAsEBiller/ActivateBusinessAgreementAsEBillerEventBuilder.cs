using EI.RP.CoreServices.Cqrs.Events;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Http;
using EI.RP.CoreServices.Http.Session;
using EI.RP.DataModels.Events;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using System;
using System.Threading.Tasks;

namespace EI.RP.DomainServices.Commands.BusinessPartner.ActivateAsEBiller
{
	internal class ActivateBusinessAgreementAsEBillerEventBuilder : ICommandEventBuilder<ActivateBusinessAgreementAsEBillerCommand>
    {
	    private readonly IDomainQueryResolver _queryResolver;
	    private readonly IClientInfoResolver _clientInfoResolver;
	    private readonly IUserSessionProvider _userSessionProvider;

	    public ActivateBusinessAgreementAsEBillerEventBuilder(IDomainQueryResolver queryResolver,
		    IClientInfoResolver clientInfoResolver,
		    IUserSessionProvider userSessionProvider)
	    {
		    _queryResolver = queryResolver;
		    _clientInfoResolver = clientInfoResolver;
		    _userSessionProvider = userSessionProvider;
	    }

		public Task<IEventApiMessage[]> BuildEventsOnSuccess(ActivateBusinessAgreementAsEBillerCommand command)
	    {
		    return BuildFor(command, EventAction.LastOperationWasSuccessful);
	    }

	    public Task<IEventApiMessage[]> BuildEventsOnError(ActivateBusinessAgreementAsEBillerCommand command, AggregateException exceptions)
	    {
			return BuildFor(command, EventAction.LastOperationFailed);
		}

	    private async Task<IEventApiMessage[]> BuildFor(ActivateBusinessAgreementAsEBillerCommand command, long eventAction)
	    {
			var account = await _queryResolver.GetAccountInfoByAccountNumber(command.BusinessAgreementID, true);

			IEventApiMessage[] messages =
			{
				new EventApiEvent(_clientInfoResolver)
				{
					CategoryId = EventCategory.Change,
					ActionId = eventAction,
					Username = _userSessionProvider.CurrentUserClaimsPrincipal.Identity.Name,
					SubCategoryId = EventSubCategory.UpdateEBiller,
					Partner = long.Parse(account.Partner),
					MPRN = account.PointReferenceNumber?.ToString(),
					ContractAccount = long.Parse(account.AccountNumber),
					Description = eventAction == EventAction.LastOperationWasSuccessful
						? "Successfully Updated EBiller flag"
						: "Failed to update EBiller flag"
				},
			};
			return messages;
		}
    }
}
