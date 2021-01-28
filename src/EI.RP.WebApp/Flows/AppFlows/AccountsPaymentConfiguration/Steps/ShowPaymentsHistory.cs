using System.Collections.Generic;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Queries.Billing.Info;
using EI.RP.DomainServices.Queries.Billing.Activity;
using EI.RP.DomainServices.Queries.Billing.EqualiserPaymentSetupInfo;
using EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.FlowDefinitions;
using System;
using Ei.Rp.DomainModels.Contracts;
using System.Linq;
using Ei.Rp.DomainModels.Contracts.Accounts;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;

namespace EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.Steps
{
    public class ShowPaymentsHistory : AccountsPaymentConfigurationScreen
    {
	    private const string Title = "Bills & Payments";
        
	    private readonly IDomainQueryResolver _domainQueryResolver;

        public ShowPaymentsHistory(IDomainQueryResolver domainQueryResolver)
	    {
		    _domainQueryResolver = domainQueryResolver;
	    }

        public static class StepEvent
        {
	        public static readonly ScreenEvent EqualizerMonthlyPayments = new ScreenEvent(nameof(ShowPaymentsHistory), nameof(EqualizerMonthlyPayments));
        }

        protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
            IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
        {
            return screenConfiguration
                .OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
                .OnEventNavigatesTo(StepEvent.EqualizerMonthlyPayments, AccountsPaymentConfigurationStep.EqualizerMonthlyPayments);
        }

        public override ScreenName ScreenStep => AccountsPaymentConfigurationStep.ShowPaymentsHistory;

        protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
        {
            var rootData = contextData.GetStepData<AccountsPaymentConfigurationFlowInitializer.RootScreenModel>(ScreenName.PreStart);
            var inputAccount = rootData.CurrentAccount();

            var stepData = new ScreenModel
            {
                AccountNumber = inputAccount.Account.AccountNumber,
                PaymentMethod = inputAccount.BankAccount.PaymentMethod
            };

            await BuildStepData(stepData, contextData);
            return stepData;
        }

        private async Task BuildStepData(ScreenModel screenModel, IUiFlowContextData contextData)
        {
	        SetTitle(Title, screenModel);

            var accountInfoTask = _domainQueryResolver.GetAccountInfoByAccountNumber(screenModel.AccountNumber);
            var equalizerSetUpInfo = _domainQueryResolver.GetEqualiserSetUpInfo(screenModel.AccountNumber);
            var getBillingInfo = _domainQueryResolver.GetAccountBillingInfoByAccountNumber(screenModel.AccountNumber);
            var getInvoicesByAccountNumber = _domainQueryResolver.GetInvoicesByAccountNumber(screenModel.AccountNumber);

            screenModel.AccountClosed = !(await accountInfoTask).IsOpen;
            screenModel.HasEqualMonthlyPayments = (await equalizerSetUpInfo).CanSetUpEqualizer;

            screenModel.AmountDue = (await getBillingInfo)?.CurrentBalanceAmount;
            var accountBillingActivities = (await getInvoicesByAccountNumber)
	            .OrderByDescending(x => x.OriginalDate);
            screenModel.DueDate = accountBillingActivities
                .FirstOrDefault(x => !x.IsPayment()  && x.InvoiceStatus!=InvoiceStatus.Paid)?.DueDate;

			if (screenModel.PaymentMethod.IsOneOf(
				PaymentMethodType.DirectDebit,
				PaymentMethodType.AlternativePayer,
				PaymentMethodType.DirectDebit,
				PaymentMethodType.DirectDebitNotAvailable,
				PaymentMethodType.Manual))
			{
	            screenModel.OverduePayments = (await getInvoicesByAccountNumber)
		            .Where(x => x.InvoiceStatus != InvoiceStatus.Paid && x.DueDate < DateTime.Now)
		            .Select(x => new OverduePayment
		            {
			            Amount = x.RemainingAmount,
			            DueDate = x.DueDate
		            }).ToArray();
			}

            screenModel.IsDue = screenModel.AmountDue > 0 &&
                                screenModel.DueDate.GetValueOrDefault() >= DateTime.Now &&
                                screenModel.PaymentMethod != PaymentMethodType.Equalizer;
        }

        protected override async Task<UiFlowScreenModel> OnRefreshStepDataAsync(IUiFlowContextData contextData,
	        UiFlowScreenModel originalScreenModel,
	        IDictionary<string, object> stepViewCustomizations = null)
        {
            var refreshedStepData = originalScreenModel.CloneDeep();
            var data = (ScreenModel)refreshedStepData;

            if (stepViewCustomizations != null)
            {
                data.SetFlowCustomizableValue(stepViewCustomizations, x=>x.PageIndex);
            }

            await BuildStepData(data, contextData);
            return data;
        }

        public class ScreenModel : UiFlowScreenModel
		{
			public override bool IsValidFor(ScreenName screenStep)
			{
				return screenStep == AccountsPaymentConfigurationStep.ShowPaymentsHistory;
			}
            public string AccountNumber { get; set; }

            public bool AccountClosed { get; set; }

            public EuroMoney AmountDue { get; set; }

            public bool IsDue { get; set; }
            public DateTime? DueDate { get; set; }

            public IEnumerable<OverduePayment> OverduePayments { get; set; }=new OverduePayment[0];

            public PaymentMethodType PaymentMethod { get; set; }

			public bool HasEqualMonthlyPayments { get; set; }
			public int PageIndex { get; set; }
		}

        public class OverduePayment
        {
            public EuroMoney Amount { get; set; }
            public DateTime DueDate { get; set; }
        }
    }
}