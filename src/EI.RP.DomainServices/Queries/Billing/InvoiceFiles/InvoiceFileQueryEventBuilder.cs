using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Events;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Http;
using EI.RP.CoreServices.Http.Session;
using EI.RP.DataModels.Events;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using NLog;

namespace EI.RP.DomainServices.Queries.Billing.InvoiceFiles
{
	internal class InvoiceFileQueryEventBuilder : IQueryEventBuilder<InvoiceFileQuery>
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		private readonly IClientInfoResolver _clientInfoResolver;

		private readonly IDomainQueryResolver _queryResolver;
		private readonly IUserSessionProvider _userSessionProvider;

		public InvoiceFileQueryEventBuilder(IClientInfoResolver clientInfoResolver,
			IUserSessionProvider userSessionProvider,
            IDomainQueryResolver queryResolver)
		{
			_clientInfoResolver = clientInfoResolver;
			_userSessionProvider = userSessionProvider;
            _queryResolver = queryResolver;
        }

		public async Task<IEventApiMessage> BuildEventOnSuccess(InvoiceFileQuery query)
		{
			return await BuildFor(query, EventAction.LastOperationWasSuccessful);
		}

		public async Task<IEventApiMessage> BuildEventOnError(InvoiceFileQuery query, AggregateException exceptions)
		{
			return await BuildFor(query, EventAction.LastOperationFailed);
		}

		private async Task<EventApiEvent> BuildFor(InvoiceFileQuery query, long eventAction)
		{
			var partner = "-1";
			var accountNumber = "-1";
			try
			{
				var account = await _queryResolver.GetAccountInfoByAccountNumber(query.AccountNumber, true);
				partner = account.Partner;
				accountNumber = account.AccountNumber;
			}
			catch (Exception ex)
			{
				Logger.Error(() => $"Execution continues, swallowing exception: {ex}");
			}

			var result = new EventApiEvent(_clientInfoResolver);
			result.Username = _userSessionProvider.CurrentUserClaimsPrincipal.Identity.Name;
			result.CategoryId = EventCategory.View;
			result.ActionId = eventAction;
			result.Partner = long.Parse(partner);
			result.ContractAccount = long.Parse(accountNumber);
			result.Description = query.ReferenceDocNumber;
			result.SubCategoryId = EventSubCategory.ViewBill;
			return result;
		}
	}
}