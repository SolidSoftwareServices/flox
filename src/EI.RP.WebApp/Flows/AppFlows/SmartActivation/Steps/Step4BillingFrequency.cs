using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using Ei.Rp.Mvc.Core.ViewModels.Validations;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.SmartActivation.FlowDefinitions;

namespace EI.RP.WebApp.Flows.AppFlows.SmartActivation.Steps
{
    public class Step4BillingFrequency : SmartActivationScreen
    {
	    protected string StepTitle => string.Join(" | ", "4. Billing Options", Title);

        public static class StepEvent
        {
	        public static readonly ScreenEvent Continue = new ScreenEvent(nameof(Step4BillingFrequency), nameof(Continue));
        }

        private readonly IDomainQueryResolver _domainQueryResolver;

        public Step4BillingFrequency(IDomainQueryResolver domainQueryResolver)
        {
            _domainQueryResolver = domainQueryResolver;
        }

        public override ScreenName ScreenStep => SmartActivationStep.Step4BillingFrequency;

        protected override Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
        {
	        var screenModel = new ScreenModel();

	        SetTitle(StepTitle, screenModel);

	        return BuildStepData(contextData, screenModel);
        }

		protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
			IScreenFlowConfigurator screenConfiguration,
			IUiFlowContextData contextData)
		{
			return screenConfiguration
				.OnEventNavigatesTo(StepEvent.Continue, SmartActivationStep.Step5Summary);
		}

		protected override async Task OnHandlingStepEvent(ScreenEvent triggeredEvent, IUiFlowContextData contextData)
		{
			await base.OnHandlingStepEvent(triggeredEvent, contextData);
		}

		protected override Task<UiFlowScreenModel> OnRefreshStepDataAsync(
			IUiFlowContextData contextData, 
			UiFlowScreenModel originalScreenModel,
			IDictionary<string, object> stepViewCustomizations = null)
		{
			var screenModel = (ScreenModel) originalScreenModel;

			SetTitle(StepTitle, screenModel);

			return BuildStepData(contextData, screenModel);
		}

		async Task<UiFlowScreenModel> BuildStepData(IUiFlowContextData contextData, ScreenModel model)
		{
			var rootStepData = contextData.GetStepData<SmartActivationFlowInitializer.StepsSharedData>();
			var accountInfo = await _domainQueryResolver.GetAccountInfoByAccountNumber(rootStepData.AccountNumber);

			model.SourceFlowType = rootStepData.SourceFlowType;
			model.IsAlternativePayer = accountInfo.PaymentMethod == PaymentMethodType.DirectDebitNotAvailable;

			return model;
		}

		public sealed class ScreenModel : UiFlowScreenModel
        {
            public ScreenModel(): base(false)
            {
            }

            public ResidentialPortalFlowType SourceFlowType { get; set; }

            [Required]
			public BillingFrequencyType? BillingFrequencyType { get; set; }

			[RequiredIf(
				nameof(BillingFrequencyType), 
				IfValue = Steps.BillingFrequencyType.EveryMonth, 
				ErrorMessage = "Please select a day of the month")]
			[Range(typeof(int), "1",  "28", ErrorMessage = "Please select a day of the month")]
			public int? BillingDayOfMonth { get; set; }

			public bool IsAlternativePayer { get; set; }

			public override bool IsValidFor(ScreenName screenStep)
            {
                return screenStep == SmartActivationStep.Step4BillingFrequency;
            }
        }
    }

    public enum BillingFrequencyType
    {
		EveryMonth,
		EveryTwoMonths
    }
}