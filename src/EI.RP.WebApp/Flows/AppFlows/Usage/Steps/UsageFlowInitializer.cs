using EI.RP.CoreServices.Http.Session;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Initialization;
using EI.RP.UiFlows.Core.Flows.Initialization.Models;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.Usage.FlowDefinitions;

namespace EI.RP.WebApp.Flows.AppFlows.Usage.Steps
{
    public class UsageFlowInitializer : UiFlowInitializationStep<ResidentialPortalFlowType, UsageFlowInitializer.RootScreenModel>
    {
        private readonly IUserSessionProvider _userSessionProvider;
        public override ResidentialPortalFlowType InitializerOfFlowType => ResidentialPortalFlowType.Usage;

        public UsageFlowInitializer(IUserSessionProvider userSessionProvider)
        {
            _userSessionProvider = userSessionProvider;
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
                .OnEventNavigatesTo(ScreenEvent.Start, UsageStep.Usage);
        }

        public class RootScreenModel : InitialFlowScreenModel, IUsageInput
        {
            public string AccountNumber { get; set; }

        }

    }
}
