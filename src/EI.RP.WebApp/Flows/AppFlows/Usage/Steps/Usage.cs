using EI.RP.CoreServices.System;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.Usage.FlowDefinitions;

namespace EI.RP.WebApp.Flows.AppFlows.Usage.Steps
{
    public class Usage : UsageScreen
    {
	    public override ScreenName ScreenStep => UsageStep.Usage;

        public static class StepEvent
        {
        }

        protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
        {
            var rootData = contextData.GetStepData<UsageFlowInitializer.RootScreenModel>(ScreenName.PreStart);

            var screenModel = new ScreenModel
            {
	            AccountNumber = rootData.AccountNumber
            };

            SetTitle(Title, screenModel);

            return screenModel;
        }

        protected override async Task<UiFlowScreenModel> OnRefreshStepDataAsync(IUiFlowContextData contextData,
            UiFlowScreenModel originalScreenModel,
            IDictionary<string, object> stepViewCustomizations = null)
        {
            var refreshedStepData = originalScreenModel.CloneDeep();
            var updatedStepData = (ScreenModel)refreshedStepData;

            SetTitle(Title, updatedStepData);

            return updatedStepData;
        }

        public class ScreenModel : UiFlowScreenModel
        {
            public override bool IsValidFor(ScreenName screenStep) => screenStep == UsageStep.Usage;

            public string AccountNumber { get; set; }
        }
    }
}
