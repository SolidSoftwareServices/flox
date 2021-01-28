using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Sap.ErpUmc.Dtos;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.Billing;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Queries.Billing.Info;

namespace EI.RP.DomainServices.Queries.Billing.Activity
{
	internal class AccountBillingActivityQueryHandler : QueryHandler<AccountBillingActivityQuery>
	{
		private readonly ISapRepositoryOfErpUmc _erpRepository;
		private readonly IDomainQueryResolver _queryResolver;

		public AccountBillingActivityQueryHandler(ISapRepositoryOfErpUmc erpRepository,
			IDomainQueryResolver queryResolver)
		{
			_erpRepository = erpRepository;
			_queryResolver = queryResolver;
		}

		protected override Type[] ValidQueryResultTypes { get; } = {typeof(AccountBillingActivity)};


		protected override async Task<IEnumerable<TQueryResult>> _ExecuteQueryAsync<TQueryResult>(
			AccountBillingActivityQuery query)
		{
			var accountBillingActivities = GetInvoicesActivity(query);
			var result = (await GetPaymentsActivity(query)).Union(await accountBillingActivities);

			return result.Cast<TQueryResult>();
		}

		private async Task<IEnumerable<AccountBillingActivity>> GetPaymentsActivity(
			AccountBillingActivityQuery queryModel)
		{
			IEnumerable<AccountBillingActivity> result = new AccountBillingActivity[0];

			if (queryModel.Source == AccountBillingActivity.ActivitySource.All ||
			    queryModel.Source == AccountBillingActivity.ActivitySource.Payment)
			{
				var payments = await _erpRepository
					.NewQuery<ContractAccountDto>()
					.Key(queryModel.AccountNumber)
					.NavigateTo<PaymentDocumentDto>()
					.Filter(x => x.ExecutionDate >= queryModel.MinDate && x.ExecutionDate <= queryModel.MaxDate,false)
					.GetMany();
				
				result = payments.Select(Map);
			}

			return result;
		}

		private async Task<IEnumerable<AccountBillingActivity>> GetInvoicesActivity(
			AccountBillingActivityQuery queryModel)
		{
			IEnumerable<AccountBillingActivity> result = new AccountBillingActivity[0];

			if (queryModel.Source == AccountBillingActivity.ActivitySource.All ||
			    queryModel.Source == AccountBillingActivity.ActivitySource.Invoice)
			{
				var invoicesTask = _erpRepository
					.NewQuery<ContractAccountDto>()
					.Key(queryModel.AccountNumber)
					.NavigateTo<InvoiceDto>()
					.Filter(x => x.InvoiceDate >= queryModel.MinDate && x.InvoiceDate <= queryModel.MaxDate,false)
					.GetMany();

				var accountBillingTask =
					_queryResolver.GetAccountBillingInfoByAccountNumber(queryModel.AccountNumber, true);
				var billingInfo = await accountBillingTask;
				var invoice = await invoicesTask;
				
				result = invoice.Select(dto => Map(dto, billingInfo));
			}

			return result;
		}

		private AccountBillingActivity Map(PaymentDocumentDto payment)
		{
			var result = new AccountBillingActivity(AccountBillingActivity.ActivitySource.Payment);
			result.DueAmount = EuroMoney.Zero;
			result.Description = payment.PaymentDocumentText;
			result.DueDate = result.OriginalDate = payment.ExecutionDate ?? default(DateTime);
			result.ReceivedAmount = payment.Amount;

			return result;
		}

		private AccountBillingActivity Map(InvoiceDto invoice, GeneralBillingInfo billingInfo)
		{
			var result = new AccountBillingActivity(AccountBillingActivity.ActivitySource.Invoice)
			{
				AccountNumber = invoice.AccountID,
				Amount = invoice.AmountDue,
				
				Description = invoice.InvoiceText,
				OriginalDate = invoice.InvoiceDate,
				DueDate = invoice.DueDate,
				PrintDate = invoice.PrintDate,
				InvoiceFileAvailable = invoice.PrintDate.HasValue &&
				                       invoice.IsBill(),
				InvoiceStatus=(InvoiceStatus)invoice.InvoiceStatusID,
                SubstituteDocument = invoice.SubstituteDocument,
			};

			if (result.InvoiceFileAvailable) result.ReferenceDocNumber = invoice.ReferenceDocNo;

			ResolveAmounts(result);

			result.CurrentBalanceAmount = billingInfo.CurrentBalanceAmount;
			
			result.NextBillDate = billingInfo.NextBillDate;
			result.LastBillIsBilled = billingInfo.HasNotPendingBillsToBeIssued;
			result.IsUnEstimatedReading = billingInfo.IsUnEstimatedReading;
			result.PaymentMethod = billingInfo.PaymentMethod;
			result.EqualizerAmount = billingInfo.EqualizerAmount;
			result.EqualizerNextBillDate = billingInfo.EqualizerNextBillDate;
			result.ReadingType = billingInfo.MeterReadingType;
			return result;

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

				accountBill.RemainingAmount = invoice.AmountRemaining;
			}
		}
	}
}