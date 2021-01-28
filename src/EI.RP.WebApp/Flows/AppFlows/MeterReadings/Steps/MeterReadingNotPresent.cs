using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Queries.Metering.Devices;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.MeterReadings.FlowDefinitions;

namespace EI.RP.WebApp.Flows.AppFlows.MeterReadings.Steps
{
    public class MeterReadingNotPresent : MeterReadingScreen
    {
	    protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
        {
            return screenConfiguration
                .OnEventReentriesCurrent(ScreenEvent.ErrorOccurred);
        }

        public override ScreenName ScreenStep => MeterReadingStep.MeterReadingNotPresent;
        private readonly IDomainQueryResolver _domainQueryResolver;

        public MeterReadingNotPresent(IDomainQueryResolver domainQueryResolver)
        {
	        _domainQueryResolver = domainQueryResolver;
        }

        protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
        {
            var rootData = contextData.GetStepData<MeterReadingFlowInitializer.RootScreenModel>(ScreenName.PreStart);

            var devices = await _domainQueryResolver.GetDevicesByAccount(rootData.AccountNumber);
            var screenModel = new ScreenModel
            {
	            IsSmartActiveAccount =
		            devices.Any(x => x.SmartActivationStatus == SmartActivationStatus.SmartActive)
            };
            SetTitle(Title, screenModel);

            return screenModel;
        }

        public class ScreenModel : UiFlowScreenModel
        {
            
            public bool IsSmartActiveAccount { get; set; }
            public override bool IsValidFor(ScreenName screenStep)
            {
                return screenStep == MeterReadingStep.MeterReadingNotPresent;
            }
        }
    }
}
