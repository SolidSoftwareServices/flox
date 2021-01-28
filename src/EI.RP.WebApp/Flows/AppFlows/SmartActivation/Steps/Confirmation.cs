using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.SmartActivation.FlowDefinitions;

namespace EI.RP.WebApp.Flows.AppFlows.SmartActivation.Steps
{
    public class Confirmation : SmartActivationScreen
    {
	    protected string StepTitle => string.Join(" | ", "Confirmation", Title);

        public override ScreenName ScreenStep => SmartActivationStep.Confirmation;

		protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(IScreenFlowConfigurator screenConfiguration,
			IUiFlowContextData contextData)
		{
			return screenConfiguration
				.OnEventNavigatesTo(ScreenEvent.ErrorOccurred, SmartActivationStep.ShowFlowGenericError);
		}

		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
        {
			var step2Data = contextData.GetStepData<Step2SelectPlan.ScreenModel>();
			var stepData = new ScreenModel
			{
				SelectedPlanName = step2Data.SelectedPlanName
			};

			SetTitle(StepTitle, stepData);

			return stepData;
		}

		public sealed class ScreenModel : UiFlowScreenModel
        {
            public ScreenModel(): base(false)
            {
            }

			public string SelectedPlanName { get; set; }

			public override bool IsValidFor(ScreenName screenStep)
            {
                return screenStep == SmartActivationStep.Confirmation;
            }
        }
    }
}