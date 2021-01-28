using System;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Events;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Http;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Events;
using EI.RP.DomainServices.Queries.Contracts.Accounts;

namespace EI.RP.DomainServices.Commands.Contracts.AddAdditionalAccount.AdditionalAccount
{
	internal class AddAdditionalAccountCommandEventBuilder : ICommandEventBuilder<AddAdditionalAccountCommand>
	{
		private readonly IUserSessionProvider _userSessionProvider;
		private readonly IClientInfoResolver _clientInfoResolver;
		private readonly IDomainQueryResolver _queryResolver;

		public AddAdditionalAccountCommandEventBuilder(IUserSessionProvider userSessionProvider,
			IClientInfoResolver clientInfoResolver,
			IDomainQueryResolver queryResolver)
		{
			_userSessionProvider = userSessionProvider;
			_clientInfoResolver = clientInfoResolver;
			_queryResolver = queryResolver;
		}

		public Task<IEventApiMessage[]> BuildEventsOnSuccess(AddAdditionalAccountCommand command)
		{
			return BuildFor(command, EventAction.LastOperationWasSuccessful);
		}

		public Task<IEventApiMessage[]> BuildEventsOnError(AddAdditionalAccountCommand command, AggregateException exceptions)
		{
			return BuildFor(command, EventAction.LastOperationFailed);
		}

		private async Task<IEventApiMessage[]> BuildFor(AddAdditionalAccountCommand command, long eventAction)
		{
			var account = (await _queryResolver.GetAccountInfoByBusinessPartner(command.Partner, true)).SingleOrDefault();

			return ((IEventApiMessage)new EventApiEvent(_clientInfoResolver)
			{
				CategoryId = EventCategory.AddAccounts,
				ActionId = eventAction,
				Username = _userSessionProvider.CurrentUserClaimsPrincipal.Identity.Name,
				SubCategoryId = EventSubCategory.AddAdditionalAccount,
				Partner = long.Parse(account.Partner),
				MPRN = account.PointReferenceNumber.ToString(),
				ContractAccount = long.Parse(command.AccountNumber),
				Description = (eventAction == EventAction.LastOperationWasSuccessful ?
					"Requested Add Additional Account" : "Failed to request Add Additional Account")
			}).ToOneItemArray();
		}
	}
}
