using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using EI.RP.DomainServices.Queries.Billing.EqualiserPaymentSetupInfo;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.FlowDefinitions;

namespace EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.Steps
{
	public class SetupEqualizerMonthlyPayments : AccountsPaymentConfigurationScreen
    {
	    private const string Title = "Equal Monthly Payments";
        
	    private readonly IDomainQueryResolver _domainQueryResolver;

        public static class StepEvent
        {
            public static readonly ScreenEvent SetupDirectDebit =
                new ScreenEvent(nameof(SetupEqualizerMonthlyPayments),nameof(SetupDirectDebit));
            public static readonly ScreenEvent NewPaymentDateSelected =
                new ScreenEvent(nameof(SetupEqualizerMonthlyPayments), nameof(NewPaymentDateSelected));
            public static readonly ScreenEvent EqualizerMonthlyPaymentStep =
                new ScreenEvent(nameof(SetupEqualizerMonthlyPayments), nameof(EqualizerMonthlyPaymentStep));
        }

        public SetupEqualizerMonthlyPayments(IDomainQueryResolver domainQueryResolver)
        {
            _domainQueryResolver = domainQueryResolver;
        }

        protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
        {
            return screenConfiguration
                .OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
                .OnEventReentriesCurrent(StepEvent.NewPaymentDateSelected)
                .OnEventNavigatesTo(StepEvent.SetupDirectDebit, AccountsPaymentConfigurationStep.InputDirectDebitDetailsStep)
                .OnEventNavigatesTo(StepEvent.EqualizerMonthlyPaymentStep, AccountsPaymentConfigurationStep.EqualizerMonthlyPayments );
        }

        public override ScreenName ScreenStep => AccountsPaymentConfigurationStep.SetUpEqualizerMonthlyPayments;

		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			var rootData = contextData.GetStepData<AccountsPaymentConfigurationFlowInitializer.RootScreenModel>(ScreenName.PreStart);
            var equalizerMonthlyPaymentsStepData = contextData.GetStepData<EqualizerMonthlyPayments.ScreenModel>();
            var firstDueDate = GetFirstDueDate();

            var screenModel = new ScreenModel
            {
	            AccountNumber = rootData.CurrentAccount().Account.AccountNumber,
                EqualizerMonthlyPaymentAmount = equalizerMonthlyPaymentsStepData.EqualizerMonthlyPaymentDetails
                    .EqualizerMonthlyPayment,
                FirstPaymentDate = firstDueDate
            };

            SetTitle(Title, screenModel);

            return screenModel;
		}

        private DateTime GetFirstDueDate()
        {
            return DateTime.Today.AddDays(10).FirstDayOfNextMonth(28);
        }

        protected override async Task<UiFlowScreenModel> OnRefreshStepDataAsync(IUiFlowContextData contextData,
            UiFlowScreenModel originalScreenModel,
            IDictionary<string, object> stepViewCustomizations = null)
        {
            var original = (ScreenModel)originalScreenModel;

            var setUpInfoTask = _domainQueryResolver.GetEqualiserSetUpInfo(
                original.AccountNumber,
                original.FirstPaymentDate);

            var refreshedStepData = originalScreenModel.CloneDeep();
            var updatedStepData = (ScreenModel)refreshedStepData;
            var equaliserSetUpInfo = await setUpInfoTask;

            updatedStepData.EqualizerMonthlyPaymentAmount = equaliserSetUpInfo.Amount;

            SetTitle(Title, updatedStepData);

            return updatedStepData;
        }

        public class ScreenModel : UiFlowScreenModel
		{
            public override bool IsValidFor(ScreenName screenStep)
			{
				return screenStep == AccountsPaymentConfigurationStep.SetUpEqualizerMonthlyPayments;
			}

			public string AccountNumber { get; set; }
            public EuroMoney EqualizerMonthlyPaymentAmount { get; set; }

            public DateTime? FirstPaymentDate { get; set; }
        }
    }
}
