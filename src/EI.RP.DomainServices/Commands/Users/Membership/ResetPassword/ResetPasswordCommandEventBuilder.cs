using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Events;
using EI.RP.CoreServices.Http;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Events;

namespace EI.RP.DomainServices.Commands.Users.Membership.ResetPassword
{
	internal class ResetPasswordCommandEventBuilder : ICommandEventBuilder<ResetPasswordDomainCommand>
	{
		private readonly IClientInfoResolver _clientInfoResolver;

		public ResetPasswordCommandEventBuilder(IClientInfoResolver clientInfoResolver)
		{
			_clientInfoResolver = clientInfoResolver;
		}

		public async Task<IEventApiMessage[]> BuildEventsOnSuccess(ResetPasswordDomainCommand domainCommand)
		{
			return await BuildFor(domainCommand, EventAction.LastOperationWasSuccessful);
		}

		public async Task<IEventApiMessage[]> BuildEventsOnError(ResetPasswordDomainCommand domainCommand,
            AggregateException exceptions)
		{
			return await BuildFor(domainCommand, EventAction.LastOperationFailed);
		}

		private async Task<IEventApiMessage[]> BuildFor(ResetPasswordDomainCommand domainCommand, long eventAction)
		{

            IEventApiMessage[] messages = {
                new EventApiEvent(_clientInfoResolver)
                {
                    CategoryId = EventCategory.User,
                    ActionId = eventAction,
                    Username = domainCommand.Email,
                    SubCategoryId = EventSubCategory.ResetPassword,
                    Partner = 0,
                    ContractAccount=0
                },
                new EventApiEvent(_clientInfoResolver)
                {
                    CategoryId = EventCategory.User,
                    ActionId = eventAction,
                    Username = domainCommand.Email,
                    SubCategoryId = EventSubCategory.ForgetPasswordCompleteId,
                    Partner = 0,
                    ContractAccount=0
                }

            };
            return messages;
        }
	}
}