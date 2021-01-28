using EI.RP.CoreServices.Cqrs.Events;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Http;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Events;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using System;
using System.Threading.Tasks;

namespace EI.RP.DomainServices.Commands.Billing.ChangeBillingPeriod
{
	internal class SetMonthlyBillingPeriodCommandEventBuilder : ICommandEventBuilder<SetMonthlyBillingPeriodCommand>
	{
		private readonly IUserSessionProvider _userSessionProvider;
		private readonly IClientInfoResolver _clientInfoResolver;
		private readonly IDomainQueryResolver _queryResolver;

		public SetMonthlyBillingPeriodCommandEventBuilder(IUserSessionProvider userSessionProvider,
			IClientInfoResolver clientInfoResolver,
			IDomainQueryResolver queryResolver)
		{
			_userSessionProvider = userSessionProvider;
			_clientInfoResolver = clientInfoResolver;
			_queryResolver = queryResolver;
		}

		public Task<IEventApiMessage[]> BuildEventsOnSuccess(SetMonthlyBillingPeriodCommand command)
		{
			return BuildFor(command, EventAction.LastOperationWasSuccessful);
		}

		public Task<IEventApiMessage[]> BuildEventsOnError(SetMonthlyBillingPeriodCommand command, AggregateException exceptions)
		{
			return BuildFor(command, EventAction.LastOperationWasSuccessful);
		}

		private async Task<IEventApiMessage[]> BuildFor(SetMonthlyBillingPeriodCommand command, long eventAction)
		{
			var account = await _queryResolver.GetAccountInfoByAccountNumber(command.AccountNumber, true);

			return ((IEventApiMessage)new EventApiEvent(_clientInfoResolver)
			{
				CategoryId = EventCategory.Change,
				ActionId = eventAction,
				Username = _userSessionProvider.CurrentUserClaimsPrincipal.Identity.Name,
				SubCategoryId = EventSubCategory.MonthlyBillingPeriod,
				Partner = long.Parse(account.Partner),
				MPRN = account.PointReferenceNumber.ToString(),
				ContractAccount = long.Parse(command.AccountNumber),
				Description = $"Billing day of the month {command.DayOfTheMonth}"
			}).ToOneItemArray();
		}
	}
}
