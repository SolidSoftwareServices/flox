using System.Collections.Generic;
using EI.RP.CoreServices.Http.Session;
using EI.RP.UiFlows.Core.Flows;


using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Initialization;
using EI.RP.UiFlows.Core.Flows.Initialization.Models;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.ContactUs.FlowDefinitions;

namespace EI.RP.WebApp.Flows.AppFlows.ContactUs.Steps
{
    public class ContactUsFlowInitializer : UiFlowInitializationStep<ResidentialPortalFlowType, ContactUsFlowInitializer.RootScreenModel>
    {
        private readonly IUserSessionProvider _userSessionProvider;
        public static class StepEvent
        {

        }

	    public override ResidentialPortalFlowType InitializerOfFlowType => ResidentialPortalFlowType.ContactUs;

		public ContactUsFlowInitializer(IUserSessionProvider userSessionProvider) 

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
                .OnEventNavigatesTo(ScreenEvent.Start, ContactUsStep.ContactUs);
        }



        public class RootScreenModel : InitialFlowScreenModel, IContactUsInput
		{
            public string AccountNumber { get; set; }

        }

    }
}
