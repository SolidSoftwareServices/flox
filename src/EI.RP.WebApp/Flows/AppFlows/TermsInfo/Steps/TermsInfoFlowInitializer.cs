using EI.RP.CoreServices.Http.Session;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Initialization;
using EI.RP.UiFlows.Core.Flows.Initialization.Models;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.TermsInfo.FlowDefinitions;

namespace EI.RP.WebApp.Flows.AppFlows.TermsInfo.Steps
{
    public class TermsInfoFlowInitializer : UiFlowInitializationStep<ResidentialPortalFlowType, TermsInfoFlowInitializer.RootScreenModel>
    {
        private readonly IUserSessionProvider _userSessionProvider;

        public enum StartType
        {
            Disclaimer = 1,
            TermsAndConditions,
            Privacy
        }

	    public override ResidentialPortalFlowType InitializerOfFlowType => ResidentialPortalFlowType.TermsInfo;

		public TermsInfoFlowInitializer(IUserSessionProvider userSessionProvider) 
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
                .OnEventNavigatesTo(ScreenEvent.Start, TermsInfoStep.TermsInfoIndex);
        }
        public class RootScreenModel : InitialFlowScreenModel, ITermsInfoInput
		{
            public StartType StartType { get; set; }
        }
    }
}