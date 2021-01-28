using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.DomainServices.Queries.Billing.Info;
using EI.RP.DomainServices.Queries.Billing.NextBill;
using EI.RP.CoreServices.System;
using EI.RP.UiFlows.Core.Flows;

using EI.RP.WebApp.Flows.AppFlows.MakeAPayment.FlowDefinitions;
using System.Collections.Generic;
using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;

namespace EI.RP.WebApp.Flows.AppFlows.MakeAPayment.Steps
{
    public class PaymentGatewayInteractionAndMainOptions : PaymentsScreen
    {
	    private readonly IDomainQueryResolver _domainQueryResolver;

        public static class StepEvent
	    {
		    public static readonly ScreenEvent UserRequestedToPayDifferentAmount =
			    new ScreenEvent(nameof(PaymentGatewayInteractionAndMainOptions),nameof(UserRequestedToPayDifferentAmount));

		    public static readonly ScreenEvent PaymentGateway =
			    new ScreenEvent(nameof(PaymentGatewayInteractionAndMainOptions), nameof(UserRequestedToPayDifferentAmount));
	    }

        public PaymentGatewayInteractionAndMainOptions(IDomainQueryResolver queryResolver)
        {
            _domainQueryResolver = queryResolver;
        }

        public override ScreenName ScreenStep => PaymentStep.PaymentGatewayInteractionAndMainOptions;

        protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
            IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
        {
            return screenConfiguration
                .OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
                .OnEventNavigatesTo(StepEvent.UserRequestedToPayDifferentAmount, PaymentStep.ChangePaymentAmount);
        }

        protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
        {
            var rootData = contextData.GetStepData<PaymentFlowInitializer.RootScreenModel>(ScreenName.PreStart);

            var stepData = new ScreenModel
            {
	            AccountNumber = rootData.AccountNumber,
                ShowLatestBillDate = true
            };

            SetTitle(Title, stepData);

            await BuildStepData(stepData, rootData);

            return stepData;
        }

        private async Task BuildStepData(ScreenModel screenModel, PaymentFlowInitializer.RootScreenModel rootData)
        {
            var billingInfo =
                await _domainQueryResolver.GetAccountBillingInfoByAccountNumber(screenModel.AccountNumber);

            screenModel.CurrentBalanceAmountLabel = "Amount Due";
            screenModel.CurrentBalanceAmount = billingInfo?.CurrentBalanceAmount;

            var estimateYourCostCalculation =
                await _domainQueryResolver.GetNextBillEstimationByAccountNumber(screenModel.AccountNumber);

            if (estimateYourCostCalculation.CostCanBeEstimated
                && rootData.StartType == PaymentFlowInitializer.StartType.FromEstimateCost)
            {
                screenModel.CurrentBalanceAmount = estimateYourCostCalculation.EstimatedAmount;
            }
        }

        protected override async Task<UiFlowScreenModel> OnRefreshStepDataAsync(IUiFlowContextData contextData,
	        UiFlowScreenModel originalScreenModel,
	        IDictionary<string, object> stepViewCustomizations = null)
        {
            var refreshedStepData = originalScreenModel.CloneDeep();
            var updatedStepData = (ScreenModel)refreshedStepData;

            SetTitle(Title, updatedStepData);

            return updatedStepData;
        }

        public class ScreenModel : UiFlowScreenModel
        {
            public override bool IsValidFor(ScreenName screenStep)
            {
                return screenStep == PaymentStep.PaymentGatewayInteractionAndMainOptions;
            }

            public string AccountNumber { get; set; }

            public string CurrentBalanceAmountLabel { get; set; }
            public EuroMoney CurrentBalanceAmount { get; set; }

            public bool? ShowLatestBillDate { get; set; }
        }
    }
}