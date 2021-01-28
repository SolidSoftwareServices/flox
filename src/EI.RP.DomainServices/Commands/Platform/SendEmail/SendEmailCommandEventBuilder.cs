using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Events;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Http;
using EI.RP.CoreServices.Http.Session;
using EI.RP.DataModels.Events;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Queries.Contracts.Accounts;

namespace EI.RP.DomainServices.Commands.Platform.SendEmail
{
    internal class SendEmailCommandEventBuilder : ICommandEventBuilder<SendEmailCommand>
    {
	    private readonly IDomainQueryResolver _queryResolver;
	    private readonly IClientInfoResolver _clientInfoResolver;
	    private readonly IUserSessionProvider _userSessionProvider;

	    public SendEmailCommandEventBuilder(IDomainQueryResolver queryResolver,
		    IClientInfoResolver clientInfoResolver,
		    IUserSessionProvider userSessionProvider)
	    {
		    _queryResolver = queryResolver;
		    _clientInfoResolver = clientInfoResolver;
		    _userSessionProvider = userSessionProvider;
	    }


		public Task<IEventApiMessage[]> BuildEventsOnSuccess(SendEmailCommand command)
	    {
			return BuildFor(command, EventAction.LastOperationWasSuccessful);
		}

	    public Task<IEventApiMessage[]> BuildEventsOnError(SendEmailCommand command, AggregateException exceptions)
	    {
			return BuildFor(command, EventAction.LastOperationFailed);
	    }

	    private async Task<IEventApiMessage[]> BuildFor(SendEmailCommand command, long eventAction)
	    {
		    var account = await _queryResolver.GetAccountInfoByAccountNumber(command.AccountNumber, true);

		    IEventApiMessage[] messages =
		    {
			    new EventApiEvent(_clientInfoResolver)
			    {
				    CategoryId = EventCategory.SendEmail,
				    ActionId = eventAction,
				    Username = _userSessionProvider.CurrentUserClaimsPrincipal.Identity.Name,
				    SubCategoryId = GetSubCategory(),
				    Partner = long.Parse(account.Partner),
				    MPRN = account.PointReferenceNumber?.ToString(),
				    ContractAccount = long.Parse(command.AccountNumber),
				    Description = eventAction == EventAction.LastOperationWasSuccessful
					    ? "Successfully Sent Email"
					    : "Failed to Send Email"
				},
		    };
		    return messages;

		    long GetSubCategory()
		    {
			    if (command.QueryType == ContactQueryType.AddAdditionalAccount)
				    return EventSubCategory.AddAnAdditionalAccountMail;
			    if (command.QueryType == ContactQueryType.BillOrPayment)
				    return EventSubCategory.BillOrPaymentQueryMail;
			    if (command.QueryType == ContactQueryType.MeterRead)
				    return EventSubCategory.MeterReadQueryMail;
			    if (command.QueryType == ContactQueryType.Other)
				    return EventSubCategory.OtherMail;
			    
				throw new ArgumentException($"command.QueryType: {command.QueryType} not supported");
		    }
		}
    }
}
