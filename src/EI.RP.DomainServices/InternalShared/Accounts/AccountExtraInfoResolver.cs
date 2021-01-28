using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DataModels.Sap.CrmUmc.Dtos.Extensions;
using EI.RP.DataModels.Sap.ErpUmc.Dtos;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.InternalShared.Contracts;
using AccountDto = EI.RP.DataModels.Sap.CrmUmc.Dtos.AccountDto;

namespace EI.RP.DomainServices.InternalShared.Accounts
{
	internal class AccountExtraInfoResolver : IAccountExtraInfoResolver
	{
		private readonly ISapRepositoryOfCrmUmc _repository;
		private readonly IContractQueryResolver _contractQueryResolver;
		private readonly DateTime _topDateTime = new DateTime(9999, 12, 31);

		public AccountExtraInfoResolver(
			ISapRepositoryOfCrmUmc repository,
			IContractQueryResolver contractQueryResolver)
		{
			_repository = repository;
			_contractQueryResolver = contractQueryResolver;
		}

		public async Task<bool> CanEqualizePayments(AccountInfo accountInfo)
		{
			//TODO: DEFAULT TO FALSE (Sukeerthi)
			var canEqualizePayments = true;

			var businessAgreement = accountInfo.BusinessAgreement;

			if (!accountInfo.IsSmart() && 
			    businessAgreement.AllowsEqualizer() && 
			    !accountInfo.IsPAYGCustomer &&
				accountInfo.ContractEndDate == SapDateTimes.SapDateTimeMax)
			{
				var contracts = await _contractQueryResolver.GetAllContracts(accountInfo);
				var contractDtos = contracts as ContractDto[] ?? contracts.ToArray();
				var latestContract = contractDtos.FirstOrDefault();

				var isSixMonthsConsumptionData = false;
				var isReadingDateTimeDataPastYear = true;
				var isTwelveMonthsGasConsumptionData = false;
				var isMeterReadingDetailsBilled = true;
				var isStaffDiscount = false;

				if (accountInfo.ClientAccountType == ClientAccountType.Electricity)
					isSixMonthsConsumptionData = GetElectricityConsumptionInfo(latestContract, false,
						out isMeterReadingDetailsBilled, out isReadingDateTimeDataPastYear);
				else if (accountInfo.ClientAccountType == ClientAccountType.Gas)
					isTwelveMonthsGasConsumptionData = IsTwelveMonthsGasConsumptionData(latestContract, false);
				else
					throw new InvalidOperationException();

				isStaffDiscount = IsStaffDiscount(latestContract, false);

				var currentBalanceAmount = latestContract != null
					? (EuroMoney)latestContract.ContractAccount.ContractAccountBalance.CurrentBalance
					: EuroMoney.Zero;
				if (accountInfo.ClientAccountType == ClientAccountType.Electricity &&
					 ((double)currentBalanceAmount.Amount <= 0 ||
					  (double)currentBalanceAmount.Amount > 400.00 ||
					  !isSixMonthsConsumptionData ||
					  !isReadingDateTimeDataPastYear ||
					  isStaffDiscount ||
					  !isMeterReadingDetailsBilled))
					canEqualizePayments = false;

				if (accountInfo.ClientAccountType == ClientAccountType.Gas &&
					((double)currentBalanceAmount.Amount <= 0 ||
					 (double)currentBalanceAmount.Amount > 500.00 ||
					 !isTwelveMonthsGasConsumptionData ||
					 isStaffDiscount))
					canEqualizePayments = false;
			}
			else
			{
				canEqualizePayments = false;
			}

			return canEqualizePayments;

			

			bool IsStaffDiscount(ContractDto latestContract, bool isStaffDiscount)
			{
				if (latestContract?.Premise.Installations != null)
					foreach (var installation in latestContract?.Premise.Installations)
						isStaffDiscount =
							installation.InstallationFacts.Any(a => a.Operand == OperandType.FirstStaffDiscount) &&
							!isStaffDiscount;

				return isStaffDiscount;
			}

			bool IsTwelveMonthsGasConsumptionData(ContractDto latestContract,
				bool isTwelveMonthsGasConsumptionData)
			{
				foreach (var installation in latestContract.Premise.Installations)
					isTwelveMonthsGasConsumptionData =
						installation.InstallationFacts.Any(a => a.Operand == OperandType.AnnualConsumption) &&
						!isTwelveMonthsGasConsumptionData;

				return isTwelveMonthsGasConsumptionData;
			}

			bool GetElectricityConsumptionInfo(ContractDto latestContract, bool isSixMonthsConsumptionData,
				out bool isMeterReadingDetailsBilled, out bool isReadingDateTimeDataPastYear)
			{
				var meterReadingResultCount = 0;
				isReadingDateTimeDataPastYear = true;
				isMeterReadingDetailsBilled = true;
				var devices = latestContract.Devices;
				if (devices != null)
					foreach (var device in devices)
					{
						var latestReadingDateTime = device.MeterReadingResults.Select(x => x.ReadingDateTime)
							.OrderByDescending(x => x).FirstOrDefault();
						var meterReadingResult = device.MeterReadingResults
							.FirstOrDefault(x => x.ReadingDateTime == latestReadingDateTime);

						if (meterReadingResult != null)
						{
							var timeDifference = DateTime.Now - meterReadingResult.ReadingDateTime.Value;
							if (timeDifference.TotalDays < 180 && !isSixMonthsConsumptionData)
								isSixMonthsConsumptionData = true;

							if (isMeterReadingDetailsBilled &&
								meterReadingResult.MeterReadingStatusID != MeterReadingStatus.Billed)
								isMeterReadingDetailsBilled = false;
						}

						meterReadingResultCount = device.MeterReadingResults.Count(x =>
							x.MeterReadingCategoryID == MeterReadingCategoryType.Network &&
							DateTime.Now.Subtract(x.ReadingDateTime.Value).TotalDays < 365);
					}

				if (meterReadingResultCount < 2) isReadingDateTimeDataPastYear = false;

				return isSixMonthsConsumptionData;
			}
		}

		

		public async Task<bool> AddressesMatchForBundle(string accountNumber)
		{

			//TODO: it smells like we only needed one query here
			var contractsTask = GetContracts(accountNumber);
			var contracts = await contractsTask;

			var topDateTime = new DateTime(9999, 12, 31);
			var sourceContract = contracts.Where(x => !x.ContractEndDate.HasValue || x.ContractEndDate.Value == topDateTime).First();

			return await IsBundleMatchForAddresses(sourceContract);
		}

		private async Task<IEnumerable<ContractItemDto>> GetContracts(string accountNumber)
		{
			var query = _repository.NewQuery<BusinessAgreementDto>()
				.Key(accountNumber)
				.NavigateTo<ContractItemDto>()
				.Filter(x => x.BusinessAgreement.CollectiveParentID == string.Empty,false)
				.Expand(x => x.BusinessAgreement)
				.Expand(x => x.BusinessAgreement.Account)
				.Expand(x => x.Premise)
				.Expand(x => x.Premise.PointOfDeliveries)
				.Expand(x => x.Product)
				.Expand(x => x.ContractItemEXTAttrs);

			var contracts = await _repository.GetMany(query);

			return contracts;
		}

		public async Task<bool> IsBundleMatchForAddresses(ContractItemDto sourceContract)
		{
			var bpContracts = (await _repository.NewQuery<AccountDto>()
				.Key(sourceContract.AccountID)
				.NavigateTo<ContractItemDto>()
				.Expand(x => x.BusinessAgreement)
				.Expand(x => x.BusinessAgreement.Account)
				.Expand(x => x.Premise)
				.Expand(x => x.Premise.PointOfDeliveries)
				.Expand(x => x.Product)
				.Expand(x => x.ContractItemEXTAttrs)
				.GetMany()).ToArray();

			bpContracts = bpContracts
				.Where(x => x.ContractEndDate == _topDateTime && x.DivisionID != sourceContract.DivisionID).ToArray();

			var bundleMatches = new ContractItemDto[0];
			if (!string.IsNullOrWhiteSpace(sourceContract.ContractItemEXTAttrs.BUNDLE_REF))
			{
				bundleMatches = bpContracts.Where(x => x.ContractItemEXTAttrs.BUNDLE_REF == sourceContract.ContractItemEXTAttrs.BUNDLE_REF).ToArray();
			}

			if (!bundleMatches.Any() && bpContracts.Any())
			{
				bundleMatches = bpContracts;
			}

			var bundleMatchesAtSameAddress = bundleMatches
				.Where(x => x.Premise.AddressInfo.IsEquivalentTo(sourceContract.Premise.AddressInfo)).ToArray();

			if (!bundleMatchesAtSameAddress.Any() && bundleMatches.Any())
			{
				return false;
			}

			return true;
		}
	}
}