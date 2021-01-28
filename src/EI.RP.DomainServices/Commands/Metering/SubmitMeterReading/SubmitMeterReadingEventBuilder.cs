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

namespace EI.RP.DomainServices.Commands.Metering.SubmitMeterReading
{
	internal class SubmitMeterReadingEventBuilder : ICommandEventBuilder<SubmitMeterReadingCommand>
	{
		private readonly IClientInfoResolver _clientInfoResolver;
		private readonly IUserSessionProvider _userSessionProvider;
		private readonly IDomainQueryResolver _queryResolver;

		public SubmitMeterReadingEventBuilder(IClientInfoResolver clientInfoResolver,
			IUserSessionProvider userSessionProvider,IDomainQueryResolver queryResolver)
		{
			_clientInfoResolver = clientInfoResolver;
			_userSessionProvider = userSessionProvider;
			_queryResolver = queryResolver;
		}

		public async Task<IEventApiMessage[]> BuildEventsOnSuccess(SubmitMeterReadingCommand command)
		{
			return await BuildFor(command, EventAction.LastOperationWasSuccessful);
		}

		public async Task<IEventApiMessage[]> BuildEventsOnError(SubmitMeterReadingCommand command,
            AggregateException exceptions)
		{
			return await BuildFor(command, EventAction.LastOperationFailed);
		}

		private async Task<IEventApiMessage[]> BuildFor(SubmitMeterReadingCommand command, long eventAction)
		{
			var account = await _queryResolver.GetAccountInfoByAccountNumber(command.AccountNumber, byPassPipeline:true);

			return ((IEventApiMessage)new EventApiEvent(_clientInfoResolver)
			{
				CategoryId = EventCategory.Change,
				ActionId = eventAction,
				Username = _userSessionProvider.CurrentUserClaimsPrincipal.Identity.Name,
				Description = $"{string.Join(" ", command.MeterReadingDataResults.Select(x=>x.MeterReading))}",
				SubCategoryId =  EventSubCategory.EnterMeterReadingEventId,
				Partner = long.Parse(account.Partner),
				ContractAccount = long.Parse(command.AccountNumber),
			}).ToOneItemArray();
		}
	}
}