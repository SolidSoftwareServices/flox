using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.ContactUs.FlowDefinitions;
using EI.RP.WebApp.Flows.AppFlows.RequestRefund.FlowDefinitions;

namespace EI.RP.WebApp.Flows.AppFlows.RequestRefund.Steps
{
    public class ShowStatusMessage : RequestRefundScreen
    {
	    public override ScreenName ScreenStep => RequestRefundStep.ShowStatusMessage;

        protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
        {
            var screenModel = new ScreenModel();

            SetTitle(Title, screenModel);

            return screenModel;
        }

        public class ScreenModel : UiFlowScreenModel
        {
            public override bool IsValidFor(ScreenName screenStep)
            {
                return screenStep == RequestRefundStep.ShowStatusMessage;
            }
        }
    }
}
