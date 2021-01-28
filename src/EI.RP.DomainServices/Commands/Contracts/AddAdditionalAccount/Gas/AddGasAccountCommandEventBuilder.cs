using Ei.Rp.DomainModels.Contracts;
using EI.RP.CoreServices.Cqrs.Events;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Http;
using EI.RP.CoreServices.Http.Session;
using EI.RP.DataModels.Events;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EI.RP.DomainServices.Commands.Contracts.AddAdditionalAccount.Gas
{
	internal class AddGasAccountCommandEventBuilder : ICommandEventBuilder<AddGasAccountCommand>
	{
		private readonly IUserSessionProvider _userSessionProvider;
		private readonly IClientInfoResolver _clientInfoResolver;
		private readonly IDomainQueryResolver _queryResolver;

		public AddGasAccountCommandEventBuilder(IUserSessionProvider userSessionProvider,
			IClientInfoResolver clientInfoResolver,
			IDomainQueryResolver queryResolver)
		{
			_userSessionProvider = userSessionProvider;
			_clientInfoResolver = clientInfoResolver;
			_queryResolver = queryResolver;
		}

		public Task<IEventApiMessage[]> BuildEventsOnSuccess(AddGasAccountCommand command)
		{
			return BuildFor(command, EventAction.LastOperationWasSuccessful);
		}

		public Task<IEventApiMessage[]> BuildEventsOnError(AddGasAccountCommand command, AggregateException exceptions)
		{
			return BuildFor(command, EventAction.LastOperationFailed);
		}

		private async Task<IEventApiMessage[]> BuildFor(AddGasAccountCommand command, long eventAction)
		{
			var account = await _queryResolver.GetAccountInfoByAccountNumber(command.BaseElectricityAccountNumber, true);

			var isUsingDirectDebit = command.PaymentSetUp == PaymentSetUpType.SetUpNewDirectDebit || command.PaymentSetUp == PaymentSetUpType.UseExistingDirectDebit;
			var ibanLastFourDigits = command.IBAN?.Substring(command.IBAN.Length - 4, 4);

			var events = new List<IEventApiMessage>();
			if (isUsingDirectDebit)
			{
				events.Add(new EventApiEvent(_clientInfoResolver)
				{
					CategoryId = EventCategory.AddGas,
					ActionId = eventAction,
					Username = _userSessionProvider.CurrentUserClaimsPrincipal.Identity.Name,
					SubCategoryId = GetEventSubCategory(),
					Partner = long.Parse(account.Partner),
					MPRN = account.PointReferenceNumber.ToString(),
					ContractAccount = long.Parse(command.BaseElectricityAccountNumber),
					Description = ibanLastFourDigits
				});
			}

			events.Add(new EventApiEvent(_clientInfoResolver)
			{
				CategoryId = EventCategory.AddGas,
				ActionId = eventAction,
				Username = _userSessionProvider.CurrentUserClaimsPrincipal.Identity.Name,
				SubCategoryId = EventSubCategory.AddGasAccount,
				Partner = long.Parse(account.Partner),
				MPRN = account.PointReferenceNumber.ToString(),
				ContractAccount = long.Parse(command.BaseElectricityAccountNumber),
				Description = "Add Gas"
			});

			events.Add(new EventApiEvent(_clientInfoResolver)
			{
				CategoryId = EventCategory.AddGas,
				ActionId = eventAction,
				Username = _userSessionProvider.CurrentUserClaimsPrincipal.Identity.Name,
				SubCategoryId = EventSubCategory.AddGasAccount,
				Partner = long.Parse(account.Partner),
				MPRN = account.PointReferenceNumber.ToString(),
				ContractAccount = long.Parse(command.BaseElectricityAccountNumber),
				Description = $"Confirmation Page {(eventAction==EventAction.LastOperationWasSuccessful? "displayed" : "Error")}"
			});

			return events.ToArray();

			long? GetEventSubCategory()
			{
				switch (command.PaymentSetUp)
				{
					case PaymentSetUpType.SetUpNewDirectDebit:
						return EventSubCategory.AddGasAccountNewDirectDebit;
					case PaymentSetUpType.UseExistingDirectDebit:
						return EventSubCategory.UseElectricityDirectDebit;
					default:
						return EventSubCategory.AddGasAccount;
				}
			}
		}
	}
}
