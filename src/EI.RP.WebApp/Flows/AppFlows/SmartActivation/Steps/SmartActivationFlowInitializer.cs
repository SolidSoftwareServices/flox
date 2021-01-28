using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Http.Session;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.SmartActivation;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Initialization;
using EI.RP.UiFlows.Core.Flows.Initialization.Models;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.SmartActivation.FlowDefinitions;

namespace EI.RP.WebApp.Flows.AppFlows.SmartActivation.Steps
{
	public class SmartActivationFlowInitializer : UiFlowInitializationStep<ResidentialPortalFlowType, SmartActivationFlowInitializer.StepsSharedData>
    {
        private readonly IUserSessionProvider _userSessionProvider;
        private readonly IDomainQueryResolver _domainQueryResolver;
        private readonly IDomainCommandDispatcher _domainCommandDispatcher;
        public SmartActivationFlowInitializer(IUserSessionProvider userSessionProvider, IDomainQueryResolver domainQueryResolver, IDomainCommandDispatcher domainCommandDispatcher)
        {
            _userSessionProvider = userSessionProvider;
            _domainQueryResolver = domainQueryResolver;
            _domainCommandDispatcher = domainCommandDispatcher;
        }

        public override ResidentialPortalFlowType InitializerOfFlowType => ResidentialPortalFlowType.SmartActivation;
        public override bool Authorize()
        {
			return !_userSessionProvider.IsAnonymous();
		}

        public override IScreenFlowConfigurator OnDefiningAdditionalInitialStateTransitions(IScreenFlowConfigurator preStartCfg, UiFlowContextData contextData)
        {
	        var rootScreenData = contextData.GetStepData<StepsSharedData>();

			return preStartCfg
	            .OnEventNavigatesTo(ScreenEvent.ErrorOccurred, SmartActivationStep.ShowFlowGenericError)
	            .OnEventNavigatesTo(ScreenEvent.Start,  SmartActivationStep.Step1EnableSmartFeatures, () => rootScreenData.IsSmartAndEligible, "Account Can Opt Into Smart")
	            .OnEventNavigatesTo(ScreenEvent.Start, SmartActivationStep.ShowFlowGenericError, () => !rootScreenData.IsSmartAndEligible, "Account Can Not Opt Into Smart");
        }

        protected override async Task<StepsSharedData> OnBuildStartData(UiFlowContextData contextData, StepsSharedData data)
        {
	        var accountInfo =  await _domainQueryResolver.GetAccountInfoByAccountNumber(data.AccountNumber);

	        data.IsSmartAndEligible = accountInfo.SmartActivationStatus == SmartActivationStatus.SmartAndEligible;
            return data;
        }

      

        public class StepsSharedData : InitialFlowScreenModel, ISmartActivationInput
        {
	        public ResidentialPortalFlowType SourceFlowType { get; set; }
			public bool IsSmartAndEligible { get; set; }
	        public string AccountNumber { get; set; }

	        public string SelectedPlanName { get; set; }

	        public DayOfWeek? SelectedPlanFreeDay { get; set; }
        }
    }
}