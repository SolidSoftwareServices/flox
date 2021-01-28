using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Queries.Billing.LatestBill;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;
using EI.RP.CoreServices.System;

namespace EI.RP.WebApp.Flows.AppFlows.Accounts.Components.AccountCardBillingDetails
{
	internal class ViewModelBuilder : IComponentViewModelBuilder<InputModel, ViewModel>
	{
        private readonly IDomainQueryResolver _domainQueryResolver;

        public ViewModelBuilder(IDomainQueryResolver domainQueryResolver)
        {
            _domainQueryResolver = domainQueryResolver;
        }

        public async Task<ViewModel> Resolve(InputModel componentInput, UiFlowScreenModel screenModelContainingTheComponent = null)
        {
            var latestBill = await _domainQueryResolver.GetLatestBillByAccountNumber(componentInput.AccountNumber);

            var result = new ViewModel();
            result.AccountNumber = latestBill.AccountNumber;
            result.AccountType = componentInput.AccountType;
            result.PaymentMethod = latestBill.PaymentMethod;
            result.CanRequestRefund = latestBill.CanRequestRefund;
            result.CanEstimateCost = latestBill.CostCanBeEstimated;
            result.HasAccountCredit = latestBill.HasAccountCredit;
            result.InvoiceFileAvailable = latestBill.InvoiceFileAvailable;
            result.ReferenceDocNumber = latestBill.ReferenceDocNumber;
            result.CurrentBalanceAmount = latestBill.CurrentBalanceAmount;
            result.DueDate = latestBill.DueDate;
            result.EqualizerAmount = latestBill.EqualizerAmount;
            result.EqualizerNextBillDate = latestBill.EqualizerNextBillDate;
            result.BillAmount = latestBill.Amount;
            result.LatestBillDate = latestBill.LatestBillDate;
            result.NextBillDate = latestBill.NextBillDate;
            result.IsDue = latestBill.CurrentBalanceAmount.Amount > 0 &&
                           latestBill.DueDate >= DateTime.Now &&
                           latestBill.PaymentMethod != PaymentMethodType.Equalizer;
            result.IsOverdue = latestBill.CurrentBalanceAmount.Amount > 0 &&
                               latestBill.DueDate < DateTime.Now &&
                               latestBill.PaymentMethod != PaymentMethodType.Equalizer;
            result.AreBillsAllPaid = latestBill.CurrentBalanceAmount <= 0 &&
                                     (componentInput.AccountType == ClientAccountType.Electricity ||
                                      componentInput.AccountType == ClientAccountType.Gas);
            result.CanPayNow = !latestBill.CostCanBeEstimated 
                               && latestBill.CurrentBalanceAmount >= 0
                               && latestBill.PaymentMethod.IsOneOf(PaymentMethodType.DirectDebit,PaymentMethodType.Manual);
            result.CanShowAdditionalMessage = latestBill.PaymentMethod == PaymentMethodType.Equalizer ||
                                              latestBill.PaymentMethod == PaymentMethodType.DirectDebitNotAvailable;
            result.CanShowCostToDate = latestBill.CanShowCostToDate;
            result.CostToDateAmount = latestBill.CostToDateAmount;
            result.CostToDateSince = latestBill.CostToDateSince;

            return result;
		}
    }
}
