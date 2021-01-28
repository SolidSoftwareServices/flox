using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Events;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Http;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Events;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Queries.Contracts.Accounts;

namespace EI.RP.DomainServices.Commands.Billing.ChangePaperBillChoice
{
	internal class ChangePaperBillCommandEventBuilder : ICommandEventBuilder<ChangePaperBillChoiceCommand>
	{
		private readonly IClientInfoResolver _clientInfoResolver;
		private readonly IDomainQueryResolver _queryResolver;
		private readonly IUserSessionProvider _userSessionProvider;

		public ChangePaperBillCommandEventBuilder(IClientInfoResolver clientInfoResolver,
			IUserSessionProvider userSessionProvider, IDomainQueryResolver queryResolver)
		{
			_clientInfoResolver = clientInfoResolver;
			_userSessionProvider = userSessionProvider;
			_queryResolver = queryResolver;
		}

		public async Task<IEventApiMessage[]> BuildEventsOnSuccess(ChangePaperBillChoiceCommand command)
		{
			return await BuildFor(command, EventAction.LastOperationWasSuccessful);
		}

		public async Task<IEventApiMessage[]> BuildEventsOnError(ChangePaperBillChoiceCommand command,
			AggregateException exceptions)
		{
			return await BuildFor(command, EventAction.LastOperationFailed);
		}


		private async Task<IEventApiMessage[]> BuildFor(ChangePaperBillChoiceCommand command, long eventAction)
		{
			var account = await _queryResolver.GetAccountInfoByAccountNumber(command.AccountNumber, true);

			return ((IEventApiMessage) new EventApiEvent(_clientInfoResolver)
			{
				CategoryId = EventCategory.Change,
				ActionId = eventAction,
				Username = _userSessionProvider.CurrentUserClaimsPrincipal.Identity.Name,
				SubCategoryId = command.NewChoice == PaperBillChoice.On
					? EventSubCategory.PaperBillOn
					: EventSubCategory.PaperBillOff,
				Partner = long.Parse(account.Partner),
				ContractAccount = long.Parse(command.AccountNumber)
			}).ToOneItemArray();
		}
	}
}