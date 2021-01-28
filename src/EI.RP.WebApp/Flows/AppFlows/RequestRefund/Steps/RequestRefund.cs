using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using EI.RP.DomainServices.Commands.Billing.RequestRefund;
using EI.RP.DomainServices.Queries.Billing.Info;
using EI.RP.WebApp.Flows.AppFlows.RequestRefund.FlowDefinitions;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Infrastructure.StringResources;

namespace EI.RP.WebApp.Flows.AppFlows.RequestRefund.Steps
{
    public class RequestRefund : RequestRefundScreen
    {
	    public override ScreenName ScreenStep => RequestRefundStep.RequestRefund;

        private readonly IDomainQueryResolver _domainQueryResolver;
        private readonly IDomainCommandDispatcher _domainCommandDispatcher;


        public RequestRefund(IDomainQueryResolver domainQueryResolver, IDomainCommandDispatcher domainCommandDispatcher)
        {
            _domainQueryResolver = domainQueryResolver;
            _domainCommandDispatcher = domainCommandDispatcher;
        }

        public static class StepEvent
        {
            public static readonly ScreenEvent SubmitRefund = new ScreenEvent(nameof(RequestRefund),nameof(SubmitRefund));
        }

        protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
            IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
        {
            return screenConfiguration
                .OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
            .OnEventNavigatesTo(StepEvent.SubmitRefund, RequestRefundStep.ShowStatusMessage);
        }

        protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
        {
            var rootData = contextData.GetStepData<RequestRefundInitializer.RootScreenModel>(ScreenName.PreStart);
            var accountNumber = rootData.AccountNumber;

            var accountTask = _domainQueryResolver.GetAccountInfoByAccountNumber(accountNumber);
            var userAccountInfo = await accountTask;

            var billingInfoTask = _domainQueryResolver.GetAccountBillingInfoByAccountNumber(accountNumber);
            var billingInfo = await billingInfoTask;

            var stepData = new ScreenModel
            {
	            DisplayAccountName = string.Join(" ", userAccountInfo.ClientAccountType, userAccountInfo.Description),
                AccountNumber = userAccountInfo.AccountNumber,
                AmountCredit = billingInfo.CurrentBalanceAmount,
                Partner = userAccountInfo.Partner,
                PaymentMethodType = userAccountInfo.PaymentMethod
            };

            SetTitle(Title, stepData);

            return stepData;
        }

        protected override async Task<UiFlowScreenModel> OnRefreshStepDataAsync(IUiFlowContextData contextData,
            UiFlowScreenModel originalScreenModel,
            IDictionary<string, object> stepViewCustomizations = null)
        {
            var refreshedStepData = originalScreenModel.CloneDeep();
            var updatedStepData = (ScreenModel)refreshedStepData;

            SetTitle(Title, updatedStepData);

            var errorMessage = contextData.LastError?.ExceptionMessage;
            updatedStepData.ErrorMessage = errorMessage;

            return updatedStepData;
        }

        protected override async Task OnHandlingStepEvent(ScreenEvent triggeredEvent, IUiFlowContextData contextData)
        {
            if (triggeredEvent == StepEvent.SubmitRefund)
            {
	            var input = contextData.GetCurrentStepData<ScreenModel>();

                await _domainCommandDispatcher.ExecuteAsync(new RequestRefundCommand(input.AccountNumber,
                    input.Partner,input.PaymentMethodType,input.Comments));
            }
        }

        public class ScreenModel : UiFlowScreenModel
        {
            public string DisplayAccountName { get; set; }

            public string Partner { get; set; }

            public PaymentMethodType PaymentMethodType { get; set; }

            [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your message here.")]
            [RegularExpression(ReusableRegexPattern.ValidAccountQuery, ErrorMessage = "Please enter your message here.")]
            [StringLength(1320, ErrorMessage = "Please enter no more than 1320 characters")]
            public string Comments { get; set; }

            public EuroMoney AmountCredit { get; set; }
            public string ErrorMessage { get; set; }
            public string AccountNumber { get; set; }
        }
    }
}
