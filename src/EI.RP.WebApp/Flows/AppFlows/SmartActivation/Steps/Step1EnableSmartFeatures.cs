using System.Collections.Generic;
using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.UiFlows.Mvc.FlowTagHelpers;
using EI.RP.WebApp.Flows.AppFlows.SmartActivation.FlowDefinitions;
using EI.RP.WebApp.Infrastructure.Validation.CustomValidators;

namespace EI.RP.WebApp.Flows.AppFlows.SmartActivation.Steps
{
    public class Step1EnableSmartFeatures : SmartActivationScreen
    {
	    protected string StepTitle => string.Join(" | ", "1. Smart Features", Title);

        public static class StepEvent
        {
			public static readonly ScreenEvent EnableSmartServices = new ScreenEvent(nameof(Step1EnableSmartFeatures), nameof(EnableSmartServices));
		}

        private readonly IDomainQueryResolver _domainQueryResolver;
        private readonly IDomainCommandDispatcher _domainCommandDispatcher;
        public Step1EnableSmartFeatures(IDomainQueryResolver domainQueryResolver, IDomainCommandDispatcher domainCommandDispatcher)
        {
            _domainQueryResolver = domainQueryResolver;
            _domainCommandDispatcher = domainCommandDispatcher;
        }

        public override ScreenName ScreenStep => SmartActivationStep.Step1EnableSmartFeatures;

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
			screenConfiguration.OnEventNavigatesTo(StepEvent.EnableSmartServices, SmartActivationStep.Step2SelectPlan);
			return screenConfiguration;
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
			var rootScreenModel = contextData.GetStepData<SmartActivationFlowInitializer.StepsSharedData>();

			model.SourceFlowType = rootScreenModel.SourceFlowType;
			model.FlowLocation = rootScreenModel.SourceFlowType == ResidentialPortalFlowType.Accounts
				? FlowActionTagHelper.StartFlowLocation.NotContained
				: FlowActionTagHelper.StartFlowLocation.ContainedInMe;

			return model;
		}

		public sealed class ScreenModel : UiFlowScreenModel
        {
			public ResidentialPortalFlowType SourceFlowType { get; set; }

			public FlowActionTagHelper.StartFlowLocation FlowLocation { get; set; }

			[BooleanRequired(ErrorMessage = "You need to check the box above before you can continue")]
			public bool InformationCollectionAuthorized { get; set; }

			public ScreenModel(): base(false)
            {
            }

            public override bool IsValidFor(ScreenName screenStep)
            {
                return screenStep == SmartActivationStep.Step1EnableSmartFeatures;
            }
        }
    }
}