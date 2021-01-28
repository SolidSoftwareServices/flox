using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Events;
using EI.RP.CoreServices.Http;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Events;

namespace EI.RP.DomainServices.Commands.Users.Membership.CreatePassword
{
	internal class CreatePasswordCommandEventBuilder : ICommandEventBuilder<CreatePasswordCommand>
	{
		private readonly IClientInfoResolver _clientInfoResolver;

		public CreatePasswordCommandEventBuilder(IClientInfoResolver clientInfoResolver)
		{
			_clientInfoResolver = clientInfoResolver;
		}

		public async Task<IEventApiMessage[]> BuildEventsOnSuccess(CreatePasswordCommand domainCommand)
		{
			return await BuildFor(domainCommand, EventAction.LastOperationWasSuccessful);
		}

		public async Task<IEventApiMessage[]> BuildEventsOnError(CreatePasswordCommand domainCommand,
            AggregateException exceptions)
		{
			return await BuildFor(domainCommand, EventAction.LastOperationFailed);
		}

		private async Task<IEventApiMessage[]> BuildFor(CreatePasswordCommand domainCommand, long eventAction)
		{
			return ((IEventApiMessage)new EventApiEvent(_clientInfoResolver)
			{
				CategoryId = EventCategory.User,
				ActionId = eventAction,
				Description = "Create Password",
				SubCategoryId = EventSubCategory.CreatePasswordRequest
			}).ToOneItemArray();
		}
	}
}