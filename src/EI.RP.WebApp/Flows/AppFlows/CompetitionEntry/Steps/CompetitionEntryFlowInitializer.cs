using EI.RP.CoreServices.Http.Session;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Initialization;
using EI.RP.UiFlows.Core.Flows.Initialization.Models;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.CompetitionEntry.FlowDefinitions;

namespace EI.RP.WebApp.Flows.AppFlows.CompetitionEntry.Steps
{
    public class CompetitionEntryFlowInitializer : UiFlowInitializationStep<ResidentialPortalFlowType, CompetitionEntryFlowInitializer.RootScreenModel>
    {
        private readonly IUserSessionProvider _userSessionProvider;
        public override ResidentialPortalFlowType InitializerOfFlowType => ResidentialPortalFlowType.CompetitionEntry;

        public CompetitionEntryFlowInitializer(IUserSessionProvider userSessionProvider)
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
                .OnEventNavigatesTo(ScreenEvent.Start, CompetitionEntryStep.CompetitionEntry);
        }

        public class RootScreenModel : InitialFlowScreenModel, ICompetitionEntryInput
        {
            public string AccountNumber { get; set; }

        }

    }
}
