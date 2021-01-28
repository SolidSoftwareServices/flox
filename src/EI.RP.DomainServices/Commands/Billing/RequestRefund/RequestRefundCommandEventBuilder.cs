using EI.RP.CoreServices.Cqrs.Events;
using EI.RP.CoreServices.Http;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Events;
using System;
using System.Threading.Tasks;

namespace EI.RP.DomainServices.Commands.Billing.RequestRefund
{
    internal class RequestRefundCommandEventBuilder : ICommandEventBuilder<RequestRefundCommand>
    {
        private readonly IClientInfoResolver _clientInfoResolver;
        private readonly IUserSessionProvider _userSessionProvider;
        public RequestRefundCommandEventBuilder(IClientInfoResolver clientInfoResolver, IUserSessionProvider userSessionProvider)
        {
            _clientInfoResolver = clientInfoResolver;
            _userSessionProvider = userSessionProvider;
        }

        private async Task<IEventApiMessage[]> BuildFor(RequestRefundCommand command, long eventAction)
        {
            return ((IEventApiMessage)new EventApiEvent(_clientInfoResolver)
            {
                CategoryId = 160,
                Description = eventAction == EventAction.LastOperationWasSuccessful ? "Log Account Enquiry Request Successfully" : "Failed to Log Account Enquiry Request",
                ActionId = eventAction,
                Username = _userSessionProvider.CurrentUserClaimsPrincipal.Identity.Name,
                SubCategoryId = EventSubCategory.RefundRequest,
                Partner = long.Parse(command.Partner),
                ContractAccount = long.Parse(command.AccountNumber),
            }).ToOneItemArray();
        }

        public async Task<IEventApiMessage[]> BuildEventsOnSuccess(RequestRefundCommand command)
        {
            return await BuildFor(command, EventAction.LastOperationWasSuccessful);
        }

        public async Task<IEventApiMessage[]> BuildEventsOnError(RequestRefundCommand command, AggregateException exceptions)
        {
            return await BuildFor(command, EventAction.LastOperationFailed);
        }
    }
}