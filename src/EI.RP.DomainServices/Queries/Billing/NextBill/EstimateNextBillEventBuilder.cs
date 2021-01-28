using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Events;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Http;
using EI.RP.CoreServices.Http.Session;
using EI.RP.DataModels.Events;
using EI.RP.DomainServices.Queries.Contracts.Accounts;

namespace EI.RP.DomainServices.Queries.Billing.NextBill
{
	internal class EstimateNextBillEventBuilder : IQueryEventBuilder<EstimateNextBillQuery>
	{
		private readonly IClientInfoResolver _clientInfoResolver;
		private readonly IDomainQueryResolver _queryResolver;
		private readonly IUserSessionProvider _userSessionProvider;

		public EstimateNextBillEventBuilder(IClientInfoResolver clientInfoResolver,
			IUserSessionProvider userSessionProvider,
			IDomainQueryResolver queryResolver)
		{
			_clientInfoResolver = clientInfoResolver;
			_userSessionProvider = userSessionProvider;
			_queryResolver = queryResolver;
		}

		public async Task<IEventApiMessage> BuildEventOnSuccess(EstimateNextBillQuery query)
		{
			return await BuildFor(query, EventAction.LastOperationWasSuccessful);
		}

		public async Task<IEventApiMessage> BuildEventOnError(EstimateNextBillQuery command,
			AggregateException exceptions)
		{
			return null;
		}


		private async Task<EventApiEvent> BuildFor(EstimateNextBillQuery command, long eventAction)
		{
			var account = await _queryResolver.GetAccountInfoByAccountNumber(command.AccountNumber, true);

			return new EventApiEvent(_clientInfoResolver)
			{
				CategoryId = EventCategory.PaymentResult,
				ActionId = eventAction,
				Username = _userSessionProvider.CurrentUserClaimsPrincipal.Identity.Name,
				SubCategoryId = EventSubCategory.CalculateEstimationForNextBill,
				Partner = long.Parse(account.Partner),
				ContractAccount = long.Parse(command.AccountNumber)
			};
		}
	}
}