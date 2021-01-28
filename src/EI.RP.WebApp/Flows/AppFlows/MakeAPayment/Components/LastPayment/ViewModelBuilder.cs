using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.DomainServices.Queries.Billing.Activity;
using EI.RP.DomainServices.Queries.Billing.Info;
using EI.RP.DomainServices.Queries.Billing.NextBill;


using System.Linq;
using Ei.Rp.DomainModels.Billing;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;
using EI.RP.WebApp.Flows.AppFlows.MakeAPayment.Steps;

namespace EI.RP.WebApp.Flows.AppFlows.MakeAPayment.Components.LastPayment
{
	internal class ViewModelBuilder : IComponentViewModelBuilder<InputModel, ViewModel>
	{
		private readonly IDomainQueryResolver _domainQueryResolver;

		public ViewModelBuilder(IDomainQueryResolver queryResolver)
		{
			_domainQueryResolver = queryResolver;
		}

		public async Task<ViewModel> Resolve(InputModel componentInput, UiFlowScreenModel screenModelContainingTheComponent = null)
		{
            var accountNumber = componentInput.AccountNumber;

            var billingActivitiesTask =
                _domainQueryResolver.GetInvoicesByAccountNumber(accountNumber);

            var accountBillingActivities = 
                (await billingActivitiesTask).OrderByDescending(x => x.OriginalDate).ToArray();

            var result = new ViewModel()
            {
                ShowPayDifferentAmount = componentInput.ShowPayDifferentAmount,
                ShowDatesTable = componentInput.ShowDatesTable
            };

            SetLatestBillDate();

            await SetBillingInfo();

            await SetAmountText();

            return result;

            void SetLatestBillDate()
            {
                var lastBill = accountBillingActivities.FirstOrDefault(x =>
                    x.Source == AccountBillingActivity.ActivitySource.Invoice && x.IsBill());

                if (lastBill != null && componentInput.ShowLatestBillDate)
                {
                    result.LatestBillDate = componentInput.LatestBillDate ?? lastBill.OriginalDate;
                }
            }

            async Task SetBillingInfo()
            {
                var billingInfo = await _domainQueryResolver.GetAccountBillingInfoByAccountNumber(accountNumber);

                result.CurrentBalanceAmount = componentInput.CurrentBalanceAmount ?? billingInfo?.CurrentBalanceAmount;

                if (componentInput.ShowNextBillDate)
                {
                    result.NextBillDate = componentInput.NextBillDate ?? billingInfo?.NextBillDate;
                }

                var accountBillingActivity = accountBillingActivities.FirstOrDefault(x => !x.IsPayment());
                if (accountBillingActivity != null && componentInput.ShowDueDate)
                {
                    result.DueDate = componentInput.DueDate ?? accountBillingActivity.DueDate;
                }
            }

            async Task SetAmountText()
            {
                var estimateYourCostCalculation = 
                    await _domainQueryResolver.GetNextBillEstimationByAccountNumber(accountNumber);

                if (estimateYourCostCalculation.CostCanBeEstimated 
                    && screenModelContainingTheComponent.GetContainedFlowStartType()== PaymentFlowInitializer.StartType.FromEstimateCost.ToString())
                {
                    result.CurrentBalanceAmountLabel = componentInput.CurrentBalanceAmountLabel ?? "Amount to Pay";
                    result.CurrentBalanceAmount = componentInput.CurrentBalanceAmount ?? estimateYourCostCalculation.EstimatedAmount;
                }
                else
                { 
                    result.CurrentBalanceAmountLabel = componentInput.CurrentBalanceAmountLabel ?? "Amount Due";
                }
            }
		}
	}
}