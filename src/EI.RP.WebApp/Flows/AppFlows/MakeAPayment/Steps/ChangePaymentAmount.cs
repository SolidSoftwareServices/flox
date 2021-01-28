using System.Collections.Generic;
using EI.RP.CoreServices.System;
using EI.RP.WebApp.Flows.AppFlows.MakeAPayment.FlowDefinitions;
using EI.RP.WebApp.Infrastructure.StringResources;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;

namespace EI.RP.WebApp.Flows.AppFlows.MakeAPayment.Steps
{
    public class ChangePaymentAmount : PaymentsScreen
    {
	    protected override string Title => "Change Payment Amount";

        public override ScreenName ScreenStep => PaymentStep.ChangePaymentAmount;

        public ChangePaymentAmount()
        {
        }

        public static class StepEvent
        {
            public static readonly ScreenEvent SaveNewPaymentAmount = new ScreenEvent(nameof(ChangePaymentAmount), nameof(SaveNewPaymentAmount));
            public static readonly ScreenEvent CancelChangePaymentAmount = new ScreenEvent(nameof(ChangePaymentAmount), nameof(CancelChangePaymentAmount));
        }

        protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
        {
            return screenConfiguration
                .OnEventNavigatesTo(StepEvent.SaveNewPaymentAmount, PaymentStep.PaymentGatewayInteractionAndMainOptions)
                .OnEventNavigatesTo(StepEvent.CancelChangePaymentAmount, PaymentStep.PaymentGatewayInteractionAndMainOptions);
        }

        protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
        {
            var rootData = contextData.GetStepData<PaymentFlowInitializer.RootScreenModel>(ScreenName.PreStart);
            var stepData = new ScreenModel
            {
	            AccountNumber = rootData.AccountNumber
            };

            SetTitle(Title, stepData);

            await BuildStepData(stepData, contextData);

            return stepData;
        }

        private async Task BuildStepData(ScreenModel screenModel, IUiFlowContextData contextData)
        {
            var paymentStep =
                    contextData.GetStepData<PaymentGatewayInteractionAndMainOptions.ScreenModel>(PaymentStep
                        .PaymentGatewayInteractionAndMainOptions);

            if (paymentStep.CurrentBalanceAmount != null)
            {
                var currentBalanceAmount = paymentStep.CurrentBalanceAmount;

                screenModel.CurrentBalanceAmount = currentBalanceAmount;
                screenModel.Amount = currentBalanceAmount?.ToDecimal(0.02M);
            }
        }

        protected override async Task OnHandlingStepEvent(ScreenEvent triggeredEvent, IUiFlowContextData contextData)
        {
            if (triggeredEvent == StepEvent.SaveNewPaymentAmount)
            {
                var paymentStep =
                    contextData.GetStepData<PaymentGatewayInteractionAndMainOptions.ScreenModel>(PaymentStep
                        .PaymentGatewayInteractionAndMainOptions);

                var currentStepData = contextData.GetCurrentStepData<ScreenModel>();

                paymentStep.CurrentBalanceAmount = currentStepData.CurrentBalanceAmount = new EuroMoney(currentStepData.Amount.GetValueOrDefault(0M));
                paymentStep.CurrentBalanceAmountLabel = currentStepData.CurrentBalanceAmountLabel = "Amount To Pay";
                paymentStep.ShowLatestBillDate = false;

            }
        }
        
        protected override async Task<UiFlowScreenModel> OnRefreshStepDataAsync(IUiFlowContextData contextData,
	        UiFlowScreenModel originalScreenModel,
	        IDictionary<string, object> stepViewCustomizations = null)
        {
            var refreshedStepData = originalScreenModel.CloneDeep();
            var updatedStepData = (ScreenModel)refreshedStepData;

            SetTitle(Title, updatedStepData);

            if (updatedStepData.Errors.Any() && updatedStepData.Errors.Any(x => x.MemberName == nameof(updatedStepData.Amount)))
            {
                var inputAmount = updatedStepData.CurrentBalanceAmount?.ToDecimal(0.02M);
            }
            else
            {
                await BuildStepData(updatedStepData, contextData);
            }

            return updatedStepData;
        }

        public class ScreenModel : UiFlowScreenModel
        {
            public override IEnumerable<ScreenEvent> DontValidateEvents =>
                base.DontValidateEvents.Union(StepEvent.CancelChangePaymentAmount.ToOneItemArray());

            public override bool IsValidFor(ScreenName screenStep)
            {
                return screenStep == PaymentStep.ChangePaymentAmount;
            }

            public string AccountNumber { get; set; }

            public string CurrentBalanceAmountLabel { get; set; }
            public EuroMoney CurrentBalanceAmount { get; set; }

            [Required(ErrorMessage = "You must enter amount for payment  ")]
            [RegularExpression(ReusableRegexPattern.ValidCurrency, ErrorMessage = "Please enter valid amount for payment")]
            [Range(0.02, 9999.99, ErrorMessage = "Payment amount must be within 0.02 to 9999.99")]
            public decimal? Amount { get; set; }
        }
    }
}
