using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.Http.Session;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.TermsInfo.FlowDefinitions;

namespace EI.RP.WebApp.Flows.AppFlows.TermsInfo.Steps
{
    public class TermsInfoIndex : TermsInfoScreen
    {
        private readonly IUserSessionProvider _userSessionProvider;
       
        public TermsInfoIndex(IUserSessionProvider userSessionProvider)
	    {
           _userSessionProvider = userSessionProvider;
        }
        
        public override ScreenName ScreenStep => TermsInfoStep.TermsInfoIndex;

        protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
        {
            var rootData = contextData.GetStepData<TermsInfoFlowInitializer.RootScreenModel>(ScreenName.PreStart);

            var screenModel = new TermsInfoScreenModel
            {
	            StartType = rootData.StartType
            };

            SetTitle(ResolveTitle(), screenModel);

            return screenModel;

            string ResolveTitle()
            {
	            switch (rootData.StartType)
	            {
		            case TermsInfoFlowInitializer.StartType.Disclaimer:
			            return "Disclaimer";
		            case TermsInfoFlowInitializer.StartType.TermsAndConditions:
			            return "Terms & Conditions";
		            case TermsInfoFlowInitializer.StartType.Privacy:
			            return "Privacy Notice and Cookies Policy";
		            default:
			            return null;
	            }
            }
        }

        public class TermsInfoScreenModel : UiFlowScreenModel
        {
            public override bool IsValidFor(ScreenName screenStep)
            {
                return screenStep == TermsInfoStep.TermsInfoIndex;
            }

            public TermsInfoFlowInitializer.StartType StartType { get; set; }

            public string AccountNumber { get; set; }
        }
    }
}