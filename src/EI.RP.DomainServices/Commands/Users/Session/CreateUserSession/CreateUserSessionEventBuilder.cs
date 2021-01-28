using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Events;
using EI.RP.CoreServices.Http;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Events;

namespace EI.RP.DomainServices.Commands.Users.Session.CreateUserSession
{
	internal class CreateUserSessionEventBuilder : ICommandEventBuilder<CreateUserSessionCommand>
	{
		private readonly IClientInfoResolver _clientInfoResolver;

		public CreateUserSessionEventBuilder(IClientInfoResolver clientInfoResolver)
		{
			_clientInfoResolver = clientInfoResolver;
		}

		public async Task<IEventApiMessage[]> BuildEventsOnSuccess(CreateUserSessionCommand command)
		{
			return await BuildFor(command, EventAction.LastOperationWasSuccessful);
		}

		public async Task<IEventApiMessage[]> BuildEventsOnError(CreateUserSessionCommand command,
            AggregateException exceptions)
		{
			return await BuildFor(command, EventAction.LastOperationFailed);
		}

		private async Task<IEventApiMessage[]> BuildFor(CreateUserSessionCommand domainCommand, long eventAction)
		{
			return ((IEventApiMessage)new EventApiEvent(_clientInfoResolver)
			{
				CategoryId = EventCategory.User,
				ActionId = eventAction,
				Username = domainCommand.UserEmail,
				Description = "Login",
				SubCategoryId = EventSubCategory.LoginRequest
			}).ToOneItemArray();
		}
	}
}