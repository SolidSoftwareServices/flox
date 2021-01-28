using System.Collections.Generic;
using System.Threading.Tasks;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.System.Identity;
using Ei.Rp.DomainModels.Infrastructure;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Initialization;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.BusinessPartnersSearch.FlowDefinitions;

namespace EI.RP.WebApp.Flows.AppFlows.BusinessPartnersSearch.Steps
{
	public class FlowInitializer : UiFlowInitializationStep<ResidentialPortalFlowType>
	{
		private readonly IUserSessionProvider _userSessionProvider;

		public override ResidentialPortalFlowType InitializerOfFlowType =>
			ResidentialPortalFlowType.BusinessPartnersSearch;
		public FlowInitializer(IUserSessionProvider userSessionProvider) 
		{
			_userSessionProvider = userSessionProvider;
		}

		public override bool Authorize()
		{
			return _userSessionProvider.IsAgentOrAdmin();
		}

		public override IScreenFlowConfigurator OnDefiningAdditionalInitialStateTransitions(
			IScreenFlowConfigurator preStartCfg, UiFlowContextData contextData)
		{
			return preStartCfg
				.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
				.OnEventNavigatesTo(ScreenEvent.Start, BusinessPartnersSearchStep.SearchAndShowResultsStep);
		}

	

		public static class StepEvent
		{
		}

	}
}