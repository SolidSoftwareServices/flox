using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DataModels.Sap.CrmUmc.Dtos.Extensions;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.Banking;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Infrastructure.Mappers;
using EI.RP.DomainServices.ModelExtensions;
using EI.RP.DomainServices.Queries.Metering.Devices;
using NLog;

namespace EI.RP.DomainServices.Queries.Contracts.Accounts.Resolvers
{
	class AccountInfoQueryResolver:AccountInfoQueryBaseResolver<AccountInfo>
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		public AccountInfoQueryResolver(IUserSessionProvider userSessionProvider, IDomainQueryResolver queryResolver, ISapRepositoryOfCrmUmc crmUmc, IDomainMapper domainMapper, IResidentialPortalDataRepository repository, ISapRepositoryOfErpUmc erpRepo) 
			: base(userSessionProvider, queryResolver, crmUmc, domainMapper, repository,erpRepo)
		{
		}
		
		protected override async Task<AccountInfo> MapToAccountInfo(BusinessAgreementDto source)
		{
			if (source == null) return null;

			var mapBusinessAgreementTask = DomainMapper.Map<BusinessAgreementDto, BusinessAgreement>(source);
			var mapBankAcountInfoTask = DomainMapper.Map<BusinessAgreementDto, BankAccountInfo>(source);
			var accountInfo = await base.MapToAccountInfo(source);

			var hasQuotationsInProgress = BusinessLogicHelper.HasQuotationsInProgress(accountInfo.Partner);


			accountInfo.Name = source.Account?.FullName ?? string.Empty;
			accountInfo.FirstName = source.Account?.FirstName ?? string.Empty;
			accountInfo.LastName = source.Account?.LastName ?? string.Empty;
			

			accountInfo.PaymentMethod = !string.IsNullOrEmpty(source.AlternativePayerCA)
				? PaymentMethodType.AlternativePayer
				: (PaymentMethodType)source.IncomingPaymentMethodID;

			accountInfo.PaperBillChoice =
				source.PaperBill == PaperBillChoice.On ? PaperBillChoice.On : PaperBillChoice.Off;

            accountInfo.BillToAccountAddressId = source.BillToAccountAddressID;
            
            accountInfo.BusinessAgreement = await mapBusinessAgreementTask;
            accountInfo.IncomingBankAccount = await mapBankAcountInfoTask;

			accountInfo.BusinessAgreement = await mapBusinessAgreementTask;
			accountInfo.IncomingBankAccount = await mapBankAcountInfoTask;
			accountInfo.HasQuotationsInProgress = await hasQuotationsInProgress;
			return accountInfo;
		}

		protected override  async Task<AccountInfo> MapToAccountInfo(ContractItemDto contractItemDto)
		{
			var getAccountInfoTask = base.MapToAccountInfo(contractItemDto);

			//direct value resolution
			var contractId = contractItemDto.ContractID;
			var lossInProgress = contractItemDto.ContractItemEXTAttrs.USER_STAT_DESC_SHORT == ContractStatusType.LossinProgress;
			
			var isPaygCustomer = ((ProductType)contractItemDto.ProductID).IsPAYGPRoduct();
			var accountIsOpen = IsActiveContract(contractItemDto);
			var contractStartDate = contractItemDto.ContractStartDate;
			var bundleReference = contractItemDto.ContractItemEXTAttrs.BUNDLE_REF;

			//wait for mapped business agreement based items
			var accountInfo = await getAccountInfoTask;
			
			//values from tasks
			var resolveBankAccountsTask = BusinessLogicHelper.ResolveBankAccounts(contractItemDto);
			
			var resolveCanSwitchToStandardPlanTask = ResolveCanSwitchToStandardPlan(accountInfo.AccountNumber, contractItemDto);
			var hasStaffDiscountAppliedTask = BusinessLogicHelper.HasStaffDiscountApplied(accountInfo.PointReferenceNumber,accountInfo.ClientAccountType);
			var switchToSmartPlanDismissedTask =
				BusinessLogicHelper.SwitchToSmartPlanWasDismissed(accountInfo.AccountNumber);
			var resolveSmartActivationStatusTask = BusinessLogicHelper.ResolveSmartActivationStatus(
				new AccountInfoQueryBusinessLogicResolver.ResolveSmartActivationStatusRequest(
					accountInfo.AccountNumber
					, accountInfo.ClientAccountType
					, accountIsOpen
					, contractItemDto
					, isPaygCustomer
					, accountInfo.HasQuotationsInProgress
					, lossInProgress
					, accountInfo.PointReferenceNumber
					, accountInfo.PaymentMethod
					, contractStartDate
					,bundleReference,
					ResolveDuelFuelSisterAccounts
				)
			);
			var canSubmitMeterReadingTask = BusinessLogicHelper.CanSubmitMeterReading(accountInfo.AccountNumber, contractItemDto);

			//mappings
			accountInfo.ContractId = contractId;
			accountInfo.IsLossInProgress = lossInProgress;
			accountInfo.IsPAYGCustomer = isPaygCustomer;
			
			accountInfo.ContractStartDate = contractStartDate;
			accountInfo.ContractEndDate = contractItemDto.ContractEndDate;

			if (contractItemDto.Premise != null)
			{
				accountInfo.Description = contractItemDto.Premise.AddressInfo == null
					? string.Empty
					: contractItemDto.Premise.AddressInfo.AsDescriptionText();
				accountInfo.PremiseConnectionObjectId = contractItemDto.Premise.ConnectionObjectID;
			}


			accountInfo.BundleReference = bundleReference;
			accountInfo.SmartPeriods = BusinessLogicHelper.ResolveSmartPeriods(contractItemDto);
			accountInfo.NonSmartPeriods = BusinessLogicHelper.ResolveNonSmartPeriods(contractItemDto);
			accountInfo.PlanName = contractItemDto.Product?.Description;
			accountInfo.DiscountAppliedPercentage = contractItemDto.DiscountPercentage();
			accountInfo.BankAccounts = await resolveBankAccountsTask;
			accountInfo.SmartActivationStatus = await resolveSmartActivationStatusTask;
			accountInfo.HasStaffDiscountApplied = await hasStaffDiscountAppliedTask;
			accountInfo.CanSwitchToStandardPlan =await resolveCanSwitchToStandardPlanTask;
			accountInfo.SwitchToSmartPlanDismissed = await switchToSmartPlanDismissedTask;
			accountInfo.CanSubmitMeterReading = accountInfo.IsOpen && await canSubmitMeterReadingTask;
			return accountInfo;
		
		}

		

		protected override IFluentODataModelQuery<BusinessAgreementDto> AddEnergyServicesQueryCustomExpansions(
			IFluentODataModelQuery<BusinessAgreementDto> query)
		{

			query = query
				.Expand(x => x.ContractItems[0].Attributes)
				.Expand(x => x.BillToAccountAddress)
				.Expand(x => x.IncomingBankAccount);
			return query;
		}

		protected override IFluentODataModelQuery<BusinessAgreementDto> AddElectricityAndGasQueryCustomExpansions(IFluentODataModelQuery<BusinessAgreementDto> query)
		
		{
			return query
				.Expand(x => x.Account)
				.Expand(x => x.Account.BankAccounts)
				.Expand(x => x.BillToAccountAddress)
				.Expand(x => x.IncomingBankAccount)
				.Expand(x => x.ContractItems[0].Attributes)
				.Expand(x => x.ContractItems[0].Product)
				.Expand(x => x.ContractItems[0].ContractItemTimeSlices)
				.Expand(x => x.ContractItems[0].ContractItemTimeSlices[0].SmartConsents);
		}

		private async Task<bool> ResolveCanSwitchToStandardPlan(string accountNumber, ContractItemDto contractItemDto)
		{
			var duelFuelContracts = await ResolveDuelFuelSisterAccounts(contractItemDto, true);
			if (duelFuelContracts.Count() == 1 && contractItemDto.DivisionID == DivisionType.Gas)
				return false;

			var devices = await QueryResolver.GetDevicesByAccountAndContract(accountNumber, contractId: contractItemDto.ContractID, byPassPipeline: true);

			return devices.Any(x => x.SmartActivationStatus == SmartActivationStatus.SmartActive &&
			                        x.CTF != null &&
			                        x.CTF.IsOneOf(CommsTechnicallyFeasibleValue.CTF3,
				                        CommsTechnicallyFeasibleValue.CTF4));
		}
	}
}