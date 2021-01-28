using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.Contracts;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.DomainServices.Commands.SmartActivation.ActivateSmartMeter;
using EI.RP.DomainServices.Commands.Banking.DirectDebit.SetUpDirectDebit;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using EI.RP.WebApp.Flows.AppFlows.SmartActivation.FlowDefinitions;
using EI.RP.WebApp.Infrastructure.Validation.CustomValidators;
using EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.FlowDefinitions;
using Ei.Rp.DomainModels.SmartActivation;
using EI.RP.CoreServices.System;
using EI.RP.DomainServices.Queries.SmartActivation.SmartActivationPlan;

namespace EI.RP.WebApp.Flows.AppFlows.SmartActivation.Steps
{
    public class Step5Summary : SmartActivationScreen
    {
	    protected string StepTitle => string.Join(" | ", "5. Summary", Title);

        public static class StepEvent
        {
			public static readonly ScreenEvent RequestCompleteSmartActivation = new ScreenEvent(nameof(Step5Summary), nameof(RequestCompleteSmartActivation));
		}

        private readonly IDomainQueryResolver _domainQueryResolver;
        private readonly IDomainCommandDispatcher _domainCommandDispatcher;

        public Step5Summary(IDomainQueryResolver domainQueryResolver, IDomainCommandDispatcher domainCommandDispatcher)
        {
            _domainQueryResolver = domainQueryResolver;
            _domainCommandDispatcher = domainCommandDispatcher;
        }

        public override ScreenName ScreenStep => SmartActivationStep.Step5Summary;

        protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
        {
	        var screenModel = new ScreenModel();

	        SetTitle(StepTitle, screenModel);

	        return await BuildStepData(contextData, screenModel);
        }

		private async Task<ScreenModel> BuildStepData(IUiFlowContextData contextData, ScreenModel screenModel)
		{
			var stepsSharedData = contextData.GetStepData<SmartActivationFlowInitializer.StepsSharedData>();
			var step2Data = contextData.GetStepData<Step2SelectPlan.ScreenModel>();
			var step4Data = contextData.GetStepData<Step4BillingFrequency.ScreenModel>();
			var setPlanInfo = SetPlanInfoTask();
			

			screenModel.SourceFlowType = stepsSharedData.SourceFlowType;
			
			

			screenModel.AccountNumber = stepsSharedData.AccountNumber;
			
			screenModel.SelectedFreeDay = step2Data.SelectedFreeDay;
			
			screenModel.MonthlyBilling = step4Data.BillingFrequencyType.GetValueOrDefault() == BillingFrequencyType.EveryMonth;
			screenModel.MonthlyBillingSelectedDay = step4Data.BillingDayOfMonth.GetValueOrDefault();

			ConfigurePaymentDetails();
			await setPlanInfo;
			return screenModel;

			void ConfigurePaymentDetails()
			{
				var paymentResult = contextData.GetStepData<Step3PaymentDetails.ScreenModel>().CalledFlowResult;
				screenModel.PaymentInfo = paymentResult.ConfigurationSelectionResults;
			
				screenModel.AlternativePayer =  screenModel.PaymentInfo.Any(c =>
					  c.SelectedPaymentSetUpType == PaymentSetUpType.AlternativePayer );

				if (!screenModel.AlternativePayer)
				{
					var isDirectDebit = screenModel.PaymentInfo.Any(c =>
										 c.SelectedPaymentSetUpType == PaymentSetUpType.SetUpNewDirectDebit ||
										 c.SelectedPaymentSetUpType == PaymentSetUpType.UseExistingDirectDebit);

					screenModel.PaymentMethod = isDirectDebit ? PaymentMethodType.DirectDebit : PaymentMethodType.Manual;
				} else
				{
					screenModel.PaymentMethod = PaymentMethodType.DirectDebitNotAvailable;
				}
			}

			async Task SetPlanInfoTask()
			{
				var plan = await _domainQueryResolver.GetSmartActivationPlan(stepsSharedData.AccountNumber,
					stepsSharedData.SelectedPlanName);
				screenModel.SelectedPlanName = stepsSharedData.SelectedPlanName;
				screenModel.GeneralTermsAndConditionsUrl = plan.GeneralTermsAndConditionsUrl;
				screenModel.PricePlanTermsAndConditionsUrl = plan.PricePlanTermsAndConditionsUrl;
			}
		}

		protected override async Task<UiFlowScreenModel> OnRefreshStepDataAsync(
			IUiFlowContextData contextData,
			UiFlowScreenModel originalScreenModel,
			IDictionary<string, object> stepViewCustomizations = null)
		{
			var screenModel = (ScreenModel)originalScreenModel;

			SetTitle(StepTitle, screenModel);

			return await BuildStepData(contextData, screenModel);
		}

		protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(IScreenFlowConfigurator screenConfiguration,
			IUiFlowContextData contextData)
		{
			return screenConfiguration
				.OnEventNavigatesTo(ScreenEvent.ErrorOccurred, SmartActivationStep.ShowFlowGenericError)
				.OnEventNavigatesTo(StepEvent.RequestCompleteSmartActivation, SmartActivationStep.Confirmation);
		}

		protected override async Task OnHandlingStepEvent(ScreenEvent triggeredEvent, IUiFlowContextData contextData)
		{
			if (triggeredEvent == StepEvent.RequestCompleteSmartActivation)
			{
				var stepData = contextData.GetStepData<ScreenModel>();
				var accountInfo = await _domainQueryResolver.GetAccountInfoByAccountNumber(stepData.AccountNumber);
				var getPlan = _domainQueryResolver.GetSmartActivationPlan(accountInfo.AccountNumber, stepData.SelectedPlanName,true);
				var mprn = (ElectricityPointReferenceNumber) accountInfo.PointReferenceNumber;
				var setUpDirectDebitDomainCommands = new List<SetUpDirectDebitDomainCommand>();

				foreach (var item in stepData.PaymentInfo)
				{
					if (item.SelectedPaymentSetUpType.IsOneOf(PaymentSetUpType.SetUpNewDirectDebit, PaymentSetUpType.UseExistingDirectDebit))
					{
						var clientAccountType = item.Account == null ? item.TargetAccountType : item.Account.ClientAccountType;
						var command = new SetUpDirectDebitDomainCommand(item.Account?.AccountNumber,
							item.CommandToExecute.NameOnBankAccount,
							item.BankAccount?.IBAN,
							item.CommandToExecute.IBAN,
							item.Account?.Partner,
							clientAccountType,
							PaymentMethodType.DirectDebit,
							null,
							null,
							null);

						setUpDirectDebitDomainCommands.Add(command);
					}
				}

				
				var cmd = new ActivateSmartMeterCommand(mprn,
														stepData.AccountNumber,
														await getPlan,
														stepData.SelectedFreeDay,
														stepData.MonthlyBilling,
														stepData.MonthlyBillingSelectedDay,
														setUpDirectDebitDomainCommands);

				await _domainCommandDispatcher.ExecuteAsync(cmd);
			}
			else
			{
				await base.OnHandlingStepEvent(triggeredEvent, contextData);
			}
		}

		public sealed class ScreenModel : UiFlowScreenModel
        {
            public ScreenModel(): base(false)
            {
            }

            public ResidentialPortalFlowType SourceFlowType { get; set; }

			public string AccountNumber { get; set; }
			public bool MonthlyBilling { get; set; }
			public int MonthlyBillingSelectedDay { get; set; }
			public string SelectedPlanName { get; set; }
			public DayOfWeek? SelectedFreeDay { get; set; }
			public PaymentMethodType PaymentMethod { get; set; }
			public bool AlternativePayer { get; set; }
			public IEnumerable<AccountsPaymentConfigurationResult.AccountConfigurationInfo> PaymentInfo { get; set; }

			public string GeneralTermsAndConditionsUrl { get; set; }
			public string PricePlanTermsAndConditionsUrl { get; set; }

			[BooleanRequired(ErrorMessage = "You need to check the box above before you can continue")]
			public bool TermsAndConditionsAccepted { get; set; }

			public override bool IsValidFor(ScreenName screenStep)
            {
                return screenStep == SmartActivationStep.Step5Summary;
            }
        }
    }
}