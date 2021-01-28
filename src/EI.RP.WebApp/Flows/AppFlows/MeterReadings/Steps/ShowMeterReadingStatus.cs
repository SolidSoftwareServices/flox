using System.Collections.Generic;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.MeterReadings.FlowDefinitions;

namespace EI.RP.WebApp.Flows.AppFlows.MeterReadings.Steps
{
    public class ShowMeterReadingStatus : MeterReadingScreen
    {
	    public ShowMeterReadingStatus()
        {
        }

        protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
        {
            return screenConfiguration
                .OnEventReentriesCurrent(ScreenEvent.ErrorOccurred);
        }

        public override string ViewPath { get; } = "ShowSuccessMessage";

        public override ScreenName ScreenStep => MeterReadingStep.ShowMeterReadingStatus;

        protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
        {
            var rootData = contextData.GetStepData<MeterReadingFlowInitializer.RootScreenModel>(ScreenName.PreStart);

            var screenModel = new ScreenModel
            {
	            AccountNumber = rootData.AccountNumber,
                Partner = rootData.Partner,
                AccountType = rootData.AccountType
            };

            SetTitle(Title, screenModel);

            return screenModel;
        }

        protected override async Task<UiFlowScreenModel> OnRefreshStepDataAsync(IUiFlowContextData contextData,
	        UiFlowScreenModel originalScreenModel,
	        IDictionary<string, object> stepViewCustomizations = null)
        {
            var refreshedStepData = originalScreenModel.CloneDeep();
            var updatedStepData = (ShowMeterReadingStatus.ScreenModel)refreshedStepData;

            SetTitle(Title, updatedStepData);

            if (stepViewCustomizations != null)
            {
                updatedStepData.SetFlowCustomizableValue(stepViewCustomizations, x => x.PageIndex);
            }

            return updatedStepData;
        }

        public class ScreenModel : UiFlowScreenModel
		{
            public string AccountNumber { get; set; }
            public string Partner { get; set; }

            public string AccountType { get; set; }

			public override bool IsValidFor(ScreenName screenStep)
            {
                return screenStep == MeterReadingStep.ShowMeterReadingStatus;
            }

            public int PageIndex { get; set; }
        }
	}
}
