using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.FlowDefinitions;
using NLog;


namespace EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps
{
    public class ShowMovingHouseUnhandledError : MovingHouseScreen
    {
	    public override ScreenName ScreenStep => MovingHouseStep.ShowMovingHouseUnhandledError;
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
        {
	        Log.Debug($"{nameof(ShowMovingHouseUnhandledError)}. LastError:{contextData.LastError.ExceptionMessage}. OccurredOnStep: {contextData.LastError.OccurredOnStep}");

			var screenModel = new ScreenModel();

			SetTitle(Title, screenModel);

			return screenModel;
        }

        protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
            IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
        {
	        return screenConfiguration.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred);
        }


        public class ScreenModel : UiFlowScreenModel
        {
	        public override bool IsValidFor(ScreenName screenStep)
            {
                return screenStep == MovingHouseStep.ShowMovingHouseUnhandledError;
            }
        }
    }
}