using System.Collections.Generic;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.CoreServices.System;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.MeterReadings.FlowDefinitions;
using EI.RP.WebApp.Infrastructure.Extensions;

namespace EI.RP.WebApp.Flows.AppFlows.MeterReadings.Steps
{
    public class ErrorSubmittingMeterReading : MeterReadingScreen
    {
	    public ErrorSubmittingMeterReading()
        {
        }

        protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
        {
            return screenConfiguration
                .OnEventReentriesCurrent(ScreenEvent.ErrorOccurred);
        }

        public override ScreenName ScreenStep => MeterReadingStep.ErrorSubmittingMeterReadingStep;

        protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
        {
            var rootData = contextData.GetStepData<MeterReadingFlowInitializer.RootScreenModel>(ScreenName.PreStart);

            var screenModel = new ScreenModel
            {
	            AccountNumber = rootData.AccountNumber,
                AccountType = rootData.AccountType,
                ErrorMessage = contextData.GetLastError()
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

            if (stepViewCustomizations != null)
            {
                updatedStepData.SetFlowCustomizableValue(stepViewCustomizations, x => x.PageIndex);
            }

            return updatedStepData;
        }

        public class ScreenModel : UiFlowScreenModel
		{
            public override bool IsValidFor(ScreenName screenStep)
            {
                return screenStep == MeterReadingStep.ErrorSubmittingMeterReadingStep;
            }

            public string ErrorMessage { get; set; }
            public string AccountNumber { get; set; }

            public ClientAccountType AccountType { get; set; }

            public int PageIndex { get; set; }
        }
    }
}
