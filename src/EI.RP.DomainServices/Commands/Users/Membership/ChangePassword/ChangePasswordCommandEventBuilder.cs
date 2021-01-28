using System;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Events;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.CoreServices.Http;
using EI.RP.CoreServices.Http.Session;
using EI.RP.DataModels.Events;
using Ei.Rp.DomainErrors;

namespace EI.RP.DomainServices.Commands.Users.Membership.ChangePassword
{
	internal class ChangePasswordCommandEventBuilder : ICommandEventBuilder<ChangePasswordCommand>
	{
		private readonly IClientInfoResolver _clientInfoResolver;
		private readonly IUserSessionProvider _userSessionProvider;

		public ChangePasswordCommandEventBuilder(IClientInfoResolver clientInfoResolver,
			IUserSessionProvider userSessionProvider)
		{
			_clientInfoResolver = clientInfoResolver;
			_userSessionProvider = userSessionProvider;
		}

		public async Task<IEventApiMessage[]> BuildEventsOnSuccess(ChangePasswordCommand domainCommand)
		{
			return await BuildFor(domainCommand, EventAction.LastOperationWasSuccessful, null);
		}

		public async Task<IEventApiMessage[]> BuildEventsOnError(ChangePasswordCommand domainCommand,
            AggregateException exceptions)
		{
			return await BuildFor(domainCommand, EventAction.LastOperationFailed, exceptions);
		}

		private async Task<IEventApiMessage[]> BuildFor(ChangePasswordCommand domainCommand, long eventAction, AggregateException exceptions)
		{
		
            IEventApiMessage[] messages = {
                new EventApiEvent(_clientInfoResolver)
                {
                    CategoryId = EventCategory.User,
                    ActionId = eventAction,
                    Description = eventAction == EventAction.LastOperationWasSuccessful ? "Change Password Successfully" : GetErrorDescription(),
                    SubCategoryId = EventSubCategory.ChangePasswordId,
                    Username = _userSessionProvider.CurrentUserClaimsPrincipal.Identity.Name,
                    Partner = 0,
                    ContractAccount=0
                }                
            };
            return messages;

            string GetErrorDescription()
            {
	            var msg = string.Join(Environment.NewLine,
		            exceptions?.InnerExceptions.Select(x => (x as DomainException)?.DomainError.ErrorMessage ?? x.Message)??new string[0]);
	            return msg.Substring(0,Math.Min(50,msg.Length));
            }
		}
	}
}