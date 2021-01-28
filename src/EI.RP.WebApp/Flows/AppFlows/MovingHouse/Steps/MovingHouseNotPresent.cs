using EI.RP.UiFlows.Core.Flows;

using EI.RP.WebApp.Flows.AppFlows.MovingHouse.FlowDefinitions;
using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;

namespace EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps
{
    public class MovingHouseNotPresent: MovingHouseScreen
    {
        public override ScreenName ScreenStep => MovingHouseStep.MovingHouseNotPresent;

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
                return screenStep == MovingHouseStep.MovingHouseNotPresent;
            }
        }
    }
}
