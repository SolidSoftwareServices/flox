using Ei.Rp.DomainModels.Contracts.Accounts;
using EI.RP.CoreServices.Cqrs.Events;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Http;
using EI.RP.CoreServices.Http.Session;
using EI.RP.DataModels.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EI.RP.DomainServices.Commands.LongRunningFlows.MovingHouse
{
	internal class RecordProgressCommandEventBuilder : ICommandEventBuilder<RecordMovingOutProgress>, ICommandEventBuilder<RecordMovingHomePrns>, ICommandEventBuilder<RecordMovingInProgress>
	{
		private readonly IDomainQueryResolver _queryResolver;
		private readonly IClientInfoResolver _clientInfoResolver;
		private readonly IUserSessionProvider _userSessionProvider;

		public RecordProgressCommandEventBuilder(IDomainQueryResolver queryResolver,
			IClientInfoResolver clientInfoResolver,
			IUserSessionProvider userSessionProvider)
		{
			_queryResolver = queryResolver;
			_clientInfoResolver = clientInfoResolver;
			_userSessionProvider = userSessionProvider;
		}

		public Task<IEventApiMessage[]> BuildEventsOnSuccess(RecordMovingOutProgress command)
		{
			return BuildFor(command.ElectricityAccount(), command.GasAccount(), command, EventAction.LastOperationWasSuccessful);
		}

		public Task<IEventApiMessage[]> BuildEventsOnError(RecordMovingOutProgress command, AggregateException exceptions)
		{
			return BuildFor(command.ElectricityAccount(), command.GasAccount(), command, EventAction.LastOperationFailed);
		}

		public Task<IEventApiMessage[]> BuildEventsOnSuccess(RecordMovingHomePrns command)
		{
			return BuildFor(command.ElectricityAccount(), command.GasAccount(), command, EventAction.LastOperationWasSuccessful);
		}

		public Task<IEventApiMessage[]> BuildEventsOnError(RecordMovingHomePrns command, AggregateException exceptions)
		{
			return BuildFor(command.ElectricityAccount(), command.GasAccount(), command, EventAction.LastOperationFailed);
		}

		public Task<IEventApiMessage[]> BuildEventsOnSuccess(RecordMovingInProgress command)
		{
			return BuildFor(command.ElectricityAccount(), command.GasAccount(), command, EventAction.LastOperationWasSuccessful);
		}

		public Task<IEventApiMessage[]> BuildEventsOnError(RecordMovingInProgress command, AggregateException exceptions)
		{
			return BuildFor(command.ElectricityAccount(), command.GasAccount(), command, EventAction.LastOperationFailed);
		}

		private async Task<IEventApiMessage[]> BuildFor<TCommand>(AccountInfo electricityAccount, AccountInfo gasAccount, TCommand command, long eventAction) where TCommand : RecordMovingHomeProgress
		{
			var messages = new List<IEventApiMessage>();
			if (electricityAccount != null)
			{
				messages.Add(CreateEvent(electricityAccount));
			}
			if (gasAccount != null)
			{
				messages.Add(CreateEvent(gasAccount));
			}
			return messages.ToArray();

			EventApiEvent CreateEvent(AccountInfo account) =>
				new EventApiEvent(_clientInfoResolver)
				{
					CategoryId = EventCategory.MovingHouse,
					ActionId = eventAction,
					Username = _userSessionProvider.CurrentUserClaimsPrincipal.Identity.Name,
					SubCategoryId = GetEventSubCategory(command),
					Partner = long.Parse(account.Partner),
					MPRN = account.PointReferenceNumber?.ToString(),
					ContractAccount = long.Parse(account.AccountNumber),
					Description = GetEventDescription(command, eventAction)
				};
		}

		private long GetEventSubCategory<TCommand>(TCommand command) where TCommand : RecordMovingHomeProgress
		{
			switch (command)
			{
				case RecordMovingHomePrns _:
					return EventSubCategory.RecordMovingHomePrns;
				case RecordMovingInProgress _:
					return EventSubCategory.RecordMovingInProgress;
				case RecordMovingOutProgress _:
					return EventSubCategory.RecordMovingOutProgress;
				default:
					throw new ArgumentException($"{command} is not supported.");
			}
		}

		private string GetEventDescription<TCommand>(TCommand command, long eventAction) where TCommand : RecordMovingHomeProgress
		{
			string action;
			switch (command)
			{
				case RecordMovingHomePrns _:
					action = "MovingHomePrns";
					break;
				case RecordMovingInProgress _:
					action = "MovingInProgress";
					break;
				case RecordMovingOutProgress _:
					action = "MovingOutProgress";
					break;
				default:
					throw new ArgumentException($"{command} is not supported.");
			}

			return eventAction == EventAction.LastOperationWasSuccessful
				? $"Successfully recorded {action}"
				: $"Failed to record {action}";
		}
	}
}
