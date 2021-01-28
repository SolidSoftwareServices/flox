﻿using EI.RP.CoreServices.Http.Session;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Initialization;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.Help.FlowDefinitions;

namespace EI.RP.WebApp.Flows.AppFlows.Help.Steps
{
	public class HelpFlowInitializer : UiFlowInitializationStep<ResidentialPortalFlowType>
	{
		private readonly IUserSessionProvider _userSessionProvider;
		public override ResidentialPortalFlowType InitializerOfFlowType => ResidentialPortalFlowType.Help;

		public HelpFlowInitializer(IUserSessionProvider userSessionProvider) 
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
				.OnEventNavigatesTo(ScreenEvent.Start, HelpStep.Help);
		}
	}
}