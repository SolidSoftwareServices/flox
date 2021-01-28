using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.System.Identity;
using Ei.Rp.DomainModels.Infrastructure;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Initialization;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.Agent.FlowDefinitions;

namespace EI.RP.WebApp.Flows.AppFlows.Agent.Steps
{
    public class AgentFlowInitializer : UiFlowInitializationStep<ResidentialPortalFlowType>
    {
        private readonly IUserSessionProvider _userSessionProvider;
	    public override ResidentialPortalFlowType InitializerOfFlowType => ResidentialPortalFlowType.Agent;

		public AgentFlowInitializer(
            IUserSessionProvider userSessionProvider
        )
        {
            _userSessionProvider = userSessionProvider;
        }

        public override bool Authorize()
        {
            return _userSessionProvider.IsAgentOrAdmin();
        }

        public override IScreenFlowConfigurator OnDefiningAdditionalInitialStateTransitions(
            IScreenFlowConfigurator preStartCfg,
            UiFlowContextData contextData)
        {
            preStartCfg.OnEventNavigatesTo(ScreenEvent.Start, AgentStep.AgentUiFlowContainerScreenStep);
            return preStartCfg;
        }
    }
}