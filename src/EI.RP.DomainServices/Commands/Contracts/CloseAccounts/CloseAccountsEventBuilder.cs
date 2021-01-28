using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Events;
using EI.RP.CoreServices.Http;
using EI.RP.CoreServices.Http.Session;
using EI.RP.DataModels.Events;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;

namespace EI.RP.DomainServices.Commands.Contracts.CloseAccounts
{
	internal class CloseAccountsEventBuilder : ICommandEventBuilder<CloseAccountsCommand>
	{
		private readonly IClientInfoResolver _clientInfoResolver;
		private readonly IUserSessionProvider _userSessionProvider;

		public CloseAccountsEventBuilder(IClientInfoResolver clientInfoResolver,
			IUserSessionProvider userSessionProvider)
		{
			_clientInfoResolver = clientInfoResolver;
			_userSessionProvider = userSessionProvider;
		}

		public async Task<IEventApiMessage[]> BuildEventsOnSuccess(CloseAccountsCommand command)
		{
			return await BuildFor(command, EventAction.LastOperationWasSuccessful);
		}

		public async Task<IEventApiMessage[]> BuildEventsOnError(CloseAccountsCommand command,
			AggregateException exceptions)
		{
			return await BuildFor(command, EventAction.LastOperationFailed);
		}


		private async Task<IEventApiMessage[]> BuildFor(CloseAccountsCommand command, long eventAction)
		{
			var ctx = command.Context;

			IEventApiMessage[] result;
			if (ctx.AuditSubCategory == EventSubCategory.CloseElectricityAndGas)
				result = new[]
				{
					BuildEvent(ctx.ElectricityAccount),
					BuildEvent(ctx.GasAccount)
				};
			else
				result = new[] {BuildEvent(ctx.ContractAccount)};

			return result;

			IEventApiMessage BuildEvent(AccountInfo account)
			{
				return new EventApiEvent(_clientInfoResolver)
				{
					CategoryId = EventCategory.MovingHouse,
					ActionId = eventAction,
					Username = _userSessionProvider.CurrentUserClaimsPrincipal.Identity.Name,
					SubCategoryId = account.ClientAccountType == ClientAccountType.Electricity
						? EventSubCategory.CloseElectricity
						: account.ClientAccountType == ClientAccountType.Gas
							? EventSubCategory.CloseGas
							: throw new NotSupportedException(),
					Partner = long.Parse(account.Partner),
					ContractAccount = long.Parse(account.AccountNumber)
				};
			}
		}
	}
}