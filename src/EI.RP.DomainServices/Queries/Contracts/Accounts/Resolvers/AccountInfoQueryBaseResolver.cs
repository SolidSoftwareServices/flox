using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DataModels.Sap.CrmUmc.Dtos.Extensions;
using EI.RP.DataServices;
using Ei.Rp.DomainErrors;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.Infrastructure;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Infrastructure.Mappers;
using EI.RP.DomainServices.Queries.Contracts.BusinessPartners;
using NLog;

namespace EI.RP.DomainServices.Queries.Contracts.Accounts.Resolvers
{
	internal abstract class AccountInfoQueryBaseResolver<TAccountInfo> : IAccountInfoQueryResolver
		where TAccountInfo : AccountBase, new()
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

		


		protected IDomainQueryResolver QueryResolver { get; }
		private readonly IUserSessionProvider _userSessionProvider;
		protected AccountInfoQueryBusinessLogicResolver BusinessLogicHelper{ get; }
		protected IDomainMapper DomainMapper{ get; }
		protected ISapRepositoryOfErpUmc ErpUmc { get; }
		protected ISapRepositoryOfCrmUmc CrmUmc { get; }
		protected AccountInfoQueryBaseResolver(IUserSessionProvider userSessionProvider,
			IDomainQueryResolver queryResolver, ISapRepositoryOfCrmUmc crmUmc, IDomainMapper domainMapper,
			IResidentialPortalDataRepository repository, ISapRepositoryOfErpUmc erpUmc)
		{
			_userSessionProvider = userSessionProvider;
			QueryResolver = queryResolver;
			CrmUmc = crmUmc;
			DomainMapper = domainMapper;
			ErpUmc = erpUmc;

			//TODO: REFACTOR/move to specialised accountinfo this class is private that's why it does not need injection
			BusinessLogicHelper = new AccountInfoQueryBusinessLogicResolver(queryResolver, crmUmc, DomainMapper,
				repository, userSessionProvider);
		}

		public Type StrategyFor { get; } = typeof(TAccountInfo);


		public async Task<IEnumerable<TQueryResult>> ResolveAsAsync<TQueryResult>(AccountInfoQuery query)
		{
			return (await ResolveAsync(query)).Cast<TQueryResult>();
		}

		private async Task<IEnumerable<TAccountInfo>> ResolveAsync(AccountInfoQuery query)
		{
			if (query.RetrievesAll() && _userSessionProvider.IsAgentOrAdmin() &&
			    _userSessionProvider.ActingAsUserName != null)
				return await QueryResolver.GetAccountsByUserName<TAccountInfo>(_userSessionProvider.ActingAsUserName, true);


			var contractItems = (await GetContractItemsElectricityAndGasAccounts(query))?.ToArray() ??
			                    new ContractItemDto[0];

			var accountId = contractItems.Any()
				? contractItems.FirstOrDefault()?.AccountID
				: string.Empty;

			var businessAgreementsTask = GetBusinessAgreementForEnergyServiceAccount(query, accountId);

			contractItems = (await ResolveDuelFuelSisterAccounts(contractItems, query)).ToArray();

			var getAccountInfosFromContractItems =
				Task.WhenAll(contractItems.Select(MapToAccountInfo));

			var accountInfoFromBusinessAgreement = await MapToAccountInfo(await businessAgreementsTask);

			var result = (await getAccountInfosFromContractItems)
				.Union(accountInfoFromBusinessAgreement!=null?accountInfoFromBusinessAgreement.ToOneItemArray():new TAccountInfo[0])
				.Where(x => x != null);

			if (query.Opened.HasValue)
			{
				result = result.Where(x => x.IsOpen == query.Opened.Value);
			}

			return result;
		}

		
		private async Task<BusinessAgreementDto> GetBusinessAgreementForEnergyServiceAccount(
			AccountInfoQuery queryModel, string accountId)
		{
			if (queryModel.AccountType != null &&
			    !queryModel.AccountType.IsOneOf(ClientAccountType.EnergyService))
			{
				return null;
			}

			IFluentODataModelQuery<BusinessAgreementDto> query;

			if (queryModel.BusinessPartner != null)
				query = CrmUmc.NewQuery<AccountDto>()
					.Key(queryModel.BusinessPartner)
					.NavigateTo<BusinessAgreementDto>()
					.Expand(x => x.Account.BusinessAgreements);
			else if (!string.IsNullOrWhiteSpace(accountId))
				query = CrmUmc.NewQuery<AccountDto>()
					.Key(accountId)
					.NavigateTo<BusinessAgreementDto>()
					.Expand(x => x.Account.BusinessAgreements);

			else
				query = CrmUmc.NewQuery<BusinessAgreementDto>();

			if (queryModel.AccountNumber != null) query = query.Key(queryModel.AccountNumber);
			query = AddEnergyServicesQueryExpansions(query);
			var businessAgreements = await CrmUmc.GetMany(query);

			return
				businessAgreements.FirstOrDefault(c => c.AccountCategory == DivisionType.EnergyService);
		}

	


		private async Task<IEnumerable<ContractItemDto>> GetContractItemsElectricityAndGasAccounts(
			AccountInfoQuery queryModel)
		{
			if (queryModel.AccountType != null &&
			    !queryModel.AccountType.IsOneOf(ClientAccountType.Electricity, ClientAccountType.Gas))
			{
				return new ContractItemDto[0];
			}

			IEnumerable<BusinessAgreementDto> result;
			if (queryModel.BusinessPartner != null)
			{
				//this is because of limitations in the odata support of SAP

				var bas = (await CrmUmc.NewQuery<AccountDto>()
					.Key(queryModel.BusinessPartner)
					.NavigateTo<ContractItemDto>().GetMany()).Select(x => x.BusinessAgreementID).Distinct();

				result = await Task.WhenAll(bas.Select(async x =>
					await AddElectricityAndGasQueryExpansions(CrmUmc.NewQuery<BusinessAgreementDto>().Key(x))
						.GetOne()));
			}
			else if (queryModel.AccountNumber != null)
			{
				result = await AddElectricityAndGasQueryExpansions(CrmUmc.NewQuery<BusinessAgreementDto>()
					.Key(queryModel.AccountNumber)).GetMany();
			}
			else if (queryModel.Prn != null)
			{
				//this is because of limitations in the odata support of SAP
				var bas = (await CrmUmc
						.NewQuery<PointOfDeliveryDto>()
						.Key(queryModel.Prn.ToString())
						.NavigateTo<PremiseDto>()
						.NavigateTo<ContractItemDto>().GetMany())
					.Select(x => x.BusinessAgreementID).Distinct();
				result = await Task.WhenAll(bas.Select(async x =>
					await AddElectricityAndGasQueryExpansions(CrmUmc.NewQuery<BusinessAgreementDto>().Key(x))
						.GetOne()));
			}
			else if (_userSessionProvider.IsAgentOrAdmin())
			{
				//this is because of limitations in the odata support of SAP
				var businessPartner = _userSessionProvider.ActingAsBusinessPartnerId;
				var bas = (await CrmUmc.NewQuery<AccountDto>()
					.Key(businessPartner)
					.NavigateTo<ContractItemDto>().GetMany()).Select(x => x.BusinessAgreementID).Distinct();

				result = await Task.WhenAll(bas.Select(async x =>
					await AddElectricityAndGasQueryExpansions(CrmUmc.NewQuery<BusinessAgreementDto>().Key(x))
						.GetOne()));
			}
			else
			{
				result = await AddElectricityAndGasQueryExpansions(CrmUmc.NewQuery<BusinessAgreementDto>())
					.GetMany();
			}

			
			var items = ResolveContracts(result.Where(x => x.CollectiveParentID == string.Empty));
			items = ApplyAccountTypeFilter(items,queryModel);
			return ApplyOpenFilter(items,queryModel);
		}

		private IEnumerable<ContractItemDto> ApplyAccountTypeFilter(IEnumerable<ContractItemDto> src,
			AccountInfoQuery accountInfoQuery)
		{
			
			if (accountInfoQuery.AccountType == null) return src;

			return src.Where(b => ResolveClientAccountType(b) == accountInfoQuery.AccountType);
		}

		private IEnumerable<ContractItemDto> ApplyOpenFilter(IEnumerable<ContractItemDto> contracts,
			AccountInfoQuery query)
		{
			var result = contracts as ContractItemDto[] ?? contracts.ToArray();
			if (query.Opened.HasValue)
			{
				result = result.Where(x => IsActiveContract(x) == query.Opened.Value).ToArray();
			}

			return result;
		}
		private IEnumerable<ContractItemDto> ResolveContracts(
			IEnumerable<BusinessAgreementDto> businessAgreements)
		{
			var agreements = businessAgreements.ToArray();
			foreach (var businessAgreement in agreements)
			{
				foreach (var contractItem in businessAgreement.ContractItems)
					contractItem.BusinessAgreement = businessAgreement;
			}

			return agreements.SelectMany(x => x.ContractItems);
		}
		protected bool IsActiveContract(ContractItemDto contractItemDto)
		{
			return contractItemDto.ContractItemEXTAttrs.USER_STAT_DESC_SHORT == ContractStatusType.Active;
		}

		private async Task<IEnumerable<ContractItemDto>> ResolveDuelFuelSisterAccounts(
			IEnumerable<ContractItemDto> src,
			AccountInfoQuery queryModel)
		{
			var result = src;
			if (queryModel.RetrieveDuelFuelSisterAccounts)
			{
				result = result
					.Where(x => IsActiveContract(x) ||
					            x.ContractItemEXTAttrs.USER_STAT_DESC_SHORT == ContractStatusType.Pending)
					.ToArray();

				if (result.Count() != 1)
					throw new InvalidOperationException("Expected only one contract as source to find the duel part");

				result = await ResolveDuelFuelSisterAccounts(result.Single(),
					queryModel.IgnoreDuelFuelAddressMismatch);
			}

			return result;
		}
		protected async Task<IEnumerable<ContractItemDto>> ResolveDuelFuelSisterAccounts(ContractItemDto sourceContract,
																						  bool ignoreAddressMismatch)
			{
				const int isValidBundleImportantCharsLength = 22;

				var businessAgreementDtos = (await AddElectricityAndGasQueryExpansions(CrmUmc.NewQuery<AccountDto>()
						.Key(sourceContract.BusinessAgreement.Account.AccountID)
						.NavigateTo<BusinessAgreementDto>())
						.GetMany())
						.ToArray();

				var bpContracts = ResolveContracts(businessAgreementDtos).ToArray();
				var otherThanSourceContracts = bpContracts
									.Where(x => IsActiveContract(x) &&
												x.ContractID != sourceContract.ContractID)
									.ToArray();

				var sourceContractBundleRef = sourceContract.ContractItemEXTAttrs.BUNDLE_REF.Truncate(isValidBundleImportantCharsLength);
				var bundleMatches = new ContractItemDto[0];

				if (!string.IsNullOrWhiteSpace(sourceContractBundleRef))
				{
					bundleMatches = otherThanSourceContracts
									.Where(x => x.ContractItemEXTAttrs.BUNDLE_REF.Truncate(isValidBundleImportantCharsLength) == sourceContractBundleRef)
									.ToArray();
				}

				var bundleMatchesAtSameAddress = ignoreAddressMismatch ? new ContractItemDto[0] :
													bundleMatches
													.Where(x => x.Premise.AddressInfo.IsEquivalentTo(sourceContract.Premise.AddressInfo))
													.ToArray();

				if (!ignoreAddressMismatch && !bundleMatchesAtSameAddress.Any() && bundleMatches.Any())
				{
					Logger.Debug($"{ResidentialDomainError.AccountsInBundleConfiguredOnDifferentAddresses.ErrorMessage} " +
								 $"Account: {sourceContract.AccountID}, " +
								 $"BundleMatchAtSameAddress: {bundleMatchesAtSameAddress.Any()}, " +
								 $"bundleMatches: {bundleMatches.Any()}");
					throw new DomainException(ResidentialDomainError.AccountsInBundleConfiguredOnDifferentAddresses);
				}

				var contractItems = (!ignoreAddressMismatch && bundleMatchesAtSameAddress.Any() ? bundleMatchesAtSameAddress : bundleMatches).ToArray();

				return sourceContract.ToOneItemArray().Union(contractItems);
			}

		private  IFluentODataModelQuery<BusinessAgreementDto> AddElectricityAndGasQueryExpansions(
			IFluentODataModelQuery<BusinessAgreementDto> query)
		{
			//common expansions to all the account info related models
			query
				.Expand(x => x.ContractItems)
				.Expand(x => x.ContractItems[0].Premise)
				.Expand(x => x.ContractItems[0].Premise.PointOfDeliveries)
				.Expand(x => x.ContractItems[0].ContractItemEXTAttrs);
			return AddElectricityAndGasQueryCustomExpansions(query);
		}

		private IFluentODataModelQuery<BusinessAgreementDto> AddEnergyServicesQueryExpansions(
			IFluentODataModelQuery<BusinessAgreementDto> query)
		{
			//common expansions to all the account info related models
			query
				.Expand(x => x.ContractItems)
				.Expand(x => x.ContractItems[0].Premise)
				.Expand(x => x.ContractItems[0].Premise.PointOfDeliveries);
				
			return AddEnergyServicesQueryCustomExpansions(query);
		}

		/// <summary>
		/// Expand the model specific mapping needings
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		protected virtual IFluentODataModelQuery<BusinessAgreementDto> AddElectricityAndGasQueryCustomExpansions(
			IFluentODataModelQuery<BusinessAgreementDto> query) => query;

		protected virtual IFluentODataModelQuery<BusinessAgreementDto> AddEnergyServicesQueryCustomExpansions(
			IFluentODataModelQuery<BusinessAgreementDto> query) => query;
		
		protected virtual async Task<TAccountInfo> MapToAccountInfo(BusinessAgreementDto source)
		{
			if (source == null) return null;

			var accountInfo = new TAccountInfo();
			accountInfo.AccountNumber = source.BusinessAgreementID;
			accountInfo.Partner = source.AccountID;


			accountInfo.ClientAccountType = ResolveClientAccountType(source);
			accountInfo.Description = source.Description ?? string.Empty;
			if (accountInfo.ClientAccountType == ClientAccountType.EnergyService)
			{
				var balances = await ErpUmc.NewQuery< EI.RP.DataModels.Sap.ErpUmc.Dtos.AccountDto>()
					.Key(source.AccountID)
					.NavigateTo(x => x.AccountESAProductBalance)
					.GetMany();
				accountInfo.IsOpen = balances.Any(x => x.NumOutstandInt > 0M || x.OutstandAmount > 0);
			}

			ClientAccountType ResolveClientAccountType(BusinessAgreementDto agreement)
			{
				return ((DivisionType) agreement.AccountCategory)?.ToAccountType() ??
				       ClientAccountType.EnergyService;
			}

			return accountInfo;
		}

		

		protected virtual async Task<TAccountInfo> MapToAccountInfo(ContractItemDto contractItemDto)
		{
			var accountInfo = await MapToAccountInfo(contractItemDto.BusinessAgreement);
			accountInfo.ClientAccountType = ResolveClientAccountType(contractItemDto);
			accountInfo.ContractStatus = (ContractStatusType)contractItemDto.ContractItemEXTAttrs.USER_STAT_DESC_SHORT;

			accountInfo.IsOpen = IsActiveContract(contractItemDto);
			accountInfo.PointReferenceNumber= ResolvePointReferenceNumber(accountInfo.ClientAccountType,contractItemDto);
			return accountInfo;

			PointReferenceNumber ResolvePointReferenceNumber(ClientAccountType accountType, ContractItemDto contract)
			{
				var pointReferenceNumberExternalId = contract.Premise?.PointOfDeliveries.FirstOrDefault()?.ExternalID;

				return accountType.IsElectricity()
					? (ElectricityPointReferenceNumber)pointReferenceNumberExternalId
					: accountType.IsGas()
						? (PointReferenceNumber)(GasPointReferenceNumber)pointReferenceNumberExternalId
						: null;
			}

			
		}

		protected ClientAccountType ResolveClientAccountType(ContractItemDto contract)
		{
			return ((DivisionType)contract.DivisionID)?.ToAccountType() ?? ClientAccountType.EnergyService;
		}
	}
}