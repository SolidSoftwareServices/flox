using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Commands.Billing.ChangePaperBillChoice;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.FlowDefinitions;

namespace EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.Steps
{
	public class SetupDirectDebitWithPaperBillOptions : AccountsPaymentConfigurationScreen
    {
        private readonly IDomainCommandDispatcher _domainCommandDispatcher;
        private readonly IDomainQueryResolver _domainQueryResolver;

        public static class StepEvent
		{
			public static readonly ScreenEvent SetUpDirectDebit = new ScreenEvent(nameof(SetupDirectDebitWithPaperBillOptions), nameof(SetUpDirectDebit));
            public static readonly ScreenEvent SwitchOffPaperBill = new ScreenEvent(nameof(SetupDirectDebitWithPaperBillOptions), nameof(SwitchOffPaperBill));
            public static readonly ScreenEvent SwitchOnPaperBill = new ScreenEvent(nameof(SetupDirectDebitWithPaperBillOptions), nameof(SwitchOnPaperBill));
        }
        public override ScreenName ScreenStep => AccountsPaymentConfigurationStep.SetupDirectDebitWithPaymentOptions;

		public SetupDirectDebitWithPaperBillOptions(IDomainQueryResolver domainQueryResolver, IDomainCommandDispatcher domainCommandDispatcher)
        {
            _domainQueryResolver = domainQueryResolver;
            _domainCommandDispatcher = domainCommandDispatcher;
        }
        protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			return screenConfiguration
				.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
				.OnEventReentriesCurrent(StepEvent.SwitchOffPaperBill)
				.OnEventReentriesCurrent(StepEvent.SwitchOnPaperBill)
				.OnEventNavigatesTo(StepEvent.SetUpDirectDebit, AccountsPaymentConfigurationStep.InputDirectDebitDetailsStep);

		}
        protected override async Task OnHandlingStepEvent(ScreenEvent triggeredEvent,
            IUiFlowContextData contextData)
        {
            var stepData = contextData.GetStepData<BillingAndPaymentOptionsData>(AccountsPaymentConfigurationStep.SetupDirectDebitWithPaymentOptions);
            var accountTask = _domainQueryResolver.GetAccountInfoByAccountNumber(stepData.AccountNumber);
            var userAccountInfo = await accountTask;

            if (triggeredEvent == StepEvent.SwitchOffPaperBill && userAccountInfo.PaperBillChoice!=PaperBillChoice.Off)
            {
				await _domainCommandDispatcher.ExecuteAsync(new ChangePaperBillChoiceCommand(stepData.AccountNumber,PaperBillChoice.Off));
				stepData.SwitchOnUpdateSuccess = true;
			}
            else if (triggeredEvent == StepEvent.SwitchOnPaperBill && userAccountInfo.PaperBillChoice!=PaperBillChoice.On)
            {
				await _domainCommandDispatcher.ExecuteAsync(new ChangePaperBillChoiceCommand(stepData.AccountNumber, PaperBillChoice.On));
				stepData.SwitchOffUpdateSuccess = true;
            }

            

		}
        protected override async Task<UiFlowScreenModel> OnRefreshStepDataAsync(IUiFlowContextData contextData,
	        UiFlowScreenModel originalScreenModel,
	        IDictionary<string, object> stepViewCustomizations = null)
        {
            var originalData = contextData.GetStepData<SetupDirectDebitWithPaperBillOptions.BillingAndPaymentOptionsData>();
            var accountTask = _domainQueryResolver.GetAccountInfoByAccountNumber(originalData.AccountNumber);

            var userAccountInfo = await accountTask;

            originalData.HasPaperBill = userAccountInfo.PaperBillChoice==PaperBillChoice.On;

            return originalData;
        }


		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			var account = contextData.GetStepData<AccountsPaymentConfigurationFlowInitializer.RootScreenModel>(ScreenName.PreStart).CurrentAccount();
            return new BillingAndPaymentOptionsData
            {
				PaymentMethod = account.BankAccount.PaymentMethod,
				AccountNumber = account.Account.AccountNumber,
                HasPaperBill = account.Account.PaperBillChoice == PaperBillChoice.On,
            };
		}


		public class BillingAndPaymentOptionsData: UiFlowScreenModel
		{
			
            public override bool IsValidFor(ScreenName screenStep)
			{
				return  screenStep== AccountsPaymentConfigurationStep.SetupDirectDebitWithPaymentOptions;
			}

            public PaymentMethodType PaymentMethod { get; set; }
            public bool HasPaperBill { get; set; }
            public string AccountNumber { get; set; }
            public bool SwitchOnUpdateSuccess { get; set; }
            public bool SwitchOffUpdateSuccess { get; set; }
        }
	}
}
