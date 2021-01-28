using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Http.Session;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Initialization;
using EI.RP.UiFlows.Core.Flows.Initialization.Models;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.AccountAndMeterDetails.FlowDefinitions;

namespace EI.RP.WebApp.Flows.AppFlows.AccountAndMeterDetails.Steps
{
    public class FlowInitializer : UiFlowInitializationStep<ResidentialPortalFlowType, FlowInitializer.RootScreenModel>
    {
	    public static class StepEvent
        {

        }

        private readonly IUserSessionProvider _userSessionProvider;
        private readonly IDomainQueryResolver _domainQueryResolver;

	    public override ResidentialPortalFlowType InitializerOfFlowType =>
		    ResidentialPortalFlowType.AccountAndMeterDetails;

		public FlowInitializer(IUserSessionProvider userSessionProvider, IDomainQueryResolver domainQueryResolver) 
        {
            _userSessionProvider = userSessionProvider;
            _domainQueryResolver = domainQueryResolver;
        }

        public override bool Authorize()
        {
            return !_userSessionProvider.IsAnonymous();
        }

        public override IScreenFlowConfigurator OnDefiningAdditionalInitialStateTransitions(
            IScreenFlowConfigurator preStartCfg, UiFlowContextData contextData)
        {
            return preStartCfg
                .OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
                .OnEventNavigatesTo(ScreenEvent.Start, AccountAndMeterDetailsStep.ShowAccountAndMeterDetails);
        }

        public class RootScreenModel : InitialFlowScreenModel, IAccountAndMeterDetailsInput
		{
			public string AccountNumber { get; set; }
		}
    }
}