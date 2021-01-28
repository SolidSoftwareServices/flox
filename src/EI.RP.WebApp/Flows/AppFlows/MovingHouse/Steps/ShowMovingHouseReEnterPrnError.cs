using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.FlowDefinitions;
using NLog;


namespace EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps
{
    public class ShowMovingHouseReEnterPrnError : MovingHouseScreen
    {
	    public override ScreenName ScreenStep => MovingHouseStep.ShowMovingHouseReEnterPrnError;
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
        {
			var stepData = new ScreenModel();

			SetTitle(Title, stepData);

			var step2Data = contextData.GetStepData<Step2InputPrns.ScreenModel>();

			if (step2Data != null)
            {
	            stepData.MovingHouseType = step2Data.MovingHouseType;
	            stepData.MPRNExist = step2Data.MPRNExist;
	            stepData.GPRNExist = step2Data.GPRNExist;
	            stepData.IsAddressMatch = step2Data.PrnsAreFromSameAddress;
            }

            return stepData;
        }

        public static class StepEvent
        {
            public static readonly ScreenEvent ReEnterMprn = new ScreenEvent(nameof(ShowMovingHouseUnhandledError),nameof(ReEnterMprn));
        }

        protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
            IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
        {
            return screenConfiguration
                .OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
                .OnEventNavigatesTo(StepEvent.ReEnterMprn, MovingHouseStep.Step2InputPrns);
        }

        protected override async Task OnHandlingStepEvent(ScreenEvent triggeredEvent,
            IUiFlowContextData contextData)
        {
            if (triggeredEvent == StepEvent.ReEnterMprn)
            {
                var step2Data = contextData.GetStepData<Step2InputPrns.ScreenModel>();
                step2Data.MPRNExist = true;
                step2Data.PrnsAreFromSameAddress = true;
            }
        }

        public class ScreenModel : UiFlowScreenModel
        {
            public MovingHouseType MovingHouseType { get; set; }

            public bool MPRNExist { get; set; }

            public bool IsAddressMatch { get; set; }

            public bool GPRNExist { get; set; } 
            public override bool IsValidFor(ScreenName screenStep)
            {
                return screenStep == MovingHouseStep.ShowMovingHouseReEnterPrnError;
            }
        }
    }
}
