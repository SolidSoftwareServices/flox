using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EI.RP.CoreServices.Caching;
using EI.RP.CoreServices.Cqrs.Events;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Http;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Events;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Queries.Contracts.Accounts;

namespace EI.RP.DomainServices.Commands.Banking.DirectDebit.SetUpDirectDebit
{
	internal class SetUpDirectDebitEventBuilder : ICommandEventBuilder<SetUpDirectDebitDomainCommand>
	{
		private readonly IClientInfoResolver _clientInfoResolver;
		private readonly IUserSessionProvider _userSessionProvider;
		private readonly IDomainQueryResolver _queryResolver;


		public SetUpDirectDebitEventBuilder(IClientInfoResolver clientInfoResolver,
			IUserSessionProvider userSessionProvider,IDomainQueryResolver queryResolver)
		{
			_clientInfoResolver = clientInfoResolver;
			_userSessionProvider = userSessionProvider;
			_queryResolver = queryResolver;
		}

        public async Task<IEventApiMessage[]> BuildEventsOnSuccess(SetUpDirectDebitDomainCommand command)
        {
	        IEventApiMessage result;
	        var accountInfo = await _queryResolver.GetAccountInfoByAccountNumber(command.AccountNumber,true);

	        if (command.IsNewBankAccount)
	        {
		        result = BuildSuccessfulEvent(command, "Sign up for Direct Debit",
			        accountInfo.PaymentMethod == PaymentMethodType.Equalizer
				        ? EventSubCategory.EqualiserSignUpId
				        : EventSubCategory.SignUpDirectDebit);
	        }
	        else
	        {
		        if (accountInfo.PaymentMethod == PaymentMethodType.Equalizer)
		        {
			        result = BuildSuccessfulEvent(command, "successfully Log Changes",
				        EventSubCategory.EqualiserEditDetails);
		        }
		        else
		        {
			        result = BuildSuccessfulEvent(command, "Edit Direct Debit", EventSubCategory.EditDirectDebit);
		        }
	        }

	        return result.ToOneItemArray();
        }

        public Task<IEventApiMessage[]> BuildEventsOnError(SetUpDirectDebitDomainCommand command,
            AggregateException exceptions)
		{
			return Task.FromResult<IEventApiMessage[]>(null);
		}

		private IEventApiMessage BuildSuccessfulEvent(SetUpDirectDebitDomainCommand domainCommand, string description,
			long subCategoryId)
		{
			var @event = new EventApiEvent(_clientInfoResolver);
			@event.CategoryId = EventCategory.Change;
			@event.ActionId = EventAction.LastOperationWasSuccessful;
			@event.Username = _userSessionProvider
				.CurrentUserClaimsPrincipal
				.Claims
				.Single(x => x.Type == ClaimTypes.Email)
				.Value;
			@event.Partner = long.Parse(domainCommand.BusinessPartner);
			@event.ContractAccount = long.Parse(domainCommand.AccountNumber);
			@event.Description = description;
			@event.SubCategoryId = subCategoryId;
			return @event;
		}
	}
}