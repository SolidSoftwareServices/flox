using System.Collections.Generic;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.CoreServices.System;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.MeterReadings.FlowDefinitions;

namespace EI.RP.WebApp.Flows.AppFlows.MeterReadings.Steps
{
    public class ErrorSubmittedLessThanActual : MeterReadingScreen
    {
	    private const string ErrorLessThanActualESBNetwork = "Sorry unfortunately we cannot accept your reading as it is less than the last ESB networks provided actual read. If you are sure your reading is correct please send a dated photo of your meter to meterreadingchanges@electricireland.ie. ";
		private const string ErrorLessThanActualGasNetwork = "Sorry unfortunately we cannot accept your reading as it is less than the last Gas Networks Ireland provided actual read. If you are sure your reading is correct please send a dated photo of your meter to meterreadingchanges@electricireland.ie. ";
		private const string ErrorLessThanActualCustomer = "Sorry unfortunately we cannot accept your reading as it is less than the last customer read. If you are sure your reading is correct please send a dated photo of your meter to meterreadingchanges@electricireland.ie. ";
		private const string ErrorElectricityAccountType = "Please ensure your MPRN is in the subject line of the email.";
		private const string ErrorGasAccountType = "Please ensure your GPRN is in the subject line of the email.";

		public ErrorSubmittedLessThanActual()
        {
        }

        protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
        {
            return screenConfiguration
                .OnEventReentriesCurrent(ScreenEvent.ErrorOccurred);
        }

        public override ScreenName ScreenStep => MeterReadingStep.ErrorSubmittedLessThanActual;

        protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
        {
            var rootData = contextData.GetStepData<MeterReadingFlowInitializer.RootScreenModel>(ScreenName.PreStart);
			var stepSubmitReadingsData = contextData.GetStepData<SubmitMeterReading.ScreenModel>();

			var screenModel = new ScreenModel
			{
				AccountNumber = rootData.AccountNumber,
				AccountType = rootData.AccountType,
				ErrorMessage = ResolveErrorMessage(rootData.AccountType, 
												   stepSubmitReadingsData.HasMeterReadingLessThanActualNetworkError, 
												   stepSubmitReadingsData.HasMeterReadingLessThanActualCustomerError)
			};

			SetTitle(Title, screenModel);

            return screenModel;
        }

		private string ResolveErrorMessage(ClientAccountType accountType, 
										   bool hasMeterReadingLessThanActualNetworkError, 
										   bool hasMeterReadingLessThanActualCustomerError)
		{
			var ret = "";
			if (hasMeterReadingLessThanActualNetworkError)
				ret = accountType == ClientAccountType.Electricity ? $"{ErrorLessThanActualESBNetwork}{ErrorElectricityAccountType}" 
																   : $"{ErrorLessThanActualGasNetwork}{ErrorGasAccountType}";
			if (hasMeterReadingLessThanActualCustomerError)
				ret = accountType == ClientAccountType.Electricity ? $"{ErrorLessThanActualCustomer}{ErrorElectricityAccountType}"
																   : $"{ErrorLessThanActualCustomer}{ErrorGasAccountType}";			
			return ret;
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
                return screenStep == MeterReadingStep.ErrorSubmittedLessThanActual;
            }

            public string ErrorMessage { get; set; }
            public string AccountNumber { get; set; }
            public ClientAccountType AccountType { get; set; }
            public int PageIndex { get; set; }
        }
    }
}
