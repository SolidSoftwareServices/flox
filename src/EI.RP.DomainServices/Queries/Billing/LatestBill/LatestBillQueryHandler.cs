using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.DeliveryPipeline.ManualTesting;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Sap.ErpUmc.Dtos;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.Billing;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.InternalShared.Contracts;
using EI.RP.DomainServices.Queries.Billing.Activity;
using EI.RP.DomainServices.Queries.Billing.Info;
using EI.RP.DomainServices.Queries.Contracts.Accounts;

namespace EI.RP.DomainServices.Queries.Billing.LatestBill
{
	internal class LatestBillQueryHandler : QueryHandler<LatestBillQuery>
	{
		private readonly ISapRepositoryOfErpUmc _erpRepository;
		private readonly IDomainQueryResolver _queryResolver;
		private readonly IConfigurableTestingItems _configurableTestingItems;
		private readonly IContractQueryResolver _contractQueryResolver;


		public LatestBillQueryHandler(ISapRepositoryOfErpUmc erpRepository, 
			IDomainQueryResolver queryResolver,IConfigurableTestingItems configurableTestingItems
			,IContractQueryResolver contractQueryResolver)
		{
			_erpRepository = erpRepository;

			_queryResolver = queryResolver;
			_configurableTestingItems = configurableTestingItems;
			_contractQueryResolver = contractQueryResolver;
		}

		protected override Type[] ValidQueryResultTypes { get; } = {typeof(LatestBillInfo)};


		protected override async Task<IEnumerable<TQueryResult>> _ExecuteQueryAsync<TQueryResult>(
			LatestBillQuery query)
		{
			var latestBillInfo = await ResolveForElectricityAndGas(query) ??
			                 await ResolveForEnergyService(query);

			
			if(latestBillInfo==null)
				throw new NotSupportedException(
					"accountInfo.AccountType is not supported");

            //TODO: Fields to be populated with Smart SAP data
            PopulateMocks(latestBillInfo);

			return latestBillInfo.ToOneItemArray().Cast<TQueryResult>();
		}

		private async Task<LatestBillInfo> ResolveForElectricityAndGas(LatestBillQuery query)
		{
			var getAccount = _queryResolver.GetAccountInfoByAccountNumber(query.AccountNumber, true);
			var accountBillingTask =
				_queryResolver.GetAccountBillingInfoByAccountNumber(query.AccountNumber, true);
			var getBillingActivities = _queryResolver.GetInvoicesByAccountNumber(query.AccountNumber, byPassPipeline: true);
			AccountInfo accountInfo = await getAccount;
			if (!accountInfo.ClientAccountType.IsElectricity()&& !accountInfo.ClientAccountType.IsGas()) return null;

			var billingActivities = await getBillingActivities;

			var accountBillingActivities =
				billingActivities.OrderByDescending(x => x.OriginalDate).ToArray();
			var accountBillingActivity = accountBillingActivities.FirstOrDefault(x => x.IsBill());
			var data = new LatestBillInfo();
			data.MeterReadingType = accountBillingActivity?.ReadingType;
			data.ReferenceDocNumber =
				accountBillingActivity != null ? accountBillingActivity.ReferenceDocNumber : string.Empty;
			data.InvoiceFileAvailable = accountBillingActivity?.InvoiceFileAvailable ?? false;
			data.Amount = accountBillingActivity != null ? accountBillingActivity.Amount : EuroMoney.Zero;
			data.HasNotPendingBillsToBeIssued =
				accountBillingActivity != null && accountBillingActivity.LastBillIsBilled;

			data.CurrentBalanceAmount = accountBillingActivity != null
				? accountBillingActivity.CurrentBalanceAmount
				: EuroMoney.Zero;

			data.EqualizerAmount =
				accountBillingActivity != null ? accountBillingActivity.EqualizerAmount : EuroMoney.Zero;
			data.EqualizerNextBillDate = accountBillingActivity?.EqualizerNextBillDate;

			if (accountBillingActivity != null)
			{
				data.DueDate = accountBillingActivity.DueDate;
				data.LatestBillDate = accountBillingActivity.OriginalDate;
				data.NextBillDate = accountBillingActivity.NextBillDate;
				data.PaymentMethod = accountBillingActivity.PaymentMethod;
				data.IsUnEstimatedReading = accountBillingActivity.IsUnEstimatedReading;
			}

			
			if (accountBillingActivity == null)
			{
				data = LatestBillInfo.From(await accountBillingTask);
				data.Amount = data.Amount == null ? EuroMoney.Zero : data.Amount;
				data.EqualizerAmount = data.EqualizerAmount == null ? EuroMoney.Zero : data.EqualizerAmount;
			}

			data.AccountIsOpen = accountInfo.IsOpen;
			data.HasAccountCredit = data.CurrentBalanceAmount.Amount < 0 && !accountInfo.IsPAYGCustomer;
			data.AccountNumber = accountInfo.AccountNumber;

			if (data.PaymentMethod == PaymentMethodType.Equalizer) data.NextBillDate = data.LatestBillDate.AddDays(60);

			data.CostCanBeEstimated = CanCostBeEstimated(accountBillingActivities, data, accountInfo);

			

			var contractItemTask = _queryResolver.GetAccounts();
			var numberOfAccounts = (await contractItemTask).Count(x => x.ContractEndDate > DateTime.Now);

			var businessAgreement = accountInfo.BusinessAgreement;
			data.CanAddGasAccount = await CanAddGasAccountAsync(businessAgreement, accountInfo);
			data.CanRequestRefund = data.CurrentBalanceAmount < -10 && CanBeRefunded(businessAgreement,
				                        await accountBillingTask, accountInfo, numberOfAccounts);

			return data;
		}

        private  void PopulateMocks(LatestBillInfo latestBillInfo)
        {
            latestBillInfo.CanShowCostToDate = _configurableTestingItems.CanShowCostToDate;
            latestBillInfo.CostToDateAmount = _configurableTestingItems.CostToDateAmount;
            latestBillInfo.CostToDateSince = _configurableTestingItems.CostToDateSince;
        }

		private bool CanCostBeEstimated(AccountBillingActivity[] accountBillingActivities,
			LatestBillInfo data, AccountInfo account)
		{
			var accountBillingNonPaymentDescList = accountBillingActivities.Where(x => !x.IsPayment()).ToArray();
			var firstBill = !accountBillingNonPaymentDescList.Any();
			var accountBillingTop6 = accountBillingNonPaymentDescList.Take(6);
			var result = false;
			if (data.PaymentMethod != PaymentMethodType.Equalizer && data.IsUnEstimatedReading &&
			    accountBillingTop6.Any(x => x.Amount.Amount > 0) &&
			    !firstBill && !account.IsPAYGCustomer)
			{
				var date = DateTime.Now;
				if (data.LatestBillDate.AddDays(14) <= date && data.NextBillDate.AddDays(-14) >= date)
					result = true;
			}

			return result;
		}

		private bool CanBeRefunded(BusinessAgreement businessAgreement,
			GeneralBillingInfo data, AccountInfo account, int numberOfOpenedAccounts)
		{
			return IsPayer()
			       && businessAgreement.IncomingPaymentMethodType != PaymentMethodType.Equalizer
			       && !account.IsPAYGCustomer
			       && HasRefundableAmount()
			       && data.IsDeviceRefundAvailable;

			bool IsPayer()
			{
				return string.IsNullOrEmpty(businessAgreement.CollectiveParentId)
				       && string.IsNullOrEmpty(businessAgreement.AlternativePayerId);
			}

			bool HasRefundableAmount()
			{
				return !(data.CurrentBalanceAmount.Amount > 0 && account.ContractEndDate > DateTime.Now) ||
				       data.CurrentBalanceAmount.Amount < 0 && numberOfOpenedAccounts == 1;
			}
		}

		private async Task<bool> CanAddGasAccountAsync(BusinessAgreement businessAgreement, AccountInfo account)
		{
			var contracts = await _contractQueryResolver.GetAllContracts(account);
			var contractDtos = contracts as ContractDto[] ?? contracts.ToArray();
			var latestContract = contractDtos.FirstOrDefault();


			if (latestContract?.Premise != null)
			{
				var installationFact = latestContract.Premise.Installations.FirstOrDefault()?.InstallationFacts
					.Where(x => x.Operand == OperandType.FirstStaffDiscount);
				var contractItem = businessAgreement.Contracts.FirstOrDefault();

				return installationFact?.Count() == 0 && account.ContractEndDate == SapDateTimes.SapDateTimeMax &&
				       businessAgreement.AccountDeterminationID == AccountDeterminationType.ResidentialCustomer &&
				       contractItem.Division == DivisionType.Electricity &&
				       string.IsNullOrEmpty(account.BundleReference) &&
				       !contractItem.ProductType.IsPAYGPRoduct();
			}

			return false;
		}

		
		#region Energy Service

		private async Task<LatestBillInfo> ResolveForEnergyService(LatestBillQuery query)
		{
			var getAccount = _queryResolver.GetAccountInfoByAccountNumber(query.AccountNumber, true);
			var accountBillingActivityQuery = new AccountBillingActivityQuery();
			accountBillingActivityQuery.AccountNumber = query.AccountNumber;

			var data = new LatestBillInfo();


			var mapTasks = Task.WhenAll(MapFromInvoices(), MapFromPayment());

			var accountInfo = (await getAccount);
			if(accountInfo.ClientAccountType != ClientAccountType.EnergyService) return null;
			data.PaymentMethod = accountInfo.PaymentMethod;

			await mapTasks;
			return data;

			async Task MapFromInvoices()
			{
				var accountBillingActivitiesTask = GetInvoicesActivity(accountBillingActivityQuery);
				var accountBillingActivities = await accountBillingActivitiesTask;


				var billingActivities = accountBillingActivities as AccountBillingActivity[] ??
				                        accountBillingActivities.ToArray();
				var latestInvoice = billingActivities.OrderByDescending(x => x.OriginalDate)
					.FirstOrDefault(x => !x.IsPayment() && ESAProductType.CanParse(x.Description));

				data.AccountDescription = latestInvoice?.Description;
				data.CurrentBalanceAmount = latestInvoice?.CurrentBalanceAmount;
			}

			async Task MapFromPayment()
			{
				var getPayments = await GetPaymentsActivity(accountBillingActivityQuery);
				var lastPayment = getPayments.OrderByDescending(x => x.OriginalDate)
					.FirstOrDefault(x => x.IsPayment());
				data.Amount = lastPayment?.ReceivedAmount;
				data.DueDate = lastPayment?.OriginalDate ?? default(DateTime);
			}
		}


		private async Task<IEnumerable<AccountBillingActivity>> GetPaymentsActivity(
			AccountBillingActivityQuery queryModel)
		{
			IEnumerable<AccountBillingActivity> result = new AccountBillingActivity[0];

			if (queryModel.Source == AccountBillingActivity.ActivitySource.All ||
			    queryModel.Source == AccountBillingActivity.ActivitySource.Payment)
			{
				var query = _erpRepository
					.NewQuery<ContractAccountDto>()
					.Key(queryModel.AccountNumber)
					.NavigateTo<PaymentDocumentDto>()
					.Filter(x => x.ExecutionDate >= queryModel.MinDate && x.ExecutionDate <= queryModel.MaxDate,false);
				var payments = await _erpRepository.GetMany(query);
				result = payments.Select(Map);
			}

			return result;
		}

		private AccountBillingActivity Map(PaymentDocumentDto payment)
		{
			var result = new AccountBillingActivity(AccountBillingActivity.ActivitySource.Payment);
			result.DueAmount = EuroMoney.Zero;
			result.Description = payment.PaymentDocumentText;

			var paymentExecutionDate = payment.ExecutionDate ?? default(DateTime);
			result.DueDate = paymentExecutionDate;
			result.OriginalDate = paymentExecutionDate;
			result.ReceivedAmount = payment.Amount;

			return result;
		}

		private async Task<IEnumerable<AccountBillingActivity>> GetInvoicesActivity(
			AccountBillingActivityQuery queryModel)
		{
			IEnumerable<AccountBillingActivity> result = new AccountBillingActivity[0];

			if (queryModel.Source == AccountBillingActivity.ActivitySource.All ||
			    queryModel.Source == AccountBillingActivity.ActivitySource.Invoice)
			{
				var query = _erpRepository
					.NewQuery<ContractAccountDto>()
					.Key(queryModel.AccountNumber)
					.NavigateTo<InvoiceDto>()
					.Filter(x => x.InvoiceDate >= queryModel.MinDate && x.InvoiceDate <= queryModel.MaxDate,false);
				var invoicesTask = _erpRepository.GetMany(query);

				var accountBillingTask =
					_queryResolver.GetAccountBillingInfoByAccountNumber(
						queryModel.AccountNumber, true);
				var billingInfo = await accountBillingTask;
				var invoice = await invoicesTask;

				result = invoice.Select(dto => Map(dto, billingInfo));
			}

			return result;
		}

		private AccountBillingActivity Map(InvoiceDto invoice, GeneralBillingInfo billingInfo)
		{
			void ResolveAmounts(AccountBillingActivity accountBill)
			{
				if (invoice.AmountDue < 0)
				{
					accountBill.ReceivedAmount = Math.Abs(invoice.AmountDue);
					accountBill.DueAmount = EuroMoney.Zero;
				}
				else
				{
					accountBill.ReceivedAmount = EuroMoney.Zero;
					accountBill.DueAmount = invoice.AmountDue;
				}
			}

			var result = new AccountBillingActivity(AccountBillingActivity.ActivitySource.Invoice)
			{
				AccountNumber = invoice.AccountID,
				Amount = invoice.AmountDue,
				Description = invoice.InvoiceText,
				OriginalDate = invoice.InvoiceDate,
				DueDate = invoice.DueDate,
				PrintDate = invoice.PrintDate,
				InvoiceFileAvailable = invoice.PrintDate.HasValue && invoice.IsBill()
			};

			if (result.InvoiceFileAvailable) result.ReferenceDocNumber = invoice.ReferenceDocNo;
			ResolveAmounts(result);

			result.CurrentBalanceAmount = billingInfo.CurrentBalanceAmount;
			return result;
		}

		#endregion
	}
}