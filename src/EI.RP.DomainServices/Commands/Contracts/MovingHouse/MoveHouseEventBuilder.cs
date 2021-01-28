using System;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Events;
using EI.RP.CoreServices.Http;
using EI.RP.CoreServices.Http.Session;
using EI.RP.DataModels.Events;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;

namespace EI.RP.DomainServices.Commands.Contracts.MovingHouse
{
	internal class MoveHouseEventBuilder : ICommandEventBuilder<MoveHouse>
	{
		private readonly IClientInfoResolver _clientInfoResolver;
		private readonly IUserSessionProvider _userSessionProvider;

		public MoveHouseEventBuilder(IClientInfoResolver clientInfoResolver,
			IUserSessionProvider userSessionProvider)
		{
			_clientInfoResolver = clientInfoResolver;
			_userSessionProvider = userSessionProvider;
		}

		public async Task<IEventApiMessage[]> BuildEventsOnSuccess(MoveHouse command)
		{
			return await BuildFor(command, EventAction.LastOperationWasSuccessful);
		}

		public async Task<IEventApiMessage[]> BuildEventsOnError(MoveHouse command,
			AggregateException exceptions)
		{
			return await BuildFor(command, EventAction.LastOperationFailed,
				exceptions.InnerExceptions.FirstOrDefault());
		}


		private async Task<IEventApiMessage[]> BuildFor(MoveHouse command, long eventAction, Exception ex = null)
		{
			IEventApiMessage[] result;
			if (eventAction == EventAction.LastOperationWasSuccessful)
				//TODO: SUCCESS event not found in existing code
				result = new IEventApiMessage[0];
			else if (!command.Context.CheckPoints.StoreNewIncommingOccupantCompleted_1)
				result = new[]
				{
					BuildEvent(command.Context.Electricity?.Account, EventSubCategory.MovingHouseCloseElec, ex?.Message,
						command.Context.NewPrns.NewMprn),
					BuildEvent(command.Context.Gas?.Account, EventSubCategory.MovingHouseCloseGas, ex?.Message,
						command.Context.NewPrns.NewGprn)
				};
			else if (!command.Context.CheckPoints.SubmitMoveOutMeterReadCompleted_2)
				result = new[]
				{
					BuildEvent(command.Context.Electricity?.Account, EventSubCategory.MovingHouseCloseElec,
						"Error submitting CMeter Read for close account", command.Context.NewPrns.NewMprn),
					BuildEvent(command.Context.Gas?.Account, EventSubCategory.MovingHouseCloseGas,
						"Error submitting CMeter Read for close account", command.Context.NewPrns.NewGprn)
				};
			else if (!command.Context.CheckPoints.SubmitMoveInMeterReadCompleted_3)
				result = new[]
				{
					BuildEvent(command.Context.Electricity?.Account, EventSubCategory.MovingHouseCloseElec,
						"Error submitting CMeter Read for moveIn account", command.Context.NewPrns.NewMprn),
					BuildEvent(command.Context.Gas?.Account, EventSubCategory.MovingHouseCloseGas,
						"Error submitting CMeter Read for moveIn account", command.Context.NewPrns.NewGprn)
				};
			else if (!command.Context.CheckPoints.SetUpNewDirectDebitsCompleted_4)
				result = new[]
				{
					BuildEvent(command.Context.Electricity?.Account, EventSubCategory.MovingHouseCloseElec,
						"Error setting up new direct debit, moving house", command.Context.NewPrns.NewMprn),
					BuildEvent(command.Context.Gas?.Account, EventSubCategory.MovingHouseCloseGas,
						"Error setting up new direct debit, moving house", command.Context.NewPrns.NewGprn)
				};
			else if (!command.Context.CheckPoints.SubmitNewContractCompleted_5)
				result = new[]
				{
					BuildEvent(command.Context.Electricity?.Account, EventSubCategory.MovingHouseCloseElec, ex?.Message,
						command.Context.NewPrns.NewMprn),
					BuildEvent(command.Context.Gas?.Account, EventSubCategory.MovingHouseCloseGas, ex?.Message,
						command.Context.NewPrns.NewGprn)
				};
			else
				result = new[]
				{
					BuildEvent(command.Context.Electricity?.Account, EventSubCategory.MovingHouseCloseElec, ex?.Message,
						command.Context.NewPrns.NewMprn),
					BuildEvent(command.Context.Gas?.Account, EventSubCategory.MovingHouseCloseGas, ex?.Message,
						command.Context.NewPrns.NewGprn)
				};

			return result.Where(x => x != null).ToArray();

			IEventApiMessage BuildEvent(AccountInfo account, long subcategory, string description = null,
				PointReferenceNumber prn = null)
			{
				if (account == null) return null;

				var message = new EventApiEvent(_clientInfoResolver)
				{
					CategoryId = EventCategory.MovingHouse,
					ActionId = eventAction,
					Username = _userSessionProvider.CurrentUserClaimsPrincipal.Identity.Name,
					SubCategoryId = subcategory,
					Partner = long.Parse(account.Partner),
					ContractAccount = long.Parse(account.AccountNumber)
				};
				message.Description = description ?? message.Description;
				message.MPRN = prn?.ToString() ?? message.MPRN;
				return message;
			}
		}
	}
}