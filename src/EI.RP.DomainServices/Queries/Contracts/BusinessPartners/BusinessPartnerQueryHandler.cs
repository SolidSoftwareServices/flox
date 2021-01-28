using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DataModels.Sap.CrmUmc.Dtos.Extensions;
using EI.RP.DataModels.Sap.CrmUmc.Functions;
using EI.RP.DataServices;
using Ei.Rp.DomainErrors;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.Infrastructure;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using NLog;

namespace EI.RP.DomainServices.Queries.Contracts.BusinessPartners
{
	internal class BusinessPartnerQueryHandler : QueryHandler<BusinessPartnerQuery>
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		private readonly IDomainQueryResolver _queryResolver;
		private readonly ISapRepositoryOfCrmUmc _repository;
		private readonly IUserSessionProvider _userSessionProvider;

		public BusinessPartnerQueryHandler(ISapRepositoryOfCrmUmc repository,
			IUserSessionProvider userSessionProvider, IDomainQueryResolver queryResolver)
		{
			_repository = repository;
			_userSessionProvider = userSessionProvider;
			_queryResolver = queryResolver;
		}

		protected override Type[] ValidQueryResultTypes { get; } =
			{typeof(BusinessPartner), typeof(AccountInfo), typeof(AccountOverview)};

		protected override async Task<IEnumerable<TQueryResult>> _ExecuteQueryAsync<TQueryResult>(
			BusinessPartnerQuery query)
		{
			if (!_userSessionProvider.IsAgentOrAdmin())
				throw new DomainException(ResidentialDomainError.AuthorizationError);


			var getAccountsFunction = new GetAccountsFunction
			{
				Query =
				{
					AccountID = query.PartnerNum ?? string.Empty,
					UserName = query.UserName ?? string.Empty,
					BusinessAgreementID = string.Empty,
					FirstName = string.Empty,
					LastName = query.LastName ?? string.Empty,
					HouseNo = query.HouseNum ?? string.Empty,
					Street = query.Street ?? string.Empty,
					City = query.City ?? string.Empty,
					Country = string.Empty,
					Region = string.Empty,
					PostalCode = string.Empty,
					Phone = string.Empty,
					Email = string.Empty
				}
			}.Expand<GetAccountsFunction, AccountDto>(x => x.AccountAddresses);

			var result = await _repository.ExecuteFunctionWithManyResults(getAccountsFunction);
			if (typeof(TQueryResult) == typeof(AccountInfo))
				return (await Task.WhenAll(result.Select(x =>
						_queryResolver.GetAccountInfoByBusinessPartner(x.AccountID, true)))).SelectMany(x => x)
					.Cast<TQueryResult>();
			if (typeof(TQueryResult) == typeof(AccountOverview))
				return (await Task.WhenAll(result.Select(x =>
						_queryResolver.GetAccountOverViewByBusinessPartner(x.AccountID, true)))).SelectMany(x => x)
					.Cast<TQueryResult>();
			if (typeof(TQueryResult) == typeof(BusinessPartner))
				return result.Select(x => new BusinessPartner
				{
					NumPartner = x.AccountID,
					Description = x.AccountAddresses?.FirstOrDefault()?.AddressInfo.AsDescriptionText() ?? string.Empty
				}).Cast<TQueryResult>();
			throw new InvalidProgramException("This could never happen");
		}
	}
}