using EI.RP.CoreServices.Http.Session;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Initialization;
using EI.RP.UiFlows.Core.Flows.Initialization.Models;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.UserContactDetails.FlowDefinitions;

namespace EI.RP.WebApp.Flows.AppFlows.UserContactDetails.Steps
{
	public class UserContactFlowInitializer : UiFlowInitializationStep<ResidentialPortalFlowType,
		UserContactFlowInitializer.RootScreenModel>
	{
		private readonly IUserSessionProvider _userSessionProvider;

		public static class StepEvent
		{

		}

		public UserContactFlowInitializer(IUserSessionProvider userSessionProvider)

		{
			_userSessionProvider = userSessionProvider;
		}

		public override bool Authorize()
		{
			return !_userSessionProvider.IsAnonymous();
		}

		public override ResidentialPortalFlowType InitializerOfFlowType => ResidentialPortalFlowType.UserContactDetails;

		public override IScreenFlowConfigurator OnDefiningAdditionalInitialStateTransitions(
			IScreenFlowConfigurator preStartCfg, UiFlowContextData contextData)
		{
			return preStartCfg
				.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
				.OnEventNavigatesTo(ScreenEvent.Start, UserContactDetailsStep.UserContactDetails,
					() => contextData.GetStepData<RootScreenModel>(ScreenName.PreStart).InitialFlowStartType == UserContactFlowType.ContactDetails,
					"Configure Contact Details")
				.OnEventNavigatesTo(ScreenEvent.Start, UserContactDetailsStep.MarketingPreferences,
				() => contextData.GetStepData<RootScreenModel>(ScreenName.PreStart).InitialFlowStartType == UserContactFlowType.MarketingPreferences,
			"Configure Marketing Peferences");
		}

		public class RootScreenModel : InitialFlowScreenModel, IUserContactDetailsFlowInput
		{
			public UserContactFlowType InitialFlowStartType { get; set; }
			public string AccountNumber { get; set; }

		}
	}
}