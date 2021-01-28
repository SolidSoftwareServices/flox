using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.Billing;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Queries.Billing.Activity;
using EI.RP.DomainServices.Queries.Contracts.Accounts;

namespace EI.RP.DomainServices.Queries.Billing.NextBill
{
	internal class NextBillEstimationQueryHandler : QueryHandler<EstimateNextBillQuery>
	{
		private readonly IDomainQueryResolver _queryResolver;

		public NextBillEstimationQueryHandler(IDomainQueryResolver queryResolver)
		{
			_queryResolver = queryResolver;
		}


		protected override Type[] ValidQueryResultTypes { get; } = {typeof(NextBillEstimation)};


		protected override async Task<IEnumerable<TQueryResult>> _ExecuteQueryAsync<TQueryResult>(
			EstimateNextBillQuery queryModel)
		{
			var accountNumber = queryModel.AccountNumber;
			var invoicesTask =
				_queryResolver.GetInvoicesByAccountNumber(accountNumber, byPassPipeline: true);
			var accountTask = _queryResolver.GetAccountInfoByAccountNumber(accountNumber, true);
			var account = await accountTask;

			var invoicesSorted = (await invoicesTask).OrderByDescending(x => x.OriginalDate).ToArray();
			var lastBill = invoicesSorted.FirstOrDefault(x => x.IsBill());
			var data = new NextBillEstimation();


			data.NextBillDate = lastBill?.NextBillDate ??
			                    (account.ContractStartDate?.AddDays(60) ?? DateTime.Now.AddDays(60));

			data.AccountNumber = accountNumber;

			MapEstimateCost(invoicesSorted, data, account);


			return data.ToOneItemArray().Cast<TQueryResult>();
		}

		private void MapEstimateCost(AccountBillingActivity[] invoicesSorted,
			NextBillEstimation nextBillEstimation, AccountInfo account)
		{
			var lastBill = invoicesSorted.FirstOrDefault(x => x.IsBill());

			var accountBillingNonPaymentDescList = invoicesSorted.Where(x => !x.IsPayment()).ToArray();
			var isFirstBill = !accountBillingNonPaymentDescList.Any();
			var accountBillingTop6 = accountBillingNonPaymentDescList.Take(6);

			if (lastBill != null && lastBill?.PaymentMethod != PaymentMethodType.Equalizer && (lastBill?.IsUnEstimatedReading ?? true) &&
			    accountBillingTop6.Any(x => x.Amount.Amount > 0) &&
			    !isFirstBill && !account.IsPAYGCustomer)
			{
				var date = DateTime.Now;
				if (lastBill.OriginalDate.AddDays(14) <= date &&
				    nextBillEstimation.NextBillDate.AddDays(-14) >= date)
					nextBillEstimation.CostCanBeEstimated = true;
			}


			CalculateEstimation(invoicesSorted, nextBillEstimation, account, lastBill);
		}

		private static void CalculateEstimation(AccountBillingActivity[] invoicesSorted,
			NextBillEstimation nextBillEstimation, AccountInfo account,
			AccountBillingActivity lastBill)
		{
			if (nextBillEstimation.CostCanBeEstimated)
			{
				var sortedBills = invoicesSorted.Where(x => x.IsBill());
				var accountBillingActivitiesTop6 = sortedBills.Take(6).ToArray();
				var accountBillingActivitiesTop7 = sortedBills.Take(7).ToArray();

				var totalBillAmount = accountBillingActivitiesTop6.Select(x => x.Amount.Amount).Sum().Value;

				var firstInvoiceDate = accountBillingActivitiesTop7.FirstOrDefault() == null
					? DateTime.Now
					: accountBillingActivitiesTop7.First().OriginalDate;
				var lastInvoiceDate = accountBillingActivitiesTop7.LastOrDefault() == null
					? account.ContractStartDate.Value
					: accountBillingActivitiesTop7.Last().OriginalDate;
				var billingPeriod = (decimal) (firstInvoiceDate - lastInvoiceDate).TotalDays;

				decimal dailyAmount;
				if (billingPeriod != 0)
					dailyAmount = totalBillAmount / billingPeriod;
				else
					dailyAmount = 0;

				var noOfDays = (decimal) (DateTime.Now.Date - firstInvoiceDate).TotalDays - 1;
				var paymentAmount = (EuroMoney) (dailyAmount * noOfDays);
				var outstandingAmount = lastBill?.CurrentBalanceAmount ?? 0d;
				nextBillEstimation.EstimatedAmount = outstandingAmount + paymentAmount;
			}
		}
	}
}