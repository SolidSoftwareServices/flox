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
using NLog;

namespace EI.RP.DomainServices.Commands.Users.ContactUs
{
	internal class UserContactRequestEventBuilder : ICommandEventBuilder<UserContactRequest>
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		private readonly IClientInfoResolver _clientInfoResolver;
		private readonly IDomainQueryResolver _queryResolver;
		private readonly IUserSessionProvider _userSessionProvider;

		public UserContactRequestEventBuilder(IClientInfoResolver clientInfoResolver,
			IUserSessionProvider userSessionProvider,
			IDomainQueryResolver queryResolver)
		{
			_clientInfoResolver = clientInfoResolver;
			_userSessionProvider = userSessionProvider;
			_queryResolver = queryResolver;
		}

		public async Task<IEventApiMessage[]> BuildEventsOnSuccess(UserContactRequest command)
		{
			return await BuildFor(command, EventAction.LastOperationWasSuccessful);
		}

		public async Task<IEventApiMessage[]> BuildEventsOnError(UserContactRequest command,
			AggregateException exceptions)
		{
			return await BuildFor(command, EventAction.LastOperationFailed);
		}


		private async Task<IEventApiMessage[]> BuildFor(UserContactRequest command, long eventAction)
		{
			long partner = 0;
			try
			{
				var account =
					await _queryResolver.GetAccountInfoByAccountNumber(command.AccountNumber, true);

				partner = long.Parse(account.Partner);
			}
			catch (Exception ex)
			{
				Logger.Warn(() => ex.ToString());
				partner = long.Parse(command.Partner);
			}

			long subCategoryId = 0;
			if (command.ContactType == ContactQueryType.AddAdditionalAccount)
				subCategoryId = EventSubCategory.AddAdditionalAccountQuery;
			else if (command.ContactType == ContactQueryType.MeterRead)

				subCategoryId = EventSubCategory.MeterReadQuery;
			else if (command.ContactType == ContactQueryType.BillOrPayment)
				subCategoryId = EventSubCategory.BillOrPaymentQuery;
			else if (command.ContactType == ContactQueryType.Other)
				subCategoryId = EventSubCategory.OtherQuery;



			return ((IEventApiMessage) new EventApiEvent(_clientInfoResolver)
			{
				CategoryId = EventCategory.AccountEnquiryId,
				Description = eventAction == EventAction.LastOperationWasSuccessful
					? "Log Account Enquiry Request Successfully"
					: "Failed to Log Account Enquiry Request",
				ActionId = eventAction,
				Username = _userSessionProvider.CurrentUserClaimsPrincipal.Identity.Name,
				SubCategoryId = subCategoryId,
				Partner = partner,
				ContractAccount = long.Parse(command.AccountNumber)
			}).ToOneItemArray();
		}
	}
}