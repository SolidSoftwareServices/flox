using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Sap.ErpUmc.Dtos;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.Billing;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.InternalShared.Contracts;
using EI.RP.DomainServices.Queries.Contracts.Accounts;

namespace EI.RP.DomainServices.Queries.Billing.Info
{
	internal class GeneralBillingQueryHandler : QueryHandler<GeneralBillingInfoQuery>
	{
		private readonly ISapRepositoryOfErpUmc _erpRepository;
		private readonly IDomainQueryResolver _queryResolver;
		private readonly IContractQueryResolver _contractQueryResolver;


		public GeneralBillingQueryHandler(ISapRepositoryOfErpUmc erpRepository, IDomainQueryResolver queryResolver,IContractQueryResolver contractQueryResolver)
		{
			_erpRepository = erpRepository; 
			_queryResolver = queryResolver;
			_contractQueryResolver = contractQueryResolver;
		}

		protected override Type[] ValidQueryResultTypes { get; } = {typeof(GeneralBillingInfo)};

		protected override async Task<IEnumerable<TQueryResult>> _ExecuteQueryAsync<TQueryResult>(
			GeneralBillingInfoQuery query)
		{
			var account = await _queryResolver.GetAccountInfoByAccountNumber(query.AccountNumber, true);
			var latestContract = account.ContractId == null ? null : await _contractQueryResolver.GetCurrentContract(account);
			
			var result = await ResolveBillingInfo(account,latestContract);
			
			
			return result.ToOneItemArray().Cast<TQueryResult>();
		}
		private async Task<GeneralBillingInfo> ResolveBillingInfo(AccountInfo accountInfo,ContractDto latestContract)
		{
			return await (accountInfo.IsOpen ? GetOpenBillingInfo(accountInfo,latestContract) : GetClosedBillingInfo(accountInfo,latestContract));
		}

		private async Task<GeneralBillingInfo> GetOpenBillingInfo(AccountInfo accountInfo, ContractDto latestContract)
		{
			var billingData = new GeneralBillingInfo();
			billingData.CurrentBalanceAmount =
				latestContract?.ContractAccount?.ContractAccountBalance?.CurrentBalance ?? EuroMoney.Zero;

			var device = latestContract?.Devices.FirstOrDefault();
			if (device != null)
			{
				var futureMeterReading = device.FutureMeterReadings.FirstOrDefault();
				billingData.NextBillDate = futureMeterReading?.MeterReadingDate ??
				                           (accountInfo.ContractStartDate?.AddDays(60) ?? DateTime.Now.AddDays(60));

				ResolveReadingType();
				ResolveNextBillInfo();
				billingData.IsDeviceRefundAvailable = ResolveDeviceRefund(device, accountInfo.ClientAccountType);
			}
			else
			{
				billingData.IsDeviceRefundAvailable = true;
				billingData.NextBillDate =
					(accountInfo.ContractStartDate ?? DateTime.Now)
					.AddDays(60);
			}

			billingData.PaymentMethod = accountInfo.PaymentMethod;
			
			if (latestContract?.ActivePaymentScheme != null)
			{
				billingData.EqualizerAmount = latestContract.ActivePaymentScheme.Amount;
				billingData.EqualizerNextBillDate = latestContract.ActivePaymentScheme.NextDueDate;
				billingData.EqualizerStartDate = latestContract.ActivePaymentScheme.StartDate;
				billingData.NextBillDate = (latestContract.ActivePaymentScheme.NextDueDate ?? DateTime.Now).AddDays(60); 
			}

			billingData.MonthlyBilling =  ResolveMonthlyBilling();
			return billingData;
			
			 GeneralBillingInfo.MonthlyBillingInfo ResolveMonthlyBilling()
			{
				var result = new GeneralBillingInfo.MonthlyBillingInfo();
				var agreement = accountInfo.BusinessAgreement;
				result.IsMonthlyBillingActive = agreement.FixedBillingDateDay.HasValue;
				result.MonthlyBillingDayOfMonth = agreement.FixedBillingDateDay;

				result.CanSwitchToMonthlyBilling = ResolveProductMonthlyBillingAvailability();

				return result;
				bool ResolveProductMonthlyBillingAvailability()
				{
					var contractItemDto = agreement.Contracts.FirstOrDefault();
					return contractItemDto?.IsBilledMonthly??false;
				}
			}
			void ResolveReadingType()
			{
				var reading = device?.MeterReadingResults
					.Where(x =>
						x.MeterReadingStatusID == MeterReadingStatus.Billed &&
						(x.MeterReadingReasonID == MeterReadingReason.PeriodicMeterReading ||
						 x.MeterReadingReasonID == MeterReadingReason.InterimMeterReadingWithBilling ||
						 x.MeterReadingReasonID == MeterReadingReason.MeterReadingAtMoveOut))
					.OrderByDescending(x => x.ReadingDateTime)
					.FirstOrDefault();

				if (reading != null)
					billingData.MeterReadingType = MeterReadingCategoryType.From(reading.MeterReadingCategoryID);
			}

			void ResolveNextBillInfo()
			{
				var meterRead24Months = device
					?.MeterReadingResults
					.Where(x => x.ReadingDateTime > DateTime.Now.AddMonths(-24)).ToList();

				var readingResultList = meterRead24Months.Where(x =>
					x.MeterReadingReasonID == MeterReadingReason.PeriodicMeterReading
					|| x.MeterReadingReasonID == MeterReadingReason.InterimMeterReadingWithBilling
					|| x.MeterReadingReasonID == MeterReadingReason.MeterReadingAtMoveOut).ToArray();
				billingData.HasNotPendingBillsToBeIssued =
					readingResultList.Any(x => x.MeterReadingStatusID != MeterReadingStatus.Billed);

				billingData.IsUnEstimatedReading =
					readingResultList.Count(x =>
						x.MeterReadingStatusID == MeterReadingStatus.Billed &&
						!((MeterReadingCategoryType) x.MeterReadingCategoryID).IsEstimation) >= 2;
			}

			
		}

		
		private async Task<GeneralBillingInfo> GetClosedBillingInfo(AccountInfo accountInfo, ContractDto latestContract)
		{

			var billingData = new GeneralBillingInfo();
			
			billingData.CurrentBalanceAmount =
				latestContract?.ContractAccount?.ContractAccountBalance?.CurrentBalance ?? EuroMoney.Zero;
			billingData.PaymentMethod = accountInfo.PaymentMethod;
			billingData.MeterReadingType = MeterReadingCategoryType.None;
			var contractItems = accountInfo.BusinessAgreement.Contracts.FirstOrDefault();

			billingData.NextBillDate = contractItems?.ContractStartDate ?? DateTime.Now.AddDays(60);

			
			var device = latestContract?.Devices.FirstOrDefault();

			billingData.IsDeviceRefundAvailable = device == null || ResolveDeviceRefund(device, accountInfo.ClientAccountType);

			return billingData;

		
		}

		

		public bool ResolveDeviceRefund(DeviceDto device, ClientAccountType clientAccountType)
		{
			MeterReadingResultDto meterReadingResult ;

			if (clientAccountType == ClientAccountType.Electricity)
				meterReadingResult =
					device.MeterReadingResults.Any() ? device.MeterReadingResults.LastOrDefault() : null;
			else if (clientAccountType == ClientAccountType.Gas)
				meterReadingResult = device.MeterReadingResults.LastOrDefault(x =>
					x.RegisterID == MeterReadingRegisterType.ActiveEnergyRegisterType);
			else
				throw new InvalidOperationException();

			return meterReadingResult != null &&
			       meterReadingResult.MeterReadingCategoryID == MeterReadingCategoryType.Network;
		}

		
	}
}