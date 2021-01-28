using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Events;
using EI.RP.CoreServices.Http;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Events;

namespace EI.RP.DomainServices.Commands.Users.Membership.CreateAccount
{
	internal class CreateAccountCommandEventBuilder : ICommandEventBuilder<CreateAccountCommand>
	{
		private readonly IClientInfoResolver _clientInfoResolver;

		public CreateAccountCommandEventBuilder(IClientInfoResolver clientInfoResolver)
		{
			_clientInfoResolver = clientInfoResolver;
		}

		public async Task<IEventApiMessage[]> BuildEventsOnSuccess(CreateAccountCommand domainCommand)
		{
			return await BuildFor(domainCommand, EventAction.LastOperationWasSuccessful);
		}

		public async Task<IEventApiMessage[]> BuildEventsOnError(CreateAccountCommand domainCommand,
            AggregateException exceptions)
		{
			return await BuildFor(domainCommand, EventAction.LastOperationFailed);
		}

		private async Task<IEventApiMessage[]> BuildFor(CreateAccountCommand domainCommand, long eventAction)
		{
			long.TryParse(domainCommand.AccountNumber, out var accountNumber);
			return ((IEventApiMessage)new EventApiEvent(_clientInfoResolver)
			{
				CategoryId = EventCategory.User,
				ActionId = eventAction,
				Username = domainCommand.UserEmail,
				Description = "Registration - Request",
				SubCategoryId = EventSubCategory.RegistrationRequest,

				ContractAccount = accountNumber,
				MPRN = domainCommand.MPRNGPRNLast6DigitsOf
			}).ToOneItemArray();
		}
	}
}